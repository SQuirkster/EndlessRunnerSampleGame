using System;
using System.Collections.Generic;

namespace Netflix
{
    namespace SerializableLeaderboardsModels
    {
        [Serializable]
        public class PageInfo
        {
            public bool hasNextPage;
            public bool hasPrevPage;
            public string startCursor;
            public string endCursor;
        }

        [Serializable]
        public class LeaderboardEntry
        {
            public int position;
            public int rank;
            public long score;
            public LeaderboardPlayerIdentity playerIdentity;
        }


        [Serializable]
        public class LeaderboardPlayerIdentity
        {
            public string playerId;
            public string handle;
        }

        [Serializable]
        public class LeaderboardEntryPage
        {
            public string startCursor;
            public string endCursor;
            public bool hasMoreBeforeStart;
            public bool hasMoreAfterEnd;
            public List<LeaderboardEntry> leaderboardEntries;
        }


        [Serializable]
        public class LeaderboardEntriesResult
        {
            public LeaderboardStatus status;
            public LeaderboardEntryPage page;
        }

        [Serializable]
        public class LeaderboardEntryResult
        {
            public LeaderboardStatus status;
            public LeaderboardEntry entry;
        }


        [Serializable]
        public class LeaderboardInfoResult
        {
            public LeaderboardStatus status;
            public LeaderboardInfo leaderboardInfo;
        }

        [Serializable]
        public class LeaderboardInfo
        {
            public int count;
            public string name;
        }
    }
}
