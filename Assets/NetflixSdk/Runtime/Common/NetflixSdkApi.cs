using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Netflix.Stats;

namespace Netflix
{
    public enum Feature {
        PlayerIdentity,
        Messaging,
        CloudSave,
        Stats,
        Leaderboards,
        SecondScreenInputController,
        Achievements
    }

    internal interface SdkApi
    {
        [Serializable]
        class CrashReporterConfig {
            public string ID;
            public string guid;
        }

        void CheckUserAuth();

        void ShowNetflixMenu(int location);

        void HideNetflixMenu();

        void ShowNetflixAccessButton();

        void HideNetflixAccessButton();

        void SetLocale(NetflixSdk.Locale locale);

        void SetupCrashReporter();

        CrashReporterConfig GetCrashReporterConfig();

        void LogHandledException(Exception exception);

        void LeaveBreadcrumb(string message);

        void UpdateCrashReporterConfig();

        internal interface IMessaging
        {
            void OnPushToken(string pushDeviceToken);

            void OnMessagingEvent(Netflix.NetflixMessaging.MessagingEventType eventType, string jsonString);

            void OnDeeplinkReceived(string deepLinkURL, bool processedByGame);
        }

        void SendCLEvent(string clTypeName, string eventDataJson);

        void LogInGameEvent(InGameEvent inGameEvent);

        bool IsFeatureSupported(Feature feature);

        string GetNativeSdkVersion();

        ILeaderboards GetLeaderboardsApi();

        IStats GetStatsApi();

        IAchievements GetAchievementsApi();
        
        IPlayerIdentity GetPlayerIdentityApi();

        ICloudSave GetCloudSaveApi();
        IMessaging GetMessagingApi();
        
        ISecondScreenInputController GetSecondScreenInputControllerApi();

        internal interface ICloudSave
        {
            Task<NetflixCloudSave.GetSlotIdsResult> GetSlotIds();

            Task<NetflixCloudSave.SaveSlotResult> SaveSlot(string slotId, NetflixCloudSave.SlotInfo slotInfo);

            Task<NetflixCloudSave.ReadSlotResult> ReadSlot(string slotId);

            Task<NetflixCloudSave.DeleteSlotResult> DeleteSlot(string slotId);

            Task<NetflixCloudSave.ResolveConflictResult> ResolveConflict(string slotId,
                NetflixCloudSave.CloudSaveResolution resolution);
        }

        Dictionary<string, string> GetTestParams();
        
        internal interface IPlayerIdentity
        {
            PlayerIdentity GetCurrentPlayer();

            Task<GetPlayerIdentitiesResult> GetPlayerIdentities(List<string> playerIds);
        }

        internal interface ILeaderboards
        {

            Task<LeaderboardEntriesResult> GetTopEntries(string leaderboardName, int count);

            Task<LeaderboardEntriesResult> GetMoreEntries(string leaderboardName, int maxEntries,
            string cursor, FetchDirection direction);

            Task<LeaderboardEntriesResult> GetEntriesAroundCurrentPlayer(string leaderboardName, int maxEntries);

            Task<LeaderboardEntryResult> GetCurrentPlayerEntry(string leaderboardName);

            Task<LeaderboardInfoResult> GetLeaderboardInfo(string leaderboardName);
        }

        internal interface IStats
        {
            Task<SubmitStatResult> SubmitStatNow(StatItem statItem);

            void SubmitStat(StatItem statItem);

            Task<AggregatedStatResult> GetAggregatedStat(String statName);
        }

        internal interface IAchievements
        {
            Task<UnlockAchievementResult> UnlockAchievement(string achievementName);

            Task<AchievementsResult> GetAchievements();

            void ShowAchievementsPanel();
        }

        internal interface ISecondScreenInputController
        {
            void SetActiveLayout(string layoutName);
        }

        void PublishToEventSink(string name, string data);
    }
}