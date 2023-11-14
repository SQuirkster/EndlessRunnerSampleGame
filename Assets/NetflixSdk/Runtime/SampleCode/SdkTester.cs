using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Netflix;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;

public class SdkTester : MonoBehaviour
{
    private Button checkUserAuthButton;
    private Button showNetflixUiButton;
    private Button hideNetflixUiButton;
    private Button setLocaleButton;
    private Button readSlotButton;
    private Button saveSlotButton;
    private Button deleteSlotButton;
    private Button getSlotIdsButton;
    private Button resolveConflictButton;
    private Button saveReadLoopButton;
    private Button getCurrentPlayerButton;
    private Button getPlayerIdentitiesButton;
    private Button sendInGameEventsButton;
    private Button showAchievementsButton;
    private Button raiseExceptionButton;
    private Button checkSupportedFeaturesButton;
    private Button setEditorCurrentProfileButton;
    public GameObject leaderboardsPanel;
    public GameObject achievementsPanel;

    private const string TEST_SLOT_ID = "highscores";
    private const int TEST_RANDOM_PAYLOAD_LEN = 768000;
    private const int TEST_SHOW_PAYLOAD_LEN = 200;
    private static int player_id = 0;

    // not used currently.
    GameObject GetChildWithName(GameObject obj, string statValue)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(statValue);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    // shows a debug text
    void ShowDebugText(string text)
    {
        SdkHolder.AssertMainThread();
        // long string cause ArgumentException: Mesh can not have more than 65000 vertices
        if (text == null) {
            return;
        }

        var textLength = text.Length;
        string subText;
        if (textLength <= TEST_SHOW_PAYLOAD_LEN)
        {
            subText = text;
        }
        else
        {
            subText = text.Substring(0, Math.Min(textLength, TEST_SHOW_PAYLOAD_LEN)) + " ...(" + textLength + ")";
        }
        GameObject.Find("debugText").GetComponent<Text>().text = subText;
    }

    // Start is called before the first frame update
    void Start()
    {
        print("SdkTester start found this.gameObject=" + this.gameObject);
        checkUserAuthButton = GameObject.Find("checkUserAuth").GetComponent<Button>();
        showNetflixUiButton = GameObject.Find("showNetflixUi").GetComponent<Button>();
        hideNetflixUiButton = GameObject.Find("hideNetflixUi").GetComponent<Button>();
        setLocaleButton     = GameObject.Find("setLocale").GetComponent<Button>();
        saveSlotButton      = GameObject.Find("saveSlot").GetComponent<Button>();
        readSlotButton      = GameObject.Find("readSlot").GetComponent<Button>();
        deleteSlotButton    = GameObject.Find("deleteSlot").GetComponent<Button>();
        getSlotIdsButton    = GameObject.Find("getSlotIds").GetComponent<Button>();
        resolveConflictButton = GameObject.Find("resolveConflict").GetComponent<Button>();
        saveReadLoopButton  = GameObject.Find("saveReadLoop").GetComponent<Button>();
        getCurrentPlayerButton = GameObject.Find("getCurrentPlayer").GetComponent<Button>();
        getPlayerIdentitiesButton = GameObject.Find("getPlayerIdentities").GetComponent<Button>();
        sendInGameEventsButton = GameObject.Find("sendInGameEvents").GetComponent<Button>();
        showAchievementsButton = GameObject.Find("showAchievements").GetComponent<Button>();
        checkSupportedFeaturesButton = GameObject.Find("checkSupportedFeatures").GetComponent<Button>();
        raiseExceptionButton = GameObject.Find("raiseException").GetComponent<Button>();
        setEditorCurrentProfileButton = GameObject.Find("setEditorCurrentProfile").GetComponent<Button>();

        checkUserAuthButton.onClick.AddListener(CallCheckUserAuth);
        showNetflixUiButton.onClick.AddListener(CallShowNetflixUi);
        hideNetflixUiButton.onClick.AddListener(CallHideNetflixUi);
        setLocaleButton.onClick.AddListener(CallSetLocale);
        saveSlotButton.onClick.AddListener(CallSaveSlot);
        readSlotButton.onClick.AddListener(CallReadSlot);
        deleteSlotButton.onClick.AddListener(CallDeleteSlot);
        resolveConflictButton.onClick.AddListener(CallResolveConflict);
        getSlotIdsButton.onClick.AddListener(CallGetSlotIds);
        saveReadLoopButton.onClick.AddListener(SaveReadLoopTest);
        getCurrentPlayerButton.onClick.AddListener(CallGetCurrentPlayer);
        getPlayerIdentitiesButton.onClick.AddListener(CallGetPlayerIdentities);
        sendInGameEventsButton.onClick.AddListener(CallInGameEventsListener);
        showAchievementsButton.onClick.AddListener(ShowAchievements);
        checkSupportedFeaturesButton.onClick.AddListener(CheckSupportedFeatures);
        raiseExceptionButton.onClick.AddListener(raiseException);
        setEditorCurrentProfileButton.onClick.AddListener(SetEditorCurrentProfile);
    }

