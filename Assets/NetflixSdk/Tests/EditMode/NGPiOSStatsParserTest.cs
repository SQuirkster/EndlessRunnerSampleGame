using NUnit.Framework;
using Netflix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_IOS

public class NGPiOSStatsParserTest
{
    [Test]
    public void TestAggregatedStatSuccess()
    {
        var resultMessage = @"{
                          ""aggregatedStat"": {
                            ""name"": ""SAMPLE_STAT"",
                            ""value"": 99999999
                          },
                          ""status"": 0
                        }";

        Stats.AggregatedStatResult result = StatsResponseParser.parseAggregatedStatResult(resultMessage);

        Assert.NotNull(result);
        Assert.AreEqual(result.status, Stats.StatsStatus.Ok);
        Assert.NotNull(result.aggregatedStat);
        Assert.AreEqual(result.aggregatedStat.name, "SAMPLE_STAT");
        Assert.AreEqual(result.aggregatedStat.value, 99999999);
    }

    [Test]
    public void TestAggregatedStatEmpty()
    {
        var resultMessage = "{\"status\":1000}";

        Stats.AggregatedStatResult result = StatsResponseParser.parseAggregatedStatResult(resultMessage);

        Assert.NotNull(result);
        Assert.AreEqual(result.status, Stats.StatsStatus.ErrorUnknown);
        Assert.IsNull(result.aggregatedStat);
    }


    [Test]
    public void TestSubmitStatSuccess()
    {
        var resultMessage = @"{
                          ""aggregatedStat"": {
                            ""name"": ""SAMPLE_STAT"",
                            ""value"": 999999
                          },
                            ""submittedStat"": {
                            ""name"": ""SAMPLE_STAT"",
                            ""value"": 999999
                          },
                          ""status"": 0
                        }";


        Stats.SubmitStatResult result = StatsResponseParser.parseSubmitStatResult(resultMessage);

        Assert.NotNull(result);
        Assert.AreEqual(result.status, Stats.StatsStatus.Ok);
        Assert.NotNull(result.aggregatedStat);
        Assert.AreEqual(result.aggregatedStat.name, "SAMPLE_STAT");
        Assert.AreEqual(result.aggregatedStat.value, 999999);

        Assert.NotNull(result);
        Assert.NotNull(result.submittedStat);
        Assert.AreEqual(result.submittedStat.name, "SAMPLE_STAT");
        Assert.AreEqual(result.submittedStat.value, 999999);
    }


    [Test]
    public void TestSubmitStatEmpty()
    {
        var resultMessage = "{\"status\":1000}";

        Stats.SubmitStatResult result = StatsResponseParser.parseSubmitStatResult(resultMessage);

        Assert.NotNull(result);
        Assert.AreEqual(result.status, Stats.StatsStatus.ErrorUnknown);
        Assert.IsNull(result.aggregatedStat);
        Assert.IsNull(result.submittedStat);
    }
}
#endif