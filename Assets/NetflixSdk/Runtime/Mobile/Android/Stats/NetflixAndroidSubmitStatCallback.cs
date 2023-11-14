using System.Threading.Tasks;
using UnityEngine;
using System;
using Netflix;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidSubmitStatCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<Stats.SubmitStatResult> tcs;

        public NetflixAndroidSubmitStatCallback(TaskCompletionSource<Stats.SubmitStatResult> tcs)
            : base("com.netflix.unity.api.stats.StatsCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var submitStatResult = NetflixJavaConverter.ToSubmitStatResult(result);
            tcs.SetResult(submitStatResult);
        }
    }
}
#endif