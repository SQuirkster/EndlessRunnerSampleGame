#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Netflix
{
    internal sealed class NetflixSdkAndroidImpl : NetflixSdkImpl
    {
        // Android only variables
        private const string NF_UNITY_SDK_JAVA_OBJECT_NAME = "com.netflix.unity.api.NfUnitySdk";
        private static NetflixAndroidCallback gNetflixCallback;
        private static AndroidJavaObject nfgSdkImpl;
        private static AndroidJavaObject nfgSdk
        {
            get
            {
                if (nfgSdkImpl == null)
                {
                    AndroidJavaObject sdk = new AndroidJavaObject(NF_UNITY_SDK_JAVA_OBJECT_NAME);
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                    NfLog.Log("SdkBridge calling init");
                    sdk.Call("init", context);
                    gNetflixCallback = new NetflixAndroidCallback();
                    sdk.Call("registerNetflixCallback", gNetflixCallback);
                    NfLog.Log("SdkBridge init called");
                    nfgSdkImpl = sdk;
                }
                return nfgSdkImpl;
            }
        }

        protected override SdkApi.IAchievements CreateAchievementsApi()
        {
            return new NetflixAndroidAchievementsApi(nfgSdk);
        }

        protected override SdkApi.ILeaderboards CreateLeaderboardsApi()
        {
            return new NetflixAndroidLeaderboardsApi(nfgSdk);
        }
        
        protected override SdkApi.IStats CreateStatsApi()
        {
            return new NetflixAndroidStatsApi(nfgSdk);
        }
        
        protected override SdkApi.IPlayerIdentity CreatePlayerIdentityApi()
        {
            return new NetflixAndroidPlayerIdentitiesApi(nfgSdk);
        }
        
        protected override SdkApi.ICloudSave CreateCloudSaveApi()
        {
            return new NetflixAndroidCloudSaveApi(nfgSdk);
        }
        
        protected override SdkApi.IMessaging CreateMessagingApi()
        {
            return new NetflixAndroidMessagingApi(nfgSdk);
        }

        private static AndroidJavaObject GetAndroidActivity()
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        public override void CheckUserAuth()
        {
            NfLog.Log("NetflixSdkAndroidImpl CheckUserAuth");
            nfgSdk.Call("checkUserAuth", GetAndroidActivity());
        }

        public override void ShowNetflixMenu(int location)
        {
            NfLog.Log("NetflixSdkAndroidImpl ShowNetflixMenu");
            nfgSdk.Call("showNetflixMenu",
                GetAndroidActivity(),
                location
                );
        }

        public override void HideNetflixMenu()
        {
            NfLog.Log("NetflixSdkAndroidImpl HideNetflixMenu");
            nfgSdk.Call("hideNetflixMenu",
                GetAndroidActivity()
                );
        }

        public override void ShowNetflixAccessButton()
        {
            NfLog.Log("NetflixSdkAndroidImpl ShowNetflixAccessButton");
            nfgSdk.Call("showNetflixAccessButton",
                GetAndroidActivity()
                );
        }

        public override void HideNetflixAccessButton()
        {
            NfLog.Log("NetflixSdkAndroidImpl HideNetflixAccessButton");
            nfgSdk.Call("hideNetflixAccessButton",
                GetAndroidActivity()
                );
        }

        public override void SetLocale(NetflixSdk.Locale locale)
        {
            NfLog.Log("NetflixSdkAndroidImpl SetLocale");
            nfgSdk.Call("setLocale",
                locale.language, locale.country, locale.variant
                );
        }

        public override SdkApi.CrashReporterConfig GetCrashReporterConfig()
        {
            NfLog.Log("NetflixSdkAndroidImpl GetCrashReporterConfig");
            AndroidJavaObject netflixCrashConfig = nfgSdk.Call<AndroidJavaObject>("getCrashReporterConfig");
            SdkApi.CrashReporterConfig config = Netflix.NetflixJavaConverter.ToCrashReporterConfig(netflixCrashConfig);
            NfLog.Log("NetflixSdkAndroidImpl bugsnagConfig guid=" + config.guid);
            return config;
        }

        public override string GetNativeSdkVersion()
        {
            return nfgSdk.Call<string>("getNfgSdkVersion");
        }

        protected override string GetUnitySdkVersion()
        {
            return nfgSdk.Call<string>("getUnitySdkVersion");
        }
        
        public override void SendCLEvent(string clTypeName, string eventDataJson)
        {
            NfLog.Log("NetflixSdkAndroidImpl SendInGameEvent");
            nfgSdk.Call("sendCLEvent", clTypeName, eventDataJson);
        }

        public override void LogInGameEvent(InGameEvent inGameEvent)
        {
            NfLog.Log("NetflixSdkAndroidImpl LogInGameEvent");
            nfgSdk.Call("sendCLEvent", inGameEvent.name, inGameEvent.ToJson());
        }
        
        public override void PublishToEventSink(string name, string data)
        {
            NfLog.Log("LogToEventSink(" + name + ", " + data + ")");
            nfgSdk.Call("logToEventSink", name, data);
        }

        public override Dictionary<string, string> GetTestParams()
        {
            var jsonStr = nfgSdk.Call<string>("getTestParams");
            NfLog.Log("GetTestParams() returned " + jsonStr);
            var testParamsDictionary = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(jsonStr)) return testParamsDictionary;
            var testParams = JsonUtility.FromJson<TestParams>(jsonStr);
            if (testParams != null && testParams.explicitCheckUserAuth != null)
            {
                testParamsDictionary.Add("explicitCheckUserAuth", testParams.explicitCheckUserAuth);
            }
            return testParamsDictionary;
        }

        [Serializable]
        internal class TestParams {
            public string explicitCheckUserAuth;
        }
    }
}
#endif