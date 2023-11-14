using System.Threading.Tasks;
using UnityEngine;

#if UNITY_ANDROID
namespace Netflix
{
    internal class NetflixAndroidStatsApi : SdkApi.IStats
    {
        private static AndroidJavaObject nfgSdk;
        public NetflixAndroidStatsApi(AndroidJavaObject sdk)
        {
            nfgSdk = sdk;
        }

        public void SubmitStat(Stats.StatItem statItem)
        {
            NfLog.Log("NetflixAndroidStats SubmitStat: ");
            nfgSdk.Call("submitStat", statItem.name, statItem.value);
        }

        public Task<Stats.SubmitStatResult> SubmitStatNow(Stats.StatItem statItem)
        {
            NfLog.Log("NetflixAndroidStats SubmitStatNow: " + statItem.value + ", value: " + statItem.value);
            var tcs = new TaskCompletionSource<Stats.SubmitStatResult>();
            var callback = new NetflixAndroidSubmitStatCallback(tcs);
            nfgSdk.Call("submitStatNow", statItem.name, statItem.value, callback);
            return tcs.Task;
        }

        public Task<Stats.AggregatedStatResult> GetAggregatedStat(string statName)
        {
            NfLog.Log("NetflixAndroidStats GetAggregatedStat " + " statName: " + statName);
            var tcs = new TaskCompletionSource<Stats.AggregatedStatResult>();
            var callback = new NetflixAndroidGetAggregatedStatCallback(tcs);
            nfgSdk.Call("getAggregatedStat", statName, callback);
            return tcs.Task;
        }
    }
}
#endif