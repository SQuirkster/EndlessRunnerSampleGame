using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidLeaderboardInfoCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<LeaderboardInfoResult> tcs;

        public NetflixAndroidLeaderboardInfoCallback(TaskCompletionSource<LeaderboardInfoResult> tcs) :
            base("com.netflix.unity.api.leaderboards.LeaderboardInfoCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var leaderboardInfoResult =  NetflixJavaConverter.ToLeaderboardInfoResult(result);
            tcs.SetResult(leaderboardInfoResult);
        }
        
    }
}
#endif