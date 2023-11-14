using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidLeaderboardsApi : SdkApi.ILeaderboards
    {
        private static AndroidJavaObject nfgSdk;
        public NetflixAndroidLeaderboardsApi(AndroidJavaObject sdk)
        {
            nfgSdk = sdk;
        }

        public Task<LeaderboardEntriesResult> GetTopEntries(string leaderboardName, int maxEntries)
        {
            NfLog.Log( MethodBase.GetCurrentMethod() + " leaderboardName " + leaderboardName + " maxEntries " + maxEntries);
            var tcs = new TaskCompletionSource<LeaderboardEntriesResult>();
            NetflixAndroidLeaderboardEntriesCallback callback = new NetflixAndroidLeaderboardEntriesCallback(tcs);
            nfgSdk.Call("getTopEntries", leaderboardName, (int) maxEntries, callback);
            return tcs.Task;   
        }

        public Task<LeaderboardEntriesResult> GetMoreEntries(string leaderboardName, int maxEntries, string cursor, FetchDirection direction)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " leaderboardName " + leaderboardName + " maxEntries " + maxEntries + " cursor " + cursor);
            var tcs = new TaskCompletionSource<LeaderboardEntriesResult>();
            NetflixAndroidLeaderboardEntriesCallback callback = new NetflixAndroidLeaderboardEntriesCallback(tcs);
            nfgSdk.Call("getMoreEntries", leaderboardName, cursor, (int) maxEntries, (int)direction,  callback);
            return tcs.Task;
        }

        public Task<LeaderboardEntriesResult> GetEntriesAroundCurrentPlayer(string leaderboardName, int maxEntries)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " leaderboardName " + leaderboardName + " maxEntries " + maxEntries);
            var tcs = new TaskCompletionSource<LeaderboardEntriesResult>();
            NetflixAndroidLeaderboardEntriesCallback callback = new NetflixAndroidLeaderboardEntriesCallback(tcs);
            nfgSdk.Call("getEntriesAroundCurrentPlayer", leaderboardName, (int) maxEntries, callback);
            return tcs.Task;   
        }

        public Task<LeaderboardEntryResult> GetCurrentPlayerEntry(string leaderboardName)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " leaderboardName " + leaderboardName);
            var tcs = new TaskCompletionSource<LeaderboardEntryResult>();
            NetflixAndroidLeaderboardEntryCallback callback = new NetflixAndroidLeaderboardEntryCallback(tcs);
            nfgSdk.Call("getCurrentPlayerEntry", leaderboardName, callback);
            return tcs.Task; 
        }

        public Task<LeaderboardInfoResult> GetLeaderboardInfo(string leaderboardName)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " leaderboardName " + leaderboardName);
            var tcs = new TaskCompletionSource<LeaderboardInfoResult>();
            NetflixAndroidLeaderboardInfoCallback callback = new NetflixAndroidLeaderboardInfoCallback(tcs);
            nfgSdk.Call("getLeaderboardInfo", leaderboardName, callback);
            return tcs.Task; 
        }
        
    }
}
#endif