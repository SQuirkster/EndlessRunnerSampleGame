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

public class NGPiOSAchievementsParsingTests
{
    [Test]
    public void TestAchievementOk()
    {
        var resultMessage = @"{
                          ""achievement"": {
                            ""name"": ""test_name"",
                            ""isLocked"": 1
                           },
                           ""status"": 0
                           }";

        var achievement = Netflix.iOSSerialization.AchievementsResponseParser.parseUnlockAchievementResult(resultMessage);
        Assert.AreEqual(achievement.status, AchievementStatus.Ok);
        Assert.AreEqual(achievement.achievement.isLocked, true);
        Assert.AreEqual(achievement.achievement.name, "test_name");
    }

    [Test]
    public void TestAchievementNull()
    {
        var resultMessage = @"{
                           ""status"": 1000
                           }";

        var achievement = Netflix.iOSSerialization.AchievementsResponseParser.parseUnlockAchievementResult(resultMessage);
        Assert.AreEqual(achievement.status, AchievementStatus.ErrorUnknown);
        Assert.IsNull(achievement.achievement);
    }

    [Test]
    public void TestAchievementEntries()
    {
        var resultMessage = @"{
                           ""status"": 0,
                            ""achievements"": [
                                {
                                    ""name"": ""one"",
                                    ""isLocked"": 1
                                },
                                {
                                    ""name"": ""two"",
                                    ""isLocked"": 0
                                }
                            ]

                           }";

        var achievements = Netflix.iOSSerialization.AchievementsResponseParser.parseAchievementsResult(resultMessage);
        Assert.AreEqual(achievements.status, AchievementStatus.Ok);

        Assert.AreEqual(achievements.achievements.Count, 2);

        Assert.AreEqual(achievements.achievements[0].name, "one");
        Assert.AreEqual(achievements.achievements[0].isLocked, true);

        Assert.AreEqual(achievements.achievements[1].name, "two");
        Assert.AreEqual(achievements.achievements[1].isLocked, false);
    }
}
#endif