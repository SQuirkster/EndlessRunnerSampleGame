using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backtrace.Unity;
using Backtrace.Unity.Model;
using UnityEngine;

namespace Netflix {
    internal class NetflixSdkImpl : SdkApi
    {
        private static readonly object ApiInitializationLock = new object();
        protected SdkApi.CrashReporterConfig crashReporterConfig;
        private SdkApi.ILeaderboards leaderboardsApi;
        private SdkApi.IStats statsApi;
        private SdkApi.IPlayerIdentity playerIdentityApi;
        private SdkApi.ICloudSave cloudSaveApi;
        private SdkApi.IMessaging messagingApi;
        private SdkApi.IAchievements achievementsApi;
        private SdkApi.ISecondScreenInputController secondScreenInputControllerApi;
        private static NetflixSdk.NetflixProfile lastSentProfile;
        internal static NetflixSdk.NetflixProfile editorCurrentProfile = new NetflixSdk.NetflixProfile
        {
            playerId = "PLAYER_ID",
            gamerProfileId = "GAMER_PROFILE_ID",
            netflixAccessToken = "ACCESS_TOKEN",
            locale = new NetflixSdk.Locale
            {
                language = "en",
                country = "US"
            }
        };

        public virtual void CheckUserAuth()
        {
            NfLog.Log("NetflixSdkImpl FakeCheckUserAuth");
            NetflixSdk.NetflixProfile profile = editorCurrentProfile;

            var netflixSdkState = new NetflixSdk.NetflixSdkState
            {
                currentProfile = profile,
                previousProfile = lastSentProfile,
            };

            var netflixEvent = new Event.NetflixEvent
            {
                eventId = Event.ON_USER_STATE_CHANGE,
                eventMsg = JsonUtility.ToJson(netflixSdkState),
            };

            lastSentProfile = editorCurrentProfile;

            NetflixEventSenderImpl.AddNetflixEvent(JsonUtility.ToJson(netflixEvent));
        }

        public virtual void HideNetflixMenu()
        {
            NfLog.Log("NetflixSdkDummyImpl HideNetflixMenu.");
        }

        public virtual void ShowNetflixMenu(int location)
        {
            NfLog.Log("NetflixSdkDummyImpl ShowNetflixMenu.");
        }

        public virtual void ShowNetflixAccessButton()
        {
            NfLog.Log("NetflixSdkDummyImpl ShowNetflixAccessButton.");
        }

        public virtual void HideNetflixAccessButton()
        {
            NfLog.Log("NetflixSdkDummyImpl HideNetflixAccessButton.");
        }

        public virtual void SetLocale(NetflixSdk.Locale locale)
        {
            NfLog.Log("NetflixSdkDummyImpl SetLocale.");
        }

        public virtual SdkApi.CrashReporterConfig GetCrashReporterConfig()
        {
            return new SdkApi.CrashReporterConfig
            {
                ID = "projectId",
                guid = "guid",
            };
        }
        public virtual void LogHandledException(Exception exception)
        {
            backtraceClient.Send(exception);
            NfLog.Log("Crash Reporter LogHandledException: " + exception);
        }

        public virtual void LeaveBreadcrumb(string message)
        {
            backtraceClient.Breadcrumbs.Info(message);
            NfLog.Log("Crash Reporter LeaveBreadcrumb:" + message);
        }

        protected static BacktraceClient backtraceClient;
        public virtual void SetupCrashReporter()
        {
            UpdateCrashReporterConfig();
            var attributes = new Dictionary<String, String>();
            attributes.Add("CaptureNativeCrashes", "true");
            attributes.Add("nativeSDKVersion", GetNativeSdkVersion());

            var configuration = ScriptableObject.CreateInstance<BacktraceConfiguration>();
            configuration.ServerUrl = "https://submit.backtrace.io/netflix/" + GetCrashReporterConfig().ID + "/json";
            configuration.Enabled = true;
            configuration.DatabasePath = "${Application.persistentDataPath}/backtrace";
            configuration.CreateDatabase = true;
            configuration.EnableBreadcrumbsSupport = true;

            backtraceClient = BacktraceClient.Initialize(configuration, attributes);
        }

        public virtual void UpdateCrashReporterConfig()
        {
            crashReporterConfig = GetCrashReporterConfig();
            NfLog.Log("UpdateCrashReporterConfig guid: " + crashReporterConfig.guid);
        }
        
        [Obsolete("SendCLEvent is deprecated, please use LogInGameEvent instead.")]
        public virtual void SendCLEvent(string clTypeName, string eventDataJson)
        {
            NfLog.Log("NetflixSdkDummyImpl SendCLEvent: " + clTypeName + " " + eventDataJson);
        }

