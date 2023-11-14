namespace Netflix
{
    internal sealed class FakeMessagingApi : SdkApi.IMessaging
    {
        public void OnPushToken(string pushDeviceToken)
        {
            NfLog.Log("OnPushToken: " + pushDeviceToken);
        }

        public void OnDeeplinkReceived(string deepLinkURL, bool processedByGame)
        {
            NfLog.Log("OnDeeplinkReceived: " + deepLinkURL);
        }

        public void OnMessagingEvent(NetflixMessaging.MessagingEventType eventType, string jsonString)
        {
            NfLog.Log("OnMessagingEvent: " + eventType);
        }
    }
}