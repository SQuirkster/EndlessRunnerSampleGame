using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Netflix;

namespace Netflix
{
    public class Leaderboards
    {
        // public methods

        public static Task<LeaderboardEntriesResult> GetTopEntries(string leaderboardName, int maxEntries)
        {
            return SdkHolder.nfsdk.GetLeaderboardsApi().GetTopEntries(leaderboardName, maxEntries);
        }

        public static Task<LeaderboardEntriesResult> GetMoreEntries(string leaderboardName, int maxEntries,
            string cursor, FetchDirection direction)
        {
            return SdkHolder.nfsdk.GetLeaderboardsApi().GetMoreEntries(leaderboardName, maxEntries, cursor, direction);
        }

        public static Task<LeaderboardEntriesResult> GetEntriesAroundCurrentPlayer(string leaderboardName, int maxEntries)
        {
            return SdkHolder.nfsdk.GetLeaderboardsApi().GetEntriesAroundCurrentPlayer(leaderboardName, maxEntries);
        }

        public static Task<LeaderboardEntryResult> GetCurrentPlayerEntry(string leaderboardName)
        {
            return SdkHolder.nfsdk.GetLeaderboardsApi().GetCurrentPlayerEntry(leaderboardName);
        }

        public static Task<LeaderboardInfoResult> GetLeaderboardInfo(string leaderboardName)
        {
            return SdkHolder.nfsdk.GetLeaderboardsApi().GetLeaderboardInfo(leaderboardName);
        }
    }

    // models

    [Serializable]
    public enum LeaderboardStatus : int
    {
        Ok = Netflix.StatusCode.Ok,

        /**
         * An unexpected error occurred
         */
        ErrorUnknown = Netflix.StatusCode.ErrorUnknown,

        /**
         * Leaderboard with the given name not found
         */
        ErrorUnknownLeaderboard = Netflix.StatusCode.ErrorNotFound,

        /**
         * Error due to network. Caller can retry the operation at a later time
         */
        ErrorNetwork = Netflix.StatusCode.ErrorNetwork,

        /**
         * Error returned when Leaderboard API call is made before platform is initialized
         */
        ErrorPlatformNotInitialized = Netflix.StatusCode.ErrorPlatformNotInitialized,

        /**
         * Error returned when Leaderboard API call is made without a user profile selected
         */
        ErrorUserProfileNotSelected = Netflix.StatusCode.ErrorUserProfileNotSelected,

        /**
         * Error returned when an operation is interrupted by a profile switch. Caller
         * can retry the operation upon returning to the corresponding profile
         */
        ErrorInterruptedByProfileSwitch = Netflix.StatusCode.ErrorInterruptedByProfileSwitch,

        /**
         * Error due to IO operations
         */
        ErrorIO = Netflix.StatusCode.ErrorIO,

        /**
         * Error due to client-server interactions
         */
        ErrorInternal = Netflix.StatusCode.ErrorInternal,

        /**
         * Error due to Leaderboard name failing string validation checks
         */
        ErrorValidation = Netflix.StatusCode.ErrorValidation,

        ErrorTimedOut = Netflix.StatusCode.ErrorTimedOut,

        /**
         * Leaderboard player entry not found
         */
        ErrorEntryNotFound = Netflix.StatusCode.ErrorCustomStart
    }

    [Serializable]
    public enum FetchDirection
    {
        Before = 0,
        After = 1
    }

    public class LeaderboardEntry
    {
        public int position;
        public int rank;
        public long score;
        public PlayerIdentity playerIdentity;

        internal LeaderboardEntry(SerializableLeaderboardsModels.LeaderboardEntry entry)
        {
            position = entry.position;
            rank = entry.rank;
            score = entry.score;
            var handle = entry.playerIdentity.handle != null && entry.playerIdentity.handle != "" ? entry.playerIdentity.handle : null;
            playerIdentity = new PlayerIdentity(entry.playerIdentity.playerId, handle);
        }
    }

    public class LeaderboardEntryPage
    {
        public readonly string startCursor;
        public readonly string endCursor;
        public readonly bool hasMoreBeforeStart;
        public readonly bool hasMoreAfterEnd;
        public readonly List<LeaderboardEntry> leaderboardEntries;

        internal LeaderboardEntryPage(SerializableLeaderboardsModels.LeaderboardEntryPage page)
        {
            startCursor = page.startCursor;
            endCursor = page.endCursor;
            hasMoreBeforeStart = page.hasMoreBeforeStart;
            hasMoreAfterEnd = page.hasMoreAfterEnd;
            if (page.leaderboardEntries != null)
            {
                leaderboardEntries = page.leaderboardEntries.ConvertAll(entry => new LeaderboardEntry(entry));
            }
            else
            {
                leaderboardEntries = new List<LeaderboardEntry>();
            }
        }
    }

    public class LeaderboardEntriesResult
    {
        public readonly LeaderboardStatus status;
        public readonly LeaderboardEntryPage page;

        internal LeaderboardEntriesResult(LeaderboardStatus status, SerializableLeaderboardsModels.LeaderboardEntryPage page)
        {
            this.status = status;
            if (page != null && page.startCursor != null && page.endCursor != null && page.leaderboardEntries != null)
            {
                this.page = new LeaderboardEntryPage(page);
            }
        }
    }

    public class LeaderboardEntryResult
    {
        public readonly LeaderboardStatus status;
        public readonly LeaderboardEntry entry;

        internal LeaderboardEntryResult(SerializableLeaderboardsModels.LeaderboardEntryResult result)
        {
            this.status = result.status;

            if (result.entry != null && result.entry.playerIdentity != null)
            {
                this.entry = new LeaderboardEntry(result.entry);
            }
        }
    }

    public class LeaderboardInfoResult
    {
        public readonly LeaderboardStatus status;
        public readonly LeaderboardInfo leaderboardInfo;

        internal LeaderboardInfoResult(SerializableLeaderboardsModels.LeaderboardInfoResult result)
        {
            this.status = result.status;
            if (result.leaderboardInfo != null && result.leaderboardInfo.name != null)
            {
                this.leaderboardInfo = new LeaderboardInfo(result.leaderboardInfo);
            }
        }
    }

    public class LeaderboardInfo
    {
        public readonly int count;
        public readonly string name;

        internal LeaderboardInfo(SerializableLeaderboardsModels.LeaderboardInfo info)
        {
            this.count = info.count;
            this.name = info.name;
        }
    }
}