using System.Threading.Tasks;
using UnityEngine;
using System;
using Netflix;

#if UNITY_ANDROID
namespace Netflix
{
    class NetflixAndroidGetAggregatedStatCallback : AndroidJavaProxy
    {
        private TaskCompletionSource<Stats.AggregatedStatResult> tcs;

        public NetflixAndroidGetAggregatedStatCallback(TaskCompletionSource<Stats.AggregatedStatResult> tcs)
            : base("com.netflix.unity.api.stats.StatsCallback")
        {
            this.tcs = tcs;
        }

        public void onResult(AndroidJavaObject result)
        {
            var aggregatedStatResult = NetflixJavaConverter.ToAggregatedStatResult(result);
            tcs.SetResult(aggregatedStatResult);
        }
    }
}
#endif