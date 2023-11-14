using System;
using System.Collections.Generic;
using System.ComponentModel;
using Netflix;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class InGameTest
{
    private enum TestEnum
    {
        None,
        TestValue1,
        TestValue2,
        TestValue3
    }
    
    private class TestEvent : InGameEvent
    {
        public int intValue { get; }
        public string strValue { get; }
        public Boolean boolValue { get; }
        public double doubleValue { get; }
        public List<string> strList { get; }
        public List<int> intList { get; }
        public List<bool> boolList { get; }
        public List<double> doubleList { get; }
        public List<TestEnum> enumList { get; }
        public int[] intArray { get; }
        [DefaultValue(56)]
        public int defaultValueInt { get; }
        [DefaultValue(TestEnum.None)]
        public TestEnum enumValue { get; }

        public TestEvent(int intValue, 
                        string strValue, 
                        Boolean boolValue, 
                        double doubleValue,
                        List<string> strList = null,
                        List<int> intList = null,
                        List<bool> boolList = null,
                        List<double> doubleList = null,
                        List<TestEnum> enumList = null,
                        int[] intArray = null,
                        int defaultValueInt = 56,
                        TestEnum enumValue = TestEnum.None
            ) : base("TestEvent")
        {
            this.intValue = intValue;
            this.strValue = strValue;
            this.boolValue = boolValue;
            this.doubleValue = doubleValue;
            this.strList = strList;
            this.intList = intList;
            this.boolList = boolList;
            this.doubleList = doubleList;
            this.enumList = enumList;
            this.intArray = intArray;
            this.enumValue = enumValue;
            this.defaultValueInt = defaultValueInt;
        }
    }
    
    private string GetTestString(string jsonString)
    {
        return jsonString.Replace("\n", "")
            .Replace("\r", "")
            .Replace("\t", "")
            .Replace(" ", "");
    }

    [Test]
    public void TestBasicEventTypes()
    {
        var e = new TestEvent(23, "test23", true, 76.5, defaultValueInt:57);
        Assert.AreEqual("TestEvent", e.name);
        var jsonifiedObject = e.ToJson();
        var testString = GetTestString(@"{
""intValue"":23,
""strValue"":""test23"",
""boolValue"":true,
""doubleValue"":76.5,
""defaultValueInt"":57
}");
        Assert.AreEqual(testString, jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }
    
    [Test]
    public void TestBasicEventLists()
    {
        var e = new TestEvent(4, "Testing4", false, 100.9,
                new List<string>{"str1", "str2"},
                new List<int>{5, 6, 8},
                new List<bool>{true, true, true},
                new List<double>{11.8,98.6,112.4},
                new List<TestEnum> {TestEnum.TestValue3},
                new int[]{67, 90, 1}
            );
        Assert.AreEqual("TestEvent", e.name);
        var jsonifiedObject = e.ToJson();
        var testString = GetTestString(@"{
""intValue"":4,
""strValue"":""Testing4"",
""boolValue"":false,
""doubleValue"":100.9,
""strList"":[""str1"",""str2""],
""intList"":[5,6,8],
""boolList"":[true,true,true],
""doubleList"":[11.8,98.6,112.4],
""enumList"":[""TestValue3""],
""intArray"":[67,90,1]
}");
        Assert.AreEqual(testString, jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }
    
    [Test]
    public void TestQuotedString()
    {
        var testQuote = "This_is_a_string_with_a_quote_\"in_it";
        var e = new TestEvent(23, testQuote, true, 33.3);
        Assert.AreEqual("TestEvent", e.name);
        var jsonifiedObject = e.ToJson();
        var testString = GetTestString(@"{
""intValue"":23,
""strValue"":""This_is_a_string_with_a_quote_\""in_it"",
""boolValue"":true,
""doubleValue"":33.3
}");
        Assert.AreEqual(testString, jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void NewGameStartTest()
    {
        var e = new NewGameStart();
        Assert.AreEqual("game.inGame.NewGameStart", e.name);
        var jsonifiedObject = e.ToJson();
        Assert.AreEqual("{}", jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void FTUEStartTest()
    {
        var e = new FirstTimeUserExperienceStart();
        Assert.AreEqual("game.inGame.FTUEStart", e.name);
        var jsonifiedObject = e.ToJson();
        Assert.AreEqual("{}", jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void FTUEComplete()
    {
        var e = new FirstTimeUserExperienceComplete();
        Assert.AreEqual("game.inGame.FTUEComplete", e.name);
        var jsonifiedObject = e.ToJson();
        Assert.AreEqual("{}", jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void FTUEStepComplete()
    {
        var e = new FirstTimeUserExperienceStepComplete(1.2, "Test_Step_1.2", "Test_Description");
        Assert.AreEqual("game.inGame.FTUEStepComplete", e.name);
        var jsonifiedObject = e.ToJson();
        var testString = GetTestString(@"{
            ""stepNumber"":1.2,
            ""stepName"":""Test_Step_1.2"",
            ""stepDesc"":""Test_Description""
        }");
        Assert.AreEqual(testString, jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void ProgressCheckpointCompleteTest()
    {
        var e = new ProgressCheckpointComplete(5.4, "Test_Checkpoint_5.4", "Test_Checkpoint_Description");
        Assert.AreEqual("game.inGame.ProgressCheckpointComplete", e.name);
        var jsonifiedObject = e.ToJson();
        var testString = GetTestString(@"{
            ""checkpointNumber"":5.4,
            ""checkpointName"":""Test_Checkpoint_5.4"",
            ""checkpointType"":""Test_Checkpoint_Description""
        }");
        Assert.AreEqual(testString, jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void GameCompleteTest()
    {
        var e = new GameComplete();
        Assert.AreEqual("game.inGame.GameComplete", e.name);
        var jsonifiedObject = e.ToJson();
        Assert.AreEqual("{}", jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }

    [Test]
    public void CustomTest()
    {
        var e = new Custom("CustomEvent", "{\"testKey\":\"testValue\"}");
        Assert.AreEqual("CustomEvent", e.name);
        var jsonifiedObject = e.ToJson();
        var testString = GetTestString(@"{
            ""testKey"":""testValue""
        }");
        Assert.AreEqual(testString, jsonifiedObject);
        Debug.Log(jsonifiedObject);
    }
}
