using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Netflix
{

    internal class SdkProvider : MonoBehaviour
    {
        private static readonly object initializationLock = new object();
        private static Netflix.SdkApi sdkImpl;
        private static Thread mainThread;

        internal static bool IsInitialized()
        {
            return sdkImpl != null;
        }

        public static Netflix.SdkApi GetSdk()
        {
            if (sdkImpl == null)
            {
                InitializeSdkImpl();
            }
            return sdkImpl;
        }

        public static Thread GetMainThread()
        {
            return mainThread;
        }

        //Consider NetflixCloudSave.GetSlotIds() in Awake()
        internal static void InitializeSdkImpl()
        {
            lock (initializationLock)
            {
                if (sdkImpl == null)
                {
                    switch (Application.platform)
                    {
#if UNITY_ANDROID
                        case RuntimePlatform.Android:
                            if (sdkImpl == null)
                            {
                                sdkImpl = new Netflix.NetflixSdkAndroidImpl();
                                sdkImpl.SetupCrashReporter();
                            }
                            break;
#endif

#if UNITY_IOS
                        case RuntimePlatform.IPhonePlayer:
                            if (sdkImpl == null)
                            {
                                sdkImpl = new Netflix.NGPiOSBridge();
                                sdkImpl.SetupCrashReporter();
                            }
                            break;
#endif

#if UNITY_STANDALONE_LINUX
                        case RuntimePlatform.LinuxPlayer:
                            if (sdkImpl == null)
                            {
                                sdkImpl = new Netflix.NetflixSdkCloudImpl();
                                sdkImpl.SetupCrashReporter();
                            }
                            break;
#endif

                        default:
                            if (sdkImpl == null)
                            {
                                sdkImpl = new NetflixSdkImpl();
                            }
                            break;
                    }
                }
            }
        }

        void Awake()
        {
            mainThread = System.Threading.Thread.CurrentThread;
            InitializeSdkImpl();
            NfLog.Log("SdkBridge my gameObject is " + gameObject.name);
        }

        // Update is called once per frame
        void Update()
        {
            Netflix.ResponseManager.DispatchResponseOnUiThread();
            Netflix.NetflixEventSenderImpl.DispatchEventsToGameApp();
        }

        void OnNfEvent(string eventMessage)
        {
            NfLog.Log("SdkBridge OnNfEvent msg=" + eventMessage);
            // let it crash for now even if it is null.
            Netflix.NetflixEventSenderImpl.AddNetflixEvent(eventMessage);
        }
    }
}