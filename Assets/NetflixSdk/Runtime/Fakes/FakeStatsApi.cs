using System;
using System.Threading.Tasks;

namespace Netflix
{
    internal sealed class FakeStatsApi: SdkApi.IStats
    {
        public Task<Stats.SubmitStatResult> SubmitStatNow(Stats.StatItem statItem)
        {
            NfLog.Log("SubmitStatNow");
            var task = Task<Stats.SubmitStatResult>.Factory.StartNew(() =>
                new Stats.SubmitStatResult
                {
                    status = Stats.StatsStatus.Ok
                });
            return task;
        }

        public void SubmitStat(Stats.StatItem statItem)
        {
            NfLog.Log("SubmitStatNow");
        }

        public Task<Stats.AggregatedStatResult> GetAggregatedStat(String statName)
        {
            NfLog.Log("GetAggregatedStat");
            var task = Task<Stats.AggregatedStatResult>.Factory.StartNew(() =>
                new Stats.AggregatedStatResult
                {
                    status = Stats.StatsStatus.Ok
                });
            return task;        
        }
    }
}