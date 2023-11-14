using System.Threading.Tasks;

namespace Netflix
{
    internal sealed class FakeLeaderboardsApi : SdkApi.ILeaderboards
    {
        public Task<LeaderboardEntryResult> GetCurrentPlayerEntry(string leaderboardName)
        {
            NfLog.Log("GetCurrentPlayerEntry");
            var task = Task<LeaderboardEntryResult>.Factory.StartNew(() => new LeaderboardEntryResult(new SerializableLeaderboardsModels.LeaderboardEntryResult
            {
                status = LeaderboardStatus.ErrorEntryNotFound
            }));
            return task;
        }

        public Task<LeaderboardEntriesResult> GetEntriesAroundCurrentPlayer(string leaderboardName, int maxEntries)
        {
            NfLog.Log("GetEntriesAroundCurrentPlayer");
            var task = Task<LeaderboardEntriesResult>.Factory.StartNew(() =>
                new LeaderboardEntriesResult(LeaderboardStatus.ErrorEntryNotFound, null));
            return task;
        }

        public Task<LeaderboardInfoResult> GetLeaderboardInfo(string leaderboardName)
        {
            NfLog.Log("GetLeaderboardInfo");
            var task = Task<LeaderboardInfoResult>.Factory.StartNew(() =>
                new LeaderboardInfoResult(new SerializableLeaderboardsModels.LeaderboardInfoResult
                {
                    status = LeaderboardStatus.ErrorEntryNotFound
                }));
            return task;
        }

        public Task<LeaderboardEntriesResult> GetMoreEntries(string leaderboardName, int maxEntries, string cursor, FetchDirection direction)
        {
            NfLog.Log("GetMoreEntries");
            var task = Task<LeaderboardEntriesResult>.Factory.StartNew(() =>
                new LeaderboardEntriesResult(LeaderboardStatus.ErrorEntryNotFound, null));
            return task;
        }

        public Task<LeaderboardEntriesResult> GetTopEntries(string leaderboardName, int count)
        {
            NfLog.Log("GetTopEntries");
            var task = Task<LeaderboardEntriesResult>.Factory.StartNew(() =>
                new LeaderboardEntriesResult(LeaderboardStatus.ErrorEntryNotFound, null));
            return task;
        }
    }
}