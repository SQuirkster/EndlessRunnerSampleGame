#if UNITY_IOS

using System.Runtime.InteropServices;

namespace Netflix
{
    internal class NGPiOSMessaging : SdkApi.IMessaging
    {
        [DllImport("__Internal")]
        private static extern void ngp_on_push_token(string pushDeviceToken);
        [DllImport("__Internal")]
        private static extern void ngp_on_messaging_event(int eventType, string json_string);
        [DllImport("__Internal")]
        private static extern void ngp_on_deeplink_received(string deepLinkURL, bool processedByGame);

        public void OnPushToken(string pushDeviceToken)
        {
            NfLog.Log("OnPushToken: " + pushDeviceToken);
            ngp_on_push_token(pushDeviceToken);
        }

        public void OnMessagingEvent(NetflixMessaging.MessagingEventType eventType, string jsonString)
        {
            int intVal = (int)eventType;
            NfLog.Log("OnMessagingEvent: " + eventType + "intVal=" + intVal);
            // todo
            ngp_on_messaging_event(intVal, jsonString);
        }

        public void OnDeeplinkReceived(string deepLinkURL, bool processedByGame)
        {
            NfLog.Log("OnDeeplinkReceived: " + deepLinkURL);
            ngp_on_deeplink_received(deepLinkURL, processedByGame);
        }
    }
}
#endif