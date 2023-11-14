#if UNITY_IOS

using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EditModeTests")]
namespace Netflix
{
    internal class NGPiOSLeaderboards : SdkApi.ILeaderboards
    {

        [DllImport("__Internal")]
        private static extern void ngp_get_top_leaderboard_entries(int correlation_id, string leaderboard_name, int maxEntries, NGPLeaderboardCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_get_more_leaderboard_entries(int correlation_id, string leaderboard_name, int maxEntries, string cursor, Netflix.FetchDirection direction, NGPLeaderboardCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_get_entries_around_current_player(int correlation_id, string leaderboard_name, int maxEntries, NGPLeaderboardCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_get_current_player_entry(int correlation_id, string leaderboard_name, NGPLeaderboardCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_get_leaderboard_info(int correlation_id, string leaderboard_name, NGPLeaderboardCallbackFuncType callback);

        public Task<LeaderboardEntriesResult> GetTopEntries(string leaderboardName, int maxEntries)
        {
            TaskCompletionSource<LeaderboardEntriesResult> completionSource = new TaskCompletionSource<LeaderboardEntriesResult>();
            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " leaderboardName " + leaderboardName + " maxEntries " + maxEntries);

            ngp_get_top_leaderboard_entries(correlationId, leaderboardName, maxEntries, NGPLeaderboardGetEntriesDispatchResult);

            return completionSource.Task;
        }

        public Task<LeaderboardEntriesResult> GetMoreEntries(string leaderboardName, int maxEntries, string cursor, FetchDirection direction)
        {
            TaskCompletionSource<LeaderboardEntriesResult> completionSource = new TaskCompletionSource<LeaderboardEntriesResult>();
            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " leaderboardName " + leaderboardName + " maxEntries " + maxEntries + " cursor " + cursor);

            ngp_get_more_leaderboard_entries(correlationId, leaderboardName, maxEntries, cursor, direction, NGPLeaderboardGetEntriesDispatchResult);

            return completionSource.Task;
        }

        public Task<LeaderboardEntriesResult> GetEntriesAroundCurrentPlayer(string leaderboardName, int maxEntries)
        {
            TaskCompletionSource<LeaderboardEntriesResult> completionSource = new TaskCompletionSource<LeaderboardEntriesResult>();
            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " leaderboardName " + leaderboardName + " maxEntries " + maxEntries);

            ngp_get_entries_around_current_player(correlationId, leaderboardName, maxEntries, NGPLeaderboardGetEntriesDispatchResult);

            return completionSource.Task;
        }
        public Task<LeaderboardEntryResult> GetCurrentPlayerEntry(string leaderboardName)
        {
            TaskCompletionSource<LeaderboardEntryResult> completionSource = new TaskCompletionSource<LeaderboardEntryResult>();
            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " leaderboardName " + leaderboardName);

            ngp_get_current_player_entry(correlationId, leaderboardName, NGPLeaderboardGetEntryDispatchResult);

            return completionSource.Task;
        }

        public Task<LeaderboardInfoResult> GetLeaderboardInfo(string leaderboardName)
        {
            TaskCompletionSource<LeaderboardInfoResult> completionSource = new TaskCompletionSource<LeaderboardInfoResult>();
            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " leaderboardName " + leaderboardName);

            ngp_get_leaderboard_info(correlationId, leaderboardName, NGPLeaderboardGetInfoDispatchResult);

            return completionSource.Task;
        }

        [AOT.MonoPInvokeCallback(typeof(NGPLeaderboardCallbackFuncType))]
        public static void NGPLeaderboardGetEntriesDispatchResult(int correlationId, string resultMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " resultMessage " + resultMessage);

            var completionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<LeaderboardEntriesResult>(correlationId);

            if (completionSource != null)
            {
                var result = new NGPiOSLeaderboards.Serializer().EntriesResult(resultMessage);
                completionSource.SetResult(result);
                NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(NGPLeaderboardCallbackFuncType))]
        public static void NGPLeaderboardGetEntryDispatchResult(int correlationId, string resultMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " resultMessage " + resultMessage);

            var completionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<LeaderboardEntryResult>(correlationId);

            if (completionSource != null)
            {
                var result = new NGPiOSLeaderboards.Serializer().EntryResult(resultMessage);
                completionSource.SetResult(result);
                NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(NGPLeaderboardCallbackFuncType))]
        public static void NGPLeaderboardGetInfoDispatchResult(int correlationId, string resultMessage)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " resultMessage " + resultMessage);

            var completionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<LeaderboardInfoResult>(correlationId);

            if (completionSource != null)
            {
                var result = new NGPiOSLeaderboards.Serializer().InfoResult(resultMessage);

                completionSource.SetResult(result);
                NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
            }
        }

        internal class Serializer
        {
            internal LeaderboardEntriesResult EntriesResult(string resultMessage)
            {
                var serializedResult = JsonUtility.FromJson<SerializableLeaderboardsModels.LeaderboardEntriesResult>(resultMessage);
                var page = serializedResult.page;
                var result = new LeaderboardEntriesResult(serializedResult.status, page);
                return result;
            }

            internal LeaderboardInfoResult InfoResult(string resultMessage)
            {
                var serializedResult = JsonUtility.FromJson<SerializableLeaderboardsModels.LeaderboardInfoResult>(resultMessage);
                var result = new LeaderboardInfoResult(serializedResult);
                return result;
            }

            internal LeaderboardEntryResult EntryResult(string resultMessage)
            {
                var serializedResult = JsonUtility.FromJson<SerializableLeaderboardsModels.LeaderboardEntryResult>(resultMessage);
                var result = new LeaderboardEntryResult(serializedResult);
                return result;
            }
        }
    }
}
#endif