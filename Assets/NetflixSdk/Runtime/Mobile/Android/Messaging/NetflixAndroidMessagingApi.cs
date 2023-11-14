#if UNITY_ANDROID
using UnityEngine;

namespace Netflix
{
    internal class NetflixAndroidMessagingApi: SdkApi.IMessaging
    {
        private static AndroidJavaObject nfgSdk;
        public NetflixAndroidMessagingApi(AndroidJavaObject sdk)
        {
            nfgSdk = sdk;
        }

        public void OnPushToken(string pushDeviceToken)
        {
            NfLog.Log("Android OnPushToken:" + pushDeviceToken);
            nfgSdk.Call("onPushToken", pushDeviceToken);
        }


        public void OnMessagingEvent(NetflixMessaging.MessagingEventType eventType, string jsonString)
        {
            var intVal = (int)eventType;
            NfLog.Log("Android OnMessagingEvent: " + eventType + " intVal=" + intVal);
            nfgSdk.Call("onMessagingEvent", intVal, jsonString);
        }

        public void OnDeeplinkReceived(string deepLinkURL, bool processedByGame)
        {
            NfLog.Log("Android OnDeeplinkReceived:" + deepLinkURL + " processedByGame:" + processedByGame);
            nfgSdk.Call("onDeeplinkReceived", deepLinkURL, processedByGame);
        }

    }
}
#endif