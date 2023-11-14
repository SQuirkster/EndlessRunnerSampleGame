using UnityEngine;
using System.Runtime.InteropServices;

namespace Netflix
{
    public class NetflixDeepLinkHandler : MonoBehaviour
    {
        public static NetflixDeepLinkHandler Instance { get; private set; }
        public string deeplinkURL;
        private void Awake()
        {
            NfLog.Log("NetflixDeepLinkHandler awake");
            if (Instance == null)
            {
                Instance = this;                
                Application.deepLinkActivated += onDeepLinkActivated;
                if (!string.IsNullOrEmpty(Application.absoluteURL))
                {
                    // Cold start and Application.absoluteURL not null so process Deep Link.
                    onDeepLinkActivated(Application.absoluteURL);
                }
                // Initialize DeepLink Manager global variable.
                else deeplinkURL = "[none]";
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void ngp_on_deeplink_received(string deeplinkUrl, bool processedByGame);
#endif
        private void onDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            deeplinkURL = url;
            NfLog.Log("deeplinkURL = " + deeplinkURL);

#if UNITY_IOS
            ngp_on_deeplink_received(deeplinkURL, false);
#endif
        }
    }
}
