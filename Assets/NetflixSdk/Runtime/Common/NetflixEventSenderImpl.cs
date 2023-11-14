using System.Collections.Concurrent;
using Netflix;
using UnityEngine;
using UnityEngine.UI;

namespace Netflix
{
    internal class NetflixEventSenderImpl
    {
        static ConcurrentQueue<Netflix.Event.NetflixEvent> eventQueue = new ConcurrentQueue<Netflix.Event.NetflixEvent>();
        private static Netflix.Event.NetflixEventReceiver gameAppEventReceiver;

        // parse and store the event.
        // parsing to be done in non-UI thread.
        public static void AddNetflixEvent(string nfEvent)
        {
            Netflix.Event.NetflixEvent netflixEvent = JsonUtility.FromJson<Netflix.Event.NetflixEvent>(nfEvent);
            eventQueue.Enqueue(netflixEvent);
        }

        // must be called from the main thread to deliver the events.
        public static void DispatchEventsToGameApp()
        {
            while (eventQueue.Count > 0)
            {
                if (!eventQueue.TryDequeue(out Netflix.Event.NetflixEvent netflixEvent))
                {
                    return;
                }

                NfLog.Log("NetflixEventSenderImpl " + netflixEvent.eventId);
#if NFLX_AUTOMATION
                SdkHolder.nfsdk.PublishToEventSink("Unity_" + netflixEvent.eventId, netflixEvent.eventMsg);
#endif
                // don't check null for gameAppEventReceiver. let it crash for now even if it is null.
                switch (netflixEvent.eventId)
                {
                    case Event.ON_NETFLIX_UI_HIDDEN:
                        NfLog.Log("NetflixEventSenderImpl ON_NETFLIX_UI_HIDDEN");
                        gameAppEventReceiver.OnNetflixUIHidden();
                        break;
                    case Event.ON_USER_STATE_CHANGE:
                        NfLog.Log("NetflixEventSenderImpl ON_USER_STATE_CHANGE");
                        NetflixSdk.NetflixSdkState state = JsonUtility.FromJson<NetflixSdk.NetflixSdkState>(netflixEvent.eventMsg);
                        FixEmptyObjects(state);
                        var currentPlayerId = (state.currentProfile != null) ? state.currentProfile.playerId : "null";
                        var previousPlayerId = (state.previousProfile != null) ? state.previousProfile.playerId : "null";
                        NfLog.Log("  currentPlayerId=" + currentPlayerId);
                        NfLog.Log("  previousPlayerId=" + previousPlayerId);
                        UpdateCrashReporterConfig(state);
                        gameAppEventReceiver.OnUserStateChange(state);
                        break;
                    case Event.ON_NETFLIX_UI_VISIBLE:
                        NfLog.Log("NetflixEventSenderImpl ON_NETFLIX_UI_VISIBLE");
                        gameAppEventReceiver.OnNetflixUIVisible();
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        // https://forum.unity.com/threads/json-utility-creates-empty-classes-instead-of-null.471047/
        static void FixEmptyObjects(NetflixSdk.NetflixSdkState state)
        {
            if (state.currentProfile != null  && string.IsNullOrEmpty(state.currentProfile.playerId)) {
                state.currentProfile = null;
            }
            if (state.previousProfile != null && string.IsNullOrEmpty(state.previousProfile.playerId)) {
                state.previousProfile = null;
            }
        }

        private static bool WasLoggedOut(Netflix.NetflixSdk.NetflixSdkState sdkState)
        {
            return (sdkState != null && sdkState.currentProfile == null && sdkState.previousProfile != null);
        }

        private static bool WasLoggedIn(Netflix.NetflixSdk.NetflixSdkState sdkState)
        {
            return (sdkState != null && sdkState.currentProfile != null && sdkState.previousProfile == null);
        }

        static void UpdateCrashReporterConfig(NetflixSdk.NetflixSdkState state)
        {
            if (WasLoggedIn(state) || WasLoggedOut(state))
            {
                Netflix.SdkProvider.GetSdk().UpdateCrashReporterConfig();
            }
        }

        public static void RegisterEventReceiver(Netflix.Event.NetflixEventReceiver eventReceiver)
        {
            gameAppEventReceiver = eventReceiver;
        }
    }
}