    void CallCheckUserAuth()
    {
        ShowDebugText("calling NetflixSdk.CheckUserAuth");
        Netflix.NetflixSdk.CheckUserAuth();
    }
    void raiseException()
    {
        ShowDebugText("raiseException button clicked");
        throw new Exception("SDK Tester raiseException " + DateTime.Now.ToString());
    }
    
    private void SetEditorCurrentProfile()
    {
#if UNITY_EDITOR
        var profile = new NetflixSdk.NetflixProfile
        {
            playerId = "Player" + player_id,
            gamerProfileId = "GamerProfileId" + player_id,
            loggingId =  "LoggingId" + player_id,
            netflixAccessToken = "GAT" + player_id,
            locale = new NetflixSdk.Locale
            {
                language = "fr",
                country = "CA"
            }
        };
        NetflixSdk.SetEditorCurrentProfile(profile);
        player_id++;
        CallCheckUserAuth();
#else
        ShowDebugText("SetEditorCurrentProfile is not supported on this platform");
#endif
    }

    void CallShowNetflixUi()
    {
        GameObject.Find("event").GetComponent<Text>().text = "calling NetflixSdk.HideNetflixAccessButton";
        Netflix.NetflixSdk.ShowNetflixAccessButton();
    }

    void CallHideNetflixUi()
    {
        ShowDebugText("calling NetflixSdk.HideNetflixAccessButton");
        Netflix.NetflixSdk.HideNetflixAccessButton();
    }

    void CallSetLocale()
    {
        ShowDebugText("calling NetflixSdk.SetLocale");
        NetflixSdk.Locale locale = new NetflixSdk.Locale
        {
            language = "en",
            country = "US"
        };
        Netflix.NetflixSdk.SetLocale(locale);
    }

