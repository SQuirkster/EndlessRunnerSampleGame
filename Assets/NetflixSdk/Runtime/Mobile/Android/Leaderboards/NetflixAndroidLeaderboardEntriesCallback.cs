using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidLeaderboardEntriesCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<LeaderboardEntriesResult> tcs;

        public NetflixAndroidLeaderboardEntriesCallback(TaskCompletionSource<LeaderboardEntriesResult> tcs) :
            base("com.netflix.unity.api.leaderboards.LeaderboardEntriesCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var leaderboardEntriesResult =  NetflixJavaConverter.ToLeaderboardEntriesResult(result);
            tcs.SetResult(leaderboardEntriesResult);
        }
        
    }
}
#endif