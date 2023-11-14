using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidLeaderboardEntryCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<LeaderboardEntryResult> tcs;

        public NetflixAndroidLeaderboardEntryCallback(TaskCompletionSource<LeaderboardEntryResult> tcs) :
            base("com.netflix.unity.api.leaderboards.LeaderboardEntryCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var leaderboardEntryResult =  NetflixJavaConverter.ToLeaderboardEntryResult(result);
            tcs.SetResult(leaderboardEntryResult);
        }
        
    }
}
#endif