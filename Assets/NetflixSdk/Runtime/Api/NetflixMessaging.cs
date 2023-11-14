using UnityEngine;

namespace Netflix
{
    public class NetflixMessaging
    {
        public enum MessagingEventType
        {
            // push message has been received by the device.
            PushNotificationReceived = 0,
            // push notification has been shown to the user.
            PushNotificationPresented = 1,
            // user has taken an action on the push notification e.g. clicked on the notification.
            PushNotificationAcknowledged = 2,
            // push notification is cleared explicitly by the user.
            PushNotificationDismissed = 3,
            // not currently used
            PushNotificationIgnored = 4,
            // local notification is scheduled by the user
            LocalNotificationScheduled = 5,

        }

        /**
         * OnPushToken() must be called by the game-app to register
         * the device token with the Netflix backend to receive the push messages
         * on the device.
         *
         * Examples
         *
         * 1. Apple: deviceToken is a unique token that identifies this device to APNs.
         *
         * More details: https://developer.apple.com/documentation/uikit/uiapplicationdelegate/1622958-application
         *
         * 2. FCM: registration token
         *
         * More details: https://firebase.google.com/docs/reference/android/com/google/firebase/messaging/FirebaseMessaging#getToken()
         *
         *
         */
        public static void OnPushToken(string pushDeviceToken)
        {
            Debug.Log("OnPushToken " + pushDeviceToken);
            SdkHolder.nfsdk.GetMessagingApi().OnPushToken(pushDeviceToken);
        }

        /**
         * When eventType=PushNotificationReceived then jsonString will be full payload received
         * via the push message.
         * For all other eventTypes it will be an empty json to provide flexibility
         * to add parameters in the future.
         *
         */
        public static void OnMessagingEvent(MessagingEventType eventType, string jsonString)
        {
            Debug.Log("onMessagingEvent " + eventType);
            SdkHolder.nfsdk.GetMessagingApi().OnMessagingEvent(eventType, jsonString);
        }

        /*
         * OnDeeplinkReceived() must be called by the game-app when the game-app
         * receives/consumes the deep-link URL. If the deep-link URL is processed
         * i.e. (game-app was able to successfully deep-linked into the requested
         * screen) by the game-app then it must pass processedByGame=true
         * to the NetflixSDK. Otherwise, it should pass processedByGame=false.
         *
         * Examples (non-exhaustive list):
         *
         * 1) When a user clicks on a Notification displayed as a result of a push
         *    message.
         * 2) An email containing the deep-link is clicked by the user and game-app
         *    consumes the deep-link URL.
         * 3) A link shared via other mobile apps or browsers is clicked and
         *    consumed by the game-app.
         *
         * If there is no deep-link URL in the push message then deepLinkUrl
         * will be an OS specific app start URL for the launch screen.
         */
        public static void OnDeeplinkReceived(string deepLinkURL, bool processedByGame)
        {
            Debug.Log("OnDeeplinkReceived deepLinkURL=" + deepLinkURL + " processedByGame=" + processedByGame);
            SdkHolder.nfsdk.GetMessagingApi().OnDeeplinkReceived(deepLinkURL, processedByGame);
        }
    }
}
