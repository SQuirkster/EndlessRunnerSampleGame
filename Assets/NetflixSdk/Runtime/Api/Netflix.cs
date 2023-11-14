using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Serialization;


/*
 This is the top level interface to be used by the Game app.

 Few examples:
 1) How to login or do periodic auth check?
 Netflix.NetflixSdk.CheckUserAuth()

 2) How to request the Netflix SDK to show the Netflix menu?
  Netflix.NetflixSdk.ShowNetflixMenu(Netflix.NetflixSdk.TOP_LEFT)
 */

// NOTE: API is not finalized and will likely change.
namespace Netflix
{
    public class NetflixSdk
    {
        /*
          Constants for the location of Netflix menu when calling ShowNetflixMenu API.
          e.g. to show the Netflix Menu at the top left of the screen call:
          ShowNetflixMenu(TOP_LEFT);
        */
        [Obsolete("TOP_LEFT is now deprecated.")]
        public const int TOP_LEFT = 1;
        [Obsolete("TOP_RIGHT is now deprecated.")]
        public const int TOP_RIGHT = 2;
        [Obsolete("BOTTOM_LEFT is now deprecated.")]
        public const int BOTTOM_LEFT = 3;
        [Obsolete("BOTTOM_RIGHT is now deprecated.")]
        public const int BOTTOM_RIGHT = 4;
        [Obsolete("CENTER_RIGHT is now deprecated.")]
        public const int CENTER_RIGHT = 5;

        public static bool IsInitialized => SdkProvider.IsInitialized();

        public static void CheckUserAuth()
        {

            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.CheckUserAuth();
        }

        [Obsolete("ShowNetflixMenu is now deprecated. Please use ShowNetflixAccessButton.")]
        public static void ShowNetflixMenu(int location)
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.ShowNetflixMenu(location);
        }

        [Obsolete("HideNetflixMenu is now deprecated. Please use HideNetflixAccessButton.")]
        public static void HideNetflixMenu()
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.HideNetflixMenu();
        }

        public static void ShowNetflixAccessButton()
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.ShowNetflixAccessButton();
        }

        public static void HideNetflixAccessButton()
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.HideNetflixAccessButton();
        }

        public static void SetLocale(Locale locale)
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.SetLocale(locale);
        }

        // LogHandledException can be called on any thread
        public static void LogHandledException(Exception exception)
        {
            SdkHolder.nfsdk.LogHandledException(exception);
        }

        // LeaveBreadcrumb can be called on any thread
        public static void LeaveBreadcrumb(String message)
        {
            SdkHolder.nfsdk.LeaveBreadcrumb(message);
        }

        // Send CL event for custom in-game event.
        [Obsolete("SendCLEvent is now deprecated. Please use LogInGameEvent instead.")]
        public static void SendCLEvent(string clTypeName, string eventDataJson)
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.SendCLEvent(clTypeName, eventDataJson);
        }

        public static void LogInGameEvent(InGameEvent inGameEvent)
        {
            SdkHolder.AssertMainThread();
            SdkHolder.nfsdk.SendCLEvent(inGameEvent.name, inGameEvent.ToJson());
        }

        public static string GetSdkVersion()
        {
            return SdkHolder.nfsdk.GetNativeSdkVersion();
        }

        public static Stats GetNetflixStats()
        {
            return new Stats();
        }

        [Serializable]
        public class NetflixSdkState
        {
            //  currentProfile will be null when the user is logged-out.
            public NetflixProfile currentProfile;
            public NetflixProfile previousProfile;
        }

        [Serializable]
        public class NetflixProfile
        {
            /**
             * gamerProfileId is a unique identifier for the netflix profile per game.
             * It will remain the same independent of the device the netflix profile is logged-in.
             */
            [Obsolete(
                "gamerProfileId is now deprecated." +
                "If an existing game is already using gamerProfileId as a primary key to local or cloud storage, " +
                "developers can continue using this field. Although this property is deprecated, Netflix is " +
                "committed to supporting this property for the lifetime of the game where it is already in use.  " +
                "If a game is currently not using the gamerProfileId property, and if the use case arises, please " +
                "use playerId as your primary key instead of gamerProfileId."
            )]
            public string gamerProfileId;

            /**
             * loggingId is a unique identifier for each netflix profile. Any logs sent to the logging backend must include this.
             */
            [Obsolete(
                "loggingId is now deprecated. Please migrate to using playerId instead. " +
                  "Assuming that no data, other than logs, should be associated with the loggingId, " +
                  "any references to this field can be safely replaced with playerId. This field " +
                  "will be removed from the Netflix SDK in a future release."
            )]
            public string loggingId;

            /**
             * netflixAccessToken: access token that allows game-backend-servers to make calls to the Netflix-backend-servers on behalf of the netflix profile.
             */
            public string netflixAccessToken;

            /**
             * Profile locale.
             */
            public Locale locale;

            /**
             * The unique player ID. This ID is not intended for display purposes but for using as a
             * primary key for any player data lookup or storage, and as the player identifier for logging
             * use cases. This field returns the same value as the playerId field in the object returned by
             * the NetflixPlayerIdentity.getCurrentPlayer() API.
             */
            public string playerId;
        }

        // Locale per IETF BCP 47
        [Serializable]
        public class Locale
        {
            public string language;
            public string country;
            public string variant;
        }
        
#if UNITY_EDITOR
        public static void SetEditorCurrentProfile(NetflixProfile profile)
        {
            NetflixSdkImpl.editorCurrentProfile = profile;
        }
#endif
        
        #if NFLX_AUTOMATION
        public static Dictionary<string, string> GetTestParams()
        {
            return SdkHolder.nfsdk.GetTestParams();
        }
        #endif
    }

    public class Event
    {
        public const string ON_USER_STATE_CHANGE = "onUserStateChange";
        public const string ON_NETFLIX_UI_VISIBLE = "onNetflixUiVisible";
        public const string ON_NETFLIX_UI_HIDDEN = "onNetflixUiHidden";

        [Serializable]
        public class NetflixEvent
        {
            [FormerlySerializedAs("eventName")]
            public String eventId;
            [FormerlySerializedAs("eventMsg")]
            public String eventMsg;
        }

        public interface NetflixEventReceiver
        {
            void OnNetflixUIVisible();
            void OnNetflixUIHidden();
            void OnUserStateChange(NetflixSdk.NetflixSdkState sdkState);
        }

        public static void RegisterEventReceiver(NetflixEventReceiver eventReceiver)
        {
            NetflixEventSenderImpl.RegisterEventReceiver(eventReceiver);
        }
    }
    
    internal class SdkHolder
    {
        private static SdkApi nfsdkImpl;
        public static SdkApi nfsdk
        {
            get
            {
                if (nfsdkImpl == null)
                {
                    nfsdkImpl = SdkProvider.GetSdk();
                }
                return nfsdkImpl;
            }
        }

        internal static void AssertMainThread()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (SdkProvider.GetMainThread() == null)
            {
                throw new Exception("Looks like Netflix SdkProvider is not initialized. This may happen if NetflixGameObject is not added to the scene.");
            }
            if (!Thread.CurrentThread.Equals(SdkProvider.GetMainThread()))
            {
                throw new Exception("NetflixSdk APIs can only be called on the main thread.");
            }
#endif
        }
    }

}
