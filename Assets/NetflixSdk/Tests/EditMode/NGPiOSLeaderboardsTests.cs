using NUnit.Framework;
using Netflix;

#if UNITY_IOS

public class NGPiOSLeaderboardsTests
{
    [Test]
    public void TestEntriesFailure()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = "{\"status\":1000}";

        var result = serializer.EntriesResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.ErrorUnknown);
        Assert.IsNull(result.page);
    }

    [Test]
    public void TestEntriesSuccess()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = @"{
                          ""page"": {
                            ""endCursor"": ""CGQQCg=="",
                            ""leaderboardEntries"": [
                              {
                                ""rank"": 1,
                                ""score"": 7781217836773164382,
                                ""playerIdentity"": {
                                  ""playerId"": ""I56NKBRCJ5FP5JQA7N2H4BAIRE"",
                                  ""handle"": ""amazed""
                                }
                              },
                              {
                                ""rank"": 2,
                                ""playerIdentity"": {
                                  ""handle"": ""SurroundSound"",
                                  ""playerId"": ""7VGGUDIUYRHH3IX2PIODS2LXSU""
                                },
                                ""score"": 110001
                              }
                            ],
                            ""hasMoreAfterEnd"": true,
                            ""hasMoreBeforeStart"": false,
                            ""startCursor"": ""CN6KscbVjJz+axAB""
                          },
                          ""status"": 0
                        }";

        var result = serializer.EntriesResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.Ok);
        Assert.AreEqual(result.page.startCursor, "CN6KscbVjJz+axAB");
        Assert.AreEqual(result.page.hasMoreAfterEnd, true);
        Assert.AreEqual(result.page.hasMoreBeforeStart, false);
        Assert.AreEqual(result.page.leaderboardEntries.Count, 2);
        Assert.AreEqual(result.page.leaderboardEntries[0].rank, 1);
        Assert.AreEqual(result.page.leaderboardEntries[0].score, 7781217836773164382);
        Assert.AreEqual(result.page.leaderboardEntries[0].playerIdentity.handle, "amazed");
        Assert.AreEqual(result.page.leaderboardEntries[0].playerIdentity.playerId, "I56NKBRCJ5FP5JQA7N2H4BAIRE");
    }

    [Test]
    public void TestEntriesMissingHandle()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = @"{
                          ""page"": {
                            ""endCursor"": ""CGQQCg=="",
                            ""leaderboardEntries"": [
                              {
                                ""rank"": 1,
                                ""score"": 7781217836773164382,
                                ""playerIdentity"": {
                                  ""playerId"": ""I56NKBRCJ5FP5JQA7N2H4BAIRE"",
                                  ""handle"": null
                                }
                              }
                            ],
                            ""hasMoreAfterEnd"": true,
                            ""hasMoreBeforeStart"": false,
                            ""startCursor"": ""CN6KscbVjJz+axAB""
                          },
                          ""status"": 0
                        }";

        var result = serializer.EntriesResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.Ok);
        Assert.AreEqual(result.page.startCursor, "CN6KscbVjJz+axAB");
        Assert.AreEqual(result.page.hasMoreAfterEnd, true);
        Assert.AreEqual(result.page.hasMoreBeforeStart, false);
        Assert.AreEqual(result.page.leaderboardEntries.Count, 1);
        Assert.AreEqual(result.page.leaderboardEntries[0].rank, 1);
        Assert.AreEqual(result.page.leaderboardEntries[0].score, 7781217836773164382);
        Assert.AreEqual(result.page.leaderboardEntries[0].playerIdentity.handle, null);
        Assert.AreEqual(result.page.leaderboardEntries[0].playerIdentity.playerId, "I56NKBRCJ5FP5JQA7N2H4BAIRE");
    }


    [Test]
    public void TestEntrySuccess()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = @"
            {""status"":0,""entry"":{""playerIdentity"":{""playerId"":""EG5TLW77SFFZXEJ5MXT432BVYM"",""handle"":""Turkeysticks""},""rank"":6,""score"":10008}}
        ";

        var result = serializer.EntryResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.Ok);
        Assert.AreEqual(result.entry.rank, 6);
        Assert.AreEqual(result.entry.score, 10008);
        Assert.AreEqual(result.entry.playerIdentity.playerId, "EG5TLW77SFFZXEJ5MXT432BVYM");
        Assert.AreEqual(result.entry.playerIdentity.handle, "Turkeysticks");
    }

    [Test]
    public void TestEntryMissingHandle()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = @"
            {""status"":0,""entry"":{""playerIdentity"":{""playerId"":""EG5TLW77SFFZXEJ5MXT432BVYM"",""handle"":null},""rank"":6,""score"":10008}}
        ";

        var result = serializer.EntryResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.Ok);
        Assert.AreEqual(result.entry.rank, 6);
        Assert.AreEqual(result.entry.score, 10008);
        Assert.AreEqual(result.entry.playerIdentity.playerId, "EG5TLW77SFFZXEJ5MXT432BVYM");
        Assert.IsNull(result.entry.playerIdentity.handle);
    }

    [Test]
    public void TestEntryFailure()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = @"
            {""status"":1000}
        ";

        var result = serializer.EntryResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.ErrorUnknown);
        Assert.IsNull(result.entry);
    }

    [Test]
    public void TestLeaderboardInfoSuccess()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = "{\"status\":0,\"leaderboardInfo\":{\"count\":66,\"name\":\"SAMPLE_LEADERBOARD\"}}";

        var result = serializer.InfoResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.Ok);
        Assert.AreEqual(result.leaderboardInfo.count, 66);
        Assert.AreEqual(result.leaderboardInfo.name, "SAMPLE_LEADERBOARD");
    }

    [Test]
    public void TestLeaderboardInfoFailure()
    {
        var serializer = new NGPiOSLeaderboards.Serializer();

        var response = "{\"status\":1000}";

        var result = serializer.InfoResult(response);

        Assert.AreEqual(result.status, Netflix.LeaderboardStatus.ErrorUnknown);
        Assert.IsNull(result.leaderboardInfo);
    }
}
#endif