        public virtual void LogInGameEvent(InGameEvent inGameEvent)
        {
            NfLog.Log("NetflixSdkDummyImpl LogInGameEvent: " + inGameEvent.name + " " + inGameEvent.ToJson());
        }

        public bool IsFeatureSupported(Feature feature)
        {
            return feature switch
            {
                Feature.Leaderboards => !(GetLeaderboardsApi() is FakeLeaderboardsApi),
                Feature.Messaging => !(GetMessagingApi() is FakeMessagingApi),
                Feature.Stats => !(GetStatsApi() is FakeStatsApi),
                Feature.PlayerIdentity => !(GetPlayerIdentityApi() is FakePlayerIdentityApi),
                Feature.SecondScreenInputController =>
                    !(GetSecondScreenInputControllerApi() is FakeSecondScreenInputController),
                Feature.Achievements => !(GetAchievementsApi() is FakeAchievementsApi),
                Feature.CloudSave => !(GetCloudSaveApi() is FakeCloudSaveApi),
                _ => false
            };
        }

        protected virtual string GetUnitySdkVersion()
        {
            return "unversioned";
        }

        public virtual string GetNativeSdkVersion()
        {
            return "unversioned";
        }

        public SdkApi.ILeaderboards GetLeaderboardsApi()
        {
            if (leaderboardsApi != null) return leaderboardsApi;
            lock (ApiInitializationLock)
            {
                leaderboardsApi = CreateLeaderboardsApi();
            }
            return leaderboardsApi;
        }

        public SdkApi.IStats GetStatsApi()
        {
            if (statsApi != null) return statsApi;
            lock (ApiInitializationLock)
            {
                statsApi = CreateStatsApi();
            }
            return statsApi;
        }

        public SdkApi.IPlayerIdentity GetPlayerIdentityApi()
        {
            if (playerIdentityApi != null) return playerIdentityApi;
            lock (ApiInitializationLock)
            {
                playerIdentityApi = CreatePlayerIdentityApi();
            }
            return playerIdentityApi;
        }
        
        public SdkApi.ICloudSave GetCloudSaveApi()
        {
            if (cloudSaveApi != null) return cloudSaveApi;
            lock (ApiInitializationLock)
            {
                cloudSaveApi = CreateCloudSaveApi();
            }
            return cloudSaveApi;
        }
        
        public SdkApi.IMessaging GetMessagingApi()
        {
            if (messagingApi != null) return messagingApi;
            lock (ApiInitializationLock)
            {
                messagingApi = CreateMessagingApi();
            }
            return messagingApi;
        }

        public SdkApi.IAchievements GetAchievementsApi()
        {
            if (achievementsApi != null) return achievementsApi;
            lock (ApiInitializationLock)
            {
                achievementsApi = CreateAchievementsApi();
            }
            return achievementsApi;
        }
        
        protected virtual SdkApi.ILeaderboards CreateLeaderboardsApi()
        {
            return new FakeLeaderboardsApi();
        }

        protected virtual SdkApi.IStats CreateStatsApi()
        {
            return new FakeStatsApi();
        }
        
        protected virtual SdkApi.IPlayerIdentity CreatePlayerIdentityApi()
        {
            return new FakePlayerIdentityApi();
        }

        protected virtual SdkApi.IAchievements CreateAchievementsApi()
        {
            return new FakeAchievementsApi();
        }

        protected virtual SdkApi.ISecondScreenInputController CreateSecondScreenInputControllerApi()
        {
            return new FakeSecondScreenInputController();
        }
        
        protected virtual SdkApi.ICloudSave CreateCloudSaveApi()
        {
            return new FakeCloudSaveApi();
        }
        
        protected virtual SdkApi.IMessaging CreateMessagingApi()
        {
            return new FakeMessagingApi();
        }

        public virtual SdkApi.ISecondScreenInputController GetSecondScreenInputControllerApi()
        {
            if (secondScreenInputControllerApi != null) return secondScreenInputControllerApi;
            lock (ApiInitializationLock)
            {
                secondScreenInputControllerApi = CreateSecondScreenInputControllerApi();
            }
            return secondScreenInputControllerApi;
        }
        
        public virtual Dictionary<string, string> GetTestParams()
        {
            NfLog.Log("GetTestParams");
            return new Dictionary<string, string>();
        }

        public virtual void PublishToEventSink(string name, string data)
        {
            NfLog.Log("PublishToEventSink");
        }
    }
}