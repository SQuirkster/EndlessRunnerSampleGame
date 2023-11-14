#if UNITY_ANDROID
namespace Netflix
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    internal class NetflixJavaConverter
    {
        internal static SdkApi.CrashReporterConfig ToCrashReporterConfig(AndroidJavaObject netflixCrashConfig)
        {
            SdkApi.CrashReporterConfig crashReporterConfig = new SdkApi.CrashReporterConfig()
            {
                ID = netflixCrashConfig.Call<String>("getProjectId"),
                guid = netflixCrashConfig.Call<String>("getGuid"),
            };
            return crashReporterConfig;
        }

        internal static GetPlayerIdentitiesResult ToGetPlayerIdentitiesResult(AndroidJavaObject androidResult)
        {
            var topLevelError = (RequestStatus) androidResult.Get<int>("status");
            var description = androidResult.Get<string>("description");
            var playerIdentities = androidResult.Get<AndroidJavaObject>("playerIdentities");
            NfLog.Log("status=" + topLevelError + ", description=" + description);

            Dictionary<string, PlayerIdentityResult> playerIdentityResult = null;
            if (playerIdentities != null)
            {
                int size = playerIdentities.Call<int>("size");
                AndroidJavaObject playerResult;
                playerIdentityResult = new Dictionary<string, PlayerIdentityResult>(size);
                for (int i = 0; i < size; ++i)
                {
                    playerResult = playerIdentities.Call<AndroidJavaObject>("get", i);
                    var playerStatus = (PlayerIdentityStatus) playerResult.Get<int>("status");
                    var playerId = playerResult.Get<string>("playerId");
                    var handle = playerResult.Get<string>("handle");
                    NfLog.Log("[" + i + "] status=" + playerStatus + " ,playerId=" + playerId + ", handle=" + handle);

                    if (playerStatus == PlayerIdentityStatus.Ok)
                    {
                        playerIdentityResult.Add(playerId, new PlayerIdentityResult(playerStatus, new PlayerIdentity(playerId, handle)));
                    }
                    else
                    {
                        playerIdentityResult.Add(playerId, new PlayerIdentityResult(playerStatus, null));
                    }
                }
            }
            return new GetPlayerIdentitiesResult(topLevelError, description, playerIdentityResult);
        }

        internal static LeaderboardEntriesResult ToLeaderboardEntriesResult(AndroidJavaObject androidResult)
        {
            if (androidResult == null) {
                return new LeaderboardEntriesResult(LeaderboardStatus.ErrorUnknown, null);
            }

            var status = (LeaderboardStatus) androidResult.Get<int>("status");
            NfLog.Log("ToLeaderboardEntriesResult status: " +status);
            if (status != LeaderboardStatus.Ok) {
                return new LeaderboardEntriesResult(status, null);
            }

            SerializableLeaderboardsModels.LeaderboardEntryPage page = new SerializableLeaderboardsModels.LeaderboardEntryPage();
            page.startCursor =  androidResult.Get<string>("startCursor");
            page.endCursor = androidResult.Get<string>("endCursor");
            page.hasMoreAfterEnd = androidResult.Get<bool>("hasMoreAfterEnd");
            page.hasMoreBeforeStart = androidResult.Get<bool>("hasMoreBeforeStart");

            var leaderboardEntries = new List<SerializableLeaderboardsModels.LeaderboardEntry>();
            var entries = androidResult.Get<AndroidJavaObject>("entries");
            if (entries != null) {

                int size = entries.Call<int>("size");
                AndroidJavaObject entryObj;

                for (int i = 0; i < size; ++i) {
                    NfLog.Log("[" + i + "]");
                    entryObj = entries.Call<AndroidJavaObject>("get", i);
                    if (entryObj != null) {
                        leaderboardEntries.Add(ToLeaderboardEntry(entryObj));
                    }
                }
            }
            page.leaderboardEntries = leaderboardEntries;
            return new LeaderboardEntriesResult(status, page);
        }

        internal static LeaderboardEntryResult ToLeaderboardEntryResult(AndroidJavaObject androidResult)
        {
            SerializableLeaderboardsModels.LeaderboardEntryResult entryResult = new SerializableLeaderboardsModels.LeaderboardEntryResult();
            if (androidResult == null) {
                entryResult.status = LeaderboardStatus.ErrorUnknown;
                return new LeaderboardEntryResult(entryResult);
            }

            var status = (LeaderboardStatus) androidResult.Get<int>("status");
            entryResult.status = status;
            if (status == LeaderboardStatus.Ok) {
                entryResult.entry = ToLeaderboardEntry(androidResult);
            }

            return new LeaderboardEntryResult(entryResult);
        }

        internal static SerializableLeaderboardsModels.LeaderboardEntry ToLeaderboardEntry(AndroidJavaObject androidResult)
        {
            SerializableLeaderboardsModels.LeaderboardEntry leaderboardEntry = new SerializableLeaderboardsModels.LeaderboardEntry();
            if (androidResult == null) {
                return leaderboardEntry;
            }

            var position = androidResult.Get<int>("position");
            var rank = androidResult.Get<int>("rank");
            var score = androidResult.Get<long>("score");
            var playerId = androidResult.Get<string>("playerId");
            var handle = androidResult.Get<string>("handle");

            NfLog.Log("rank= " + rank + ",(" + position + "), score:" + score +", playerId=" + playerId + ", handle=" + handle);

            SerializableLeaderboardsModels.LeaderboardPlayerIdentity playerIdentity = new SerializableLeaderboardsModels.LeaderboardPlayerIdentity();
            playerIdentity.playerId = playerId;
            playerIdentity.handle = handle;
            leaderboardEntry.position = position;
            leaderboardEntry.rank = rank;
            leaderboardEntry.score = score;
            leaderboardEntry.playerIdentity = playerIdentity;

            return leaderboardEntry;
        }

        internal static LeaderboardInfoResult ToLeaderboardInfoResult(AndroidJavaObject androidResult)
        {
            SerializableLeaderboardsModels.LeaderboardInfoResult infoResult = new SerializableLeaderboardsModels.LeaderboardInfoResult();
            if (androidResult == null) {
                infoResult.status = LeaderboardStatus.ErrorUnknown;
                return new LeaderboardInfoResult(infoResult);
            }

            var status = (LeaderboardStatus) androidResult.Get<int>("status");
            infoResult.status = status;
            if (status != LeaderboardStatus.Ok) {
                return new LeaderboardInfoResult(infoResult);
            }

            var count = androidResult.Get<int>("count");
            var name = androidResult.Get<string>("name");

            NfLog.Log("count= " + count + ", name:" + name);

            SerializableLeaderboardsModels.LeaderboardInfo leaderboardInfo = new SerializableLeaderboardsModels.LeaderboardInfo();
            leaderboardInfo.count = count;
            leaderboardInfo.name = name;

            infoResult.leaderboardInfo = leaderboardInfo;
            return new LeaderboardInfoResult(infoResult);
        }

        internal static Stats.SubmitStatResult ToSubmitStatResult(AndroidJavaObject androidResult)
        {

            var submitStatResult = new Stats.SubmitStatResult();
            if (androidResult == null) {
                submitStatResult.status = Stats.StatsStatus.ErrorUnknown;
                return submitStatResult;
            }

            var status = (Stats.StatsStatus) androidResult.Get<int>("status");
            submitStatResult.status = status;

            var name = androidResult.Get<string>("statName");
            var submittedValue = androidResult.Get<long>("submittedStatValue");
            var aggregatedValue = androidResult.Get<long>("aggregatedStatValue");

            NfLog.Log(" ToSubmitStatResult:  name:" + name + ", submittedValue:"
                + submittedValue + ", aggregatedValue:" + aggregatedValue);

            submitStatResult.submittedStat = new Stats.StatItem(name, submittedValue);
            if (status == Stats.StatsStatus.Ok)
            {
                submitStatResult.aggregatedStat = new Stats.AggregatedStat(name, aggregatedValue);
            }
            return submitStatResult;
        }


        internal static Stats.AggregatedStatResult ToAggregatedStatResult(AndroidJavaObject androidResult)
        {
            var aggregatedStatResult = new Stats.AggregatedStatResult();
            if (androidResult == null)
            {
                aggregatedStatResult.status = Stats.StatsStatus.ErrorUnknown;
                return aggregatedStatResult;
            }

            var status = (Stats.StatsStatus)androidResult.Get<int>("status");
            aggregatedStatResult.status = status;

            var name = androidResult.Get<string>("statName");
            var aggregatedValue = androidResult.Get<long>("aggregatedStatValue");

            NfLog.Log(" ToAggregatedStatResult: status:" + aggregatedStatResult.status.ToString()
                + "name:" + name + ", aggregatedValue:" + aggregatedValue);

            if (status == Stats.StatsStatus.Ok)
            {
                aggregatedStatResult.aggregatedStat = new Stats.AggregatedStat(name, aggregatedValue);
            }
            return aggregatedStatResult;
        }

        internal static Achievement ToAchievementEntry(AndroidJavaObject androidResult)
        {
            if (androidResult == null) {
                return null;
            }

            var achievementName = androidResult.Get<string>("name");
            var isLocked = androidResult.Get<bool>("isLocked");

            NfLog.Log("ToAchievementEntry name: " + achievementName + ", isLocked: " + isLocked);

            return new Achievement(achievementName, isLocked);
        }
    }
}
#endif