    void CallReadSlot()
    {
        ShowDebugText("calling NetflixSdk.ReadSlot");
        Task<NetflixCloudSave.ReadSlotResult> resultTask = NetflixCloudSave.ReadSlot(TEST_SLOT_ID);
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            var dataString = "null";
            var timestamp = "null";
            if (cloudSaveResult.slotInfo != null && cloudSaveResult.slotInfo.GetDataBytes() != null)
            {
                dataString = ByteArrayToString(cloudSaveResult.slotInfo.GetDataBytes());
                timestamp = cloudSaveResult.slotInfo.GetServerSyncTimestamp();
            }
            ResponseManager.AddNewResponseFromAnyThread(new ResponseDispatcher(() =>
            {
                ShowDebugText("ReadSlot status: " + cloudSaveResult.status  + " errorDesc: " + cloudSaveResult.errorDescription + " timestamp: " + timestamp + " data: " + dataString);

                if (cloudSaveResult.status == NetflixCloudSave.CloudSaveStatus.SlotConflict ) {
                    ShowDebugText("read status: " + cloudSaveResult.status +  ", " +  cloudSaveResult.conflictResolution.local.GetServerSyncTimestamp() + ", remote.timestamp: " +  cloudSaveResult.conflictResolution.remote.GetServerSyncTimestamp());
                }
            }));
        });
        continuation.Wait();
    }

    void CallSaveSlot()
    {
        ShowDebugText("calling NetflixSdk.SaveSlot");
        var content = "Hello world! Now " + DateTime.Now.ToString() + " - " + RandomStringGenerator(TEST_RANDOM_PAYLOAD_LEN);
        var slotInfo = new NetflixCloudSave.SlotInfo(Encoding.UTF8.GetBytes(content));
        var resultTask = NetflixCloudSave.SaveSlot(TEST_SLOT_ID, slotInfo);
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            ResponseManager.AddNewResponseFromAnyThread(new ResponseDispatcher(() =>
            {
                ShowDebugText("SaveSlot status: " + cloudSaveResult.status + " errorDecription: " + cloudSaveResult.errorDescription);

                // Doing conflic resolution here to mimic the calling pattern that game comps such as rogue do
                if (cloudSaveResult.status == NetflixCloudSave.CloudSaveStatus.SlotConflict ) {
                    CallResolveConflict();
                }
            }));
        });
        continuation.Wait();
    }

    string RandomStringGenerator(int length)
    {
        const string table = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var tableLastIndex = table.Length-1;
        var randomCharacters = new char[length];

        for(int i = 0; i < length; i++) {
            randomCharacters[i] = table[Range(0, tableLastIndex)];
        }

        return new string(randomCharacters);
    }

    void CallDeleteSlot()
    {
        ShowDebugText("calling NetflixSdk.DeleteSlot");
        var resultTask = NetflixCloudSave.DeleteSlot(TEST_SLOT_ID);
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            ResponseManager.AddNewResponseFromAnyThread(new ResponseDispatcher(() =>
            {
                ShowDebugText("DeleteSlot status: " + cloudSaveResult.status  + " errorDesc: " + cloudSaveResult.errorDescription);
            }));
        });
        continuation.Wait();
    }

    void CallResolveConflict()
    {
        ShowDebugText("calling NetflixSdk.ResolveConflict");
        var resultTask = NetflixCloudSave.ResolveConflict(TEST_SLOT_ID, NetflixCloudSave.CloudSaveResolution.KeepLocal);
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            ResponseManager.AddNewResponseFromAnyThread(new ResponseDispatcher(() =>
            {
                ShowDebugText("ResolveConflict status: " + cloudSaveResult.status);
            }));
        });
        continuation.Wait();
    }

    void CallGetSlotIds()
    {
        ShowDebugText("calling NetflixSdk.GetSlotIds");
        var resultTask = NetflixCloudSave.GetSlotIds();
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            var sb = new StringBuilder();
            sb.Append("GetSlotIds status: " + cloudSaveResult.status);

            if (cloudSaveResult.status == NetflixCloudSave.CloudSaveStatus.Ok)
            {
                sb.Append("[");
                foreach (String slot in cloudSaveResult.slotIds)
                {
                    sb.Append(slot);
                    sb.Append(", ");
                }
                sb.Append("]");
            }
            ResponseManager.AddNewResponseFromAnyThread(new ResponseDispatcher(() =>
            {
                ShowDebugText(sb.ToString());
            }));
        });
        continuation.Wait();
    }

    void SaveReadLoopTest()
    {
        ShowDebugText("start SaveReadLoopTest");

        long score = 0;
        long readScore = 10;
        bool passed = true;
        for (int i = 0; i < 20; i++)
        {
            ShowDebugText("SaveReadLoopTest Loop " + i);
            score = readScore * 2 + i;
            SaveScore(score);
            readScore = ReadScore();
            if (readScore != score)
            {
                passed = false;
                break;
            }
        }
        var sb = new StringBuilder();
        sb.Append("SaveReadLoopTest result :");
        sb.Append(passed ? "passed" : "fail");
        if (!passed)
        {
            sb.Append("\n Saved : " + score);
            sb.Append("\n Read  : " + readScore);
        }
        ShowDebugText(sb.ToString());
    }

    private void SaveScore(long score)
    {
        NfLog.Log("calling NetflixSdk.SaveSlot(" + TEST_SLOT_ID + ", " + score + ")");
        var bytes = BitConverter.GetBytes(score);
        NfLog.Log("   bytes: " + BitConverter.ToString(bytes));
        var slotInfo = new NetflixCloudSave.SlotInfo(bytes);
        var resultTask = NetflixCloudSave.SaveSlot(TEST_SLOT_ID, slotInfo);
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            NfLog.Log("SaveSlot status: " + cloudSaveResult.status);
        });
        continuation.Wait();
    }

    private long ReadScore()
    {
        NfLog.Log("calling NetflixSdk.ReadSlot(" + TEST_SLOT_ID + ")");
        byte[] data = null;
        var resultTask = NetflixCloudSave.ReadSlot(TEST_SLOT_ID);
        var continuation = resultTask.ContinueWith(task =>
        {
            var cloudSaveResult = task.Result;
            NfLog.Log("ReadSlot status: " + cloudSaveResult.status);
            NfLog.Log("ReadSlot serverSyncTimestamp: " + cloudSaveResult.slotInfo.GetServerSyncTimestamp());
            data = cloudSaveResult.slotInfo.GetDataBytes();
            if (data == null)
            {
                NfLog.Log("ReadSlot data bytes: null");
            }
            else
            {
                NfLog.Log("ReadSlot data bytes: " + BitConverter.ToString(data));
                NfLog.Log("ReadSlot data int  : " + BitConverter.ToInt32(data, 0));
            }
        });
        continuation.Wait();
        return BitConverter.ToInt32(data, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private string ByteArrayToString(byte[] byteArray)
    {
        if (byteArray == null)
        {
            return "null";
        }
        return System.Text.Encoding.UTF8.GetString(byteArray);
    }

    void CallGetCurrentPlayer()
    {
        NfLog.Log("calling GetCurrentPlayer");
        var player = NetflixPlayerIdentity.GetCurrentPlayer();
        if (player == null)
        {
            ShowDebugText("Current player: null");
            NfLog.Log("Current player: null");
        }
        else
        {
            ShowDebugText("Current player: playerId=" + player.playerId + ", handle=" + player.handle);
            NfLog.Log("Current player: playerId=" + player.playerId + ", handle=" + player.handle);
        }
    }

    void CallGetPlayerIdentities()
    {
        var ids = new List<string>
        {
            "33HJ7FW56RHXTBHSWXYC2A7GT4",
            "P5EVVWZ5GNE2RNGVKYCSGBPZUU",
            "OTRHYU65SREXNEFGRLVZAAIYB4",
            "notfound"
        };

        NfLog.Log("calling GetPlayerIdentities " + string.Join(", ", ids.ToArray()));
        var resultTask = NetflixPlayerIdentity.GetPlayerIdentities(ids);
        var continuation = resultTask.ContinueWith(task =>
        {
            var result = task.Result;
            var sb = new StringBuilder();

            if (result.status != RequestStatus.Ok)
            {
                NfLog.Log("Status: " + result.status);
                sb.Append("GetPlayerIdentities() returned status=" + result.status);
                if (result.description != null)
                {
                    sb.Append(", description=" + result.description);
                }
            }
            else if (result.identities != null)
            {
                NfLog.Log("Got result.identities");
                var players = result.identities;
                sb.Append("GetPlayerIdentities() returned [ ");
                foreach (string id in ids)
                {
                    sb.Append(id + ": ");
                    if (players[id].status != PlayerIdentityStatus.Ok) {
                        sb.Append("error=" + players[id].status);
                    }
                    else if (players[id].playerIdentity != null)
                    {
                        sb.Append("playerId=" + players[id].playerIdentity.playerId);
                        sb.Append(", handle=" + players[id].playerIdentity.handle);
                    }
                    else
                    {
                        sb.Append("UNEXPECTED null playerIdentity");
                    }
                    sb.Append("\n");
                }
                sb.Append(" ]");
            }
            else
            {
                NfLog.Log("Bug! result.identities is null when status is Ok");
                sb.Append("Unexpected! result.identities is null when status is Ok");
            }
            ShowDebugText(sb.ToString());
        });
        continuation.Wait();
    }

    public void ShowLeaderboard()
    {
        leaderboardsPanel.SetActive(true);
    }

    private void ShowAchievements()
    {
        achievementsPanel.SetActive(true);
    }

    public void CallInGameEventsListener()
    {
        ShowDebugText("sending mulitple inGameEvents (Start, FirstTimeUserExperienceStart, FirstTimeUserExperienceStepComplete, ProgressCheckpointComplete, GameComplete, Custom)");
        var startGame = new NewGameStart();
        NfLog.Log($"Sending game.inGame.NewGameStart - {startGame.ToJson()}");
        NetflixSdk.LogInGameEvent(startGame);
        var ftueStart = new FirstTimeUserExperienceStart();
        NfLog.Log($"Sending game.inGame.FTUEStart - {ftueStart.ToJson()}");
        NetflixSdk.LogInGameEvent(ftueStart);
        var ftueStepComplete = new FirstTimeUserExperienceStepComplete(1, "TestStep", "This is just a Test step.");
        NfLog.Log($"Sending game.inGame.FTUEStepComplete - {ftueStepComplete.ToJson()}");
        NetflixSdk.LogInGameEvent(ftueStepComplete);
        var ftueComplete = new FirstTimeUserExperienceComplete();
        NfLog.Log($"Sending game.inGame.FTUEComplete - {ftueComplete.ToJson()}");
        NetflixSdk.LogInGameEvent(ftueComplete);
        var checkpoint = new ProgressCheckpointComplete(1, "TestCheckpoint", "TEST", "A Test checkpoint");
        NfLog.Log($"Sending game.inGame.ProgressCheckpointComplete - {checkpoint.ToJson()}");
        NetflixSdk.LogInGameEvent(checkpoint);
        var complete = new GameComplete();
        NfLog.Log($"Sending game.inGame.GameComplete - {complete.ToJson()}");
        NetflixSdk.LogInGameEvent(complete);
        var customEvent = new Custom("game.inGame.Action", @"{
            ""actionName"":""dummy"",
            ""actionReceiver"":""no_one"",
            ""actionOutcome"":""success"",
            ""count"":1
        }");
        NfLog.Log($"Sending game.inGame.Action - {customEvent.ToJson()}");
        NetflixSdk.LogInGameEvent(customEvent);
        var failureEvent = new Custom("inGame.Failure", @"{
            ""failureType"":""puzzle"",
            ""failureName"":""puzzle_01"",
            ""failureOutcome"":""failed""
        }");
        NfLog.Log($"Sending inGame.Failure - {failureEvent.ToJson()}");
        NetflixSdk.LogInGameEvent(failureEvent);
        var exception = new Custom("badJsonEvent", "{{}");
        NfLog.Log("Sending badJsonEvent {{}");
        NetflixSdk.LogInGameEvent(exception);
    }

    public void GetSdkVersion()
    {
        var version = NetflixSdk.GetSdkVersion();
        ShowDebugText("SDK version is " + version);
    }

    private void CheckSupportedFeatures()
    {
        var builder = new StringBuilder();
        builder.Append("TODO");
        ShowDebugText(builder.ToString());
    }
}
