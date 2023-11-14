using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Netflix;
using UnityEngine;
using UnityEngine.UI;

public class GameAppEventHandler : MonoBehaviour
{
    public class EventConsumer : Netflix.Event.NetflixEventReceiver
    {
        GameAppEventHandler gameAppEventHandler;
        public EventConsumer(GameAppEventHandler gameAppEventHandler)
        {
            this.gameAppEventHandler = gameAppEventHandler;
        }

        public void OnNetflixUIVisible()
        {
            GameObject.Find("event").GetComponent<Text>().text = "event=OnNetflixUIVisible";
        }

        public void OnNetflixUIHidden()
        {
            GameObject.Find("event").GetComponent<Text>().text = "event=OnNetflixUIHidden";
        }

        public void OnUserStateChange(Netflix.NetflixSdk.NetflixSdkState sdkState)
        {
            GameObject.Find("event").GetComponent<Text>().text = "event=OnUserStateChange currentProfile = "
                + profileToString(sdkState.currentProfile) + ", previousProfile = "
                + profileToString(sdkState.previousProfile);
             ;
            gameAppEventHandler.ProcessUserStateChange(sdkState);
        }

        private string profileToString(NetflixSdk.NetflixProfile profile)
        {
            if (profile == null) return "null";
            var builder = new StringBuilder();
            builder.Append(", locale=").Append(profile.locale.language).Append("-").Append(profile.locale.country).Append("-").Append(profile.locale.variant);
            builder.Append(", playerId=").Append(showFirstNChars(profile.playerId, 16));
            builder.Append(", netflixAccessToken=").Append(showFirstNChars(profile.netflixAccessToken, 16));
            builder.Append(" }");
            return builder.ToString();
        }

        private string showFirstNChars(string longString, int numCharsToShow)
        {
            var builder = new StringBuilder();
            var length = longString.Length;
            if (length > numCharsToShow)
            {
                builder.Append(longString.Substring(0, numCharsToShow));
                builder.Append("...(").Append(length).Append(")");
            }
            else
            {
                builder.Append(longString);
            }
            return builder.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Netflix.Event.RegisterEventReceiver(new EventConsumer(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool WasLoggedOut(Netflix.NetflixSdk.NetflixSdkState sdkState)
    {
        return (sdkState != null && sdkState.currentProfile == null && sdkState.previousProfile != null);
    }

    private void ProcessUserStateChange(Netflix.NetflixSdk.NetflixSdkState sdkState)
    {
        if (WasLoggedOut(sdkState))
        {
            Netflix.NetflixSdk.CheckUserAuth();
        }
    }
}
