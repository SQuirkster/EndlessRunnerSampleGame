#if UNITY_IOS
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Netflix.SerializableStatModels;
using UnityEngine;

namespace Netflix
{
    internal class NGPiOSStats : SdkApi.IStats
    {
        [DllImport("__Internal")]
        private static extern void ngp_submit_stat(string statName, Int64 statValue);
        [DllImport("__Internal")]
        private static extern void ngp_submit_stat_now(int correlationId, string statName, Int64 statValue, NGPLeaderboardCallbackFuncType callback);
        [DllImport("__Internal")]
        private static extern void ngp_get_aggregated_stat(int correlationId, string statName, NGPLeaderboardCallbackFuncType callback);

        public void SubmitStat(Stats.StatItem statItem)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " name " + statItem.name + " value" + statItem.value);

            ngp_submit_stat(statItem.name, statItem.value);
        }

        public Task<Stats.SubmitStatResult> SubmitStatNow(Stats.StatItem statItem)
        {
            TaskCompletionSource<Stats.SubmitStatResult> completionSource = new TaskCompletionSource<Stats.SubmitStatResult>();

            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " name " + statItem.name + " value " + statItem.value);

            ngp_submit_stat_now(correlationId, statItem.name, statItem.value, NGPStatSubmitDispatchResult);

            return completionSource.Task;
        }

        public Task<Stats.AggregatedStatResult> GetAggregatedStat(string statName)
        {
            NfLog.Log(MethodBase.GetCurrentMethod() + " statName " + statName);

            TaskCompletionSource<Stats.AggregatedStatResult> completionSource = new TaskCompletionSource<Stats.AggregatedStatResult>();

            int correlationId = NetflixBridgeCallbackManager.shared.AddCompletionSource(completionSource);

            ngp_get_aggregated_stat(correlationId, statName, NGPAggregateStatDispatchResult);

            NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " statName " + statName);

            return completionSource.Task;
        }

        [MonoPInvokeCallback(typeof(NGPLeaderboardCallbackFuncType))]
        public static void NGPStatSubmitDispatchResult(int correlationId, string resultMessage)
        {
            MethodBase methodBase = MethodBase.GetCurrentMethod();
            NfLog.Log(methodBase + " correlationId " + correlationId + " resultMessage " + resultMessage);

            TaskCompletionSource<Stats.SubmitStatResult> submitCompletionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<Stats.SubmitStatResult>(correlationId);

            if (submitCompletionSource != null)
            {
                Stats.SubmitStatResult statResult = StatsResponseParser.parseSubmitStatResult(resultMessage);
                NfLog.Log(methodBase + " correlationId " + correlationId + " SubmitStatResult " + JsonUtility.ToJson(statResult));
                if (statResult != null)
                {
                    submitCompletionSource.SetResult(statResult);

                    NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
                }
                return;
            }
        }

        [MonoPInvokeCallback(typeof(NGPLeaderboardCallbackFuncType))]
        public static void NGPAggregateStatDispatchResult(int correlationId, string resultMessage)
        {
            MethodBase methodBase = MethodBase.GetCurrentMethod();
            NfLog.Log(methodBase + " correlationId " + correlationId + " resultMessage " + resultMessage);

            TaskCompletionSource<Stats.AggregatedStatResult> aggregatedCompletionSource = NetflixBridgeCallbackManager.shared.CompletionSourceForId<Stats.AggregatedStatResult>(correlationId);

            if (aggregatedCompletionSource != null)
            {
                Stats.AggregatedStatResult statResult = StatsResponseParser.parseAggregatedStatResult(resultMessage);
                NfLog.Log(MethodBase.GetCurrentMethod() + " correlationId " + correlationId + " AggregatedStatResult " + JsonUtility.ToJson(statResult));
                if (statResult != null)
                {
                    aggregatedCompletionSource.SetResult(statResult);

                    NetflixBridgeCallbackManager.shared.RemoveCompletionSourceForId(correlationId);
                }
            }
        }
    }
    
    internal static class StatsResponseParser
    {
        internal static Stats.AggregatedStatResult parseAggregatedStatResult(string resultMessage)
        {
            var serializedModel = JsonUtility.FromJson<SerializedAggregatedStatResult>(resultMessage);
            return new Stats.AggregatedStatResult(serializedModel);
        }

        internal static Stats.SubmitStatResult parseSubmitStatResult(string resultMessage)
        {
            var serializedModel = JsonUtility.FromJson<SerializedSubmitStatResponse>(resultMessage);
            return new Stats.SubmitStatResult(serializedModel);
        }
    }
}
#endif