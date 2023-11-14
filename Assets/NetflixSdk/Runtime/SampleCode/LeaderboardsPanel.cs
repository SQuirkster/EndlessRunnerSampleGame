using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Netflix;
using System.Text;

public class LeaderboardsPanel : MonoBehaviour
{
    public string submitStatName;
    public string submitStatValue;

    public string leaderboardName;
    public string leaderboardValue;
    public string cursorValue;

    public GameObject textResult;

    private List<Transform> resultRows = new List<Transform>();

    void Start()
    {
        GameObject.Find("submitStatNameInputField").GetComponent<InputField>().text = "SAMPLE_STAT";
        GameObject.Find("submitStatValueInputField").GetComponent<InputField>().text = "10";

        GameObject.Find("GetEntriesName").GetComponent<InputField>().text = "SAMPLE_LEADERBOARD";
        GameObject.Find("GetEntriesCount").GetComponent<InputField>().text = "10";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onSubmitStatNameChanged(string statName)
    {
        this.submitStatName = statName;
    }

    public void onSubmitStatValueChanged(string statValue)
    {
        this.submitStatValue = statValue;
    }

    public async void onGetAggregatedStat()
    {
        var result = await Netflix.Stats.GetAggregatedStat(this.submitStatName);
        var builder = new StringBuilder();
        builder.Append("GetAggregatedStat status: " + result.status.ToString());
        if (result.aggregatedStat != null)
        {
            builder.Append("\n(")
                .Append(result.aggregatedStat.name)
                .Append(", ")
                .Append(result.aggregatedStat.value)
                .Append(")");
        }
        showResultMessage(builder.ToString());
    }

    public void onSubmitStat()
    {
        Netflix.Stats.SubmitStat(new Stats.StatItem(this.submitStatName, Convert.ToInt64(this.submitStatValue)));
        showResultMessage("Stat fired and forgotten!");
    }

    public async void onSubmitStatNow()
    {
        var result = await Netflix.Stats.SubmitStatNow(new Stats.StatItem(this.submitStatName, Convert.ToInt64(this.submitStatValue)));
        var builder = new StringBuilder();
        builder.Append("SubmitStatNow status: " + result.status.ToString())
               .Append("(")
               .Append((int)result.status)
               .Append(")");
        if (result.submittedStat != null)
        {
            builder.Append("\nsubmittedStat = (")
                .Append(result.submittedStat.name)
                .Append(", ")
                .Append(result.submittedStat.value)
                .Append(")");
        }

        if (result.aggregatedStat != null)
        {
            builder.Append("\naggregatedStat = (")
                .Append(result.aggregatedStat.name)
                .Append(", ")
                .Append(result.aggregatedStat.value)
                .Append(")");
        }
        showResultMessage(builder.ToString());
    }

    private void clearResultsTable()
    {
        foreach (Transform existingRow in resultRows)
        {
            Destroy(existingRow.gameObject);
        }

        resultRows = new List<Transform>();
        textResult.SetActive(false);
    }

    private void showResultMessage(string message)
    {
        clearResultsTable();
        textResult.SetActive(true);
        textResult.GetComponent<Text>().text = message;
    }

    private void renderEntries(Netflix.LeaderboardEntriesResult result)
    {
        clearResultsTable();

        if (result.status != Netflix.LeaderboardStatus.Ok)
        { 
            showResultMessage("Error result, status: " + result.status);
            return;
        }

        for (int i = 0; i < result.page.leaderboardEntries.Count; i++)
        {
            Transform newRow = drawEntry(i, result.page.leaderboardEntries[i]);

            if (i == 0 || i == result.page.leaderboardEntries.Count - 1)
            {
                var cursor = i == 0 ? result.page.startCursor : result.page.endCursor;
                newRow.Find("CursorButton").GetComponentInChildren<Text>().text = cursor;

                newRow.Find("CursorButton").GetComponent<Button>().onClick.AddListener(delegate
                {
                    GameObject.Find("GetEntriesCursor").GetComponent<InputField>().text = cursor;
                });
            }
            else
            {
                newRow.Find("CursorButton").GetComponent<Button>().gameObject.SetActive(false);
            }
        }
    }

    private Transform drawEntry(int i, Netflix.LeaderboardEntry entry)
    {
        var leaderboardPanel = GameObject.Find("LeaderboardsPanelContent");
        var rowTemplate = GameObject.Find("ResultRowTemplate");

        Transform newRow = Instantiate(rowTemplate.transform, leaderboardPanel.transform);
        newRow.gameObject.SetActive(true);

        resultRows.Add(newRow);

        RectTransform rowRectTransform = newRow.GetComponent<RectTransform>();
        rowRectTransform.anchoredPosition = new Vector2(0, -700 - (75 * i));

        resultRows.Add(newRow);

        var name = entry.playerIdentity.handle != null ? entry.playerIdentity.handle : entry.playerIdentity.playerId;

        newRow.Find("Name").GetComponent<Text>().text = name;
        newRow.Find("Rank").GetComponent<Text>().text = "" + entry.rank;
        newRow.Find("Pos").GetComponent<Text>().text = "" + entry.position;
        newRow.Find("Score").GetComponent<Text>().text = "" + entry.score;

        return newRow;

    }

    private void renderEntry(Netflix.LeaderboardEntryResult result)
    {
        clearResultsTable();

        if (result.status != Netflix.LeaderboardStatus.Ok)
        {

            showResultMessage("Error result, status: " + result.status);
            return;
        }

        drawEntry(0, result.entry);
    }

    private void renderInfo(Netflix.LeaderboardInfoResult result)
    {
        clearResultsTable();

        if (result.status != Netflix.LeaderboardStatus.Ok)
        {

            showResultMessage("Error result, status: " + result.status);
            return;
        }

        showResultMessage("Status: " + result.status + ", Count: " + result.leaderboardInfo.count + ", Name: " + result.leaderboardInfo.name);

    }

    public void onLeaderboardNameChanged(string name)
    {
        this.leaderboardName = name;
    }

    public void onLeaderboardValueChanged(string value)
    {
        this.leaderboardValue = value;
    }

    public void onCursorChanged(string value)
    {
        this.cursorValue = value;
    }


    public async void getTopEntries()
    {
        if (convertValue() < 0) { return; }

        var result = await Netflix.Leaderboards.GetTopEntries(leaderboardName, convertValue());
        renderEntries(result);
    }

    public void getMoreEntriesBeforeSubmit()
    {
        getMoreEntries(Netflix.FetchDirection.Before);
    }

    public void getMoreEntriesAfterSubmit()
    {
        getMoreEntries(Netflix.FetchDirection.After);
    }


    public async void getPlayerSurroundingEntries()
    {
        if (convertValue() < 0) { return; }

        var result = await Netflix.Leaderboards.GetEntriesAroundCurrentPlayer(leaderboardName, convertValue());
        renderEntries(result);
    }

    public async void getCurrentPlayerEntry()
    {
        var result = await Netflix.Leaderboards.GetCurrentPlayerEntry(leaderboardName);
        renderEntry(result);
    }

    public async void getInfo()
    {
        var result = await Netflix.Leaderboards.GetLeaderboardInfo(leaderboardName);
        renderInfo(result);
    }

    public async void getMoreEntries(Netflix.FetchDirection direction)
    {
        if(convertValue() < 0) { return; }

        var result = await Netflix.Leaderboards.GetMoreEntries(leaderboardName, convertValue(), cursorValue, direction);
        renderEntries(result);
    }

    private int convertValue()
    {
        try
        {
            return Convert.ToInt32(leaderboardValue);
        }
        catch
        {
            NfLog.Log("Invalid leaderboard count value " + leaderboardValue);
            return -1;
        }
    }

    public void exit()
    {
        this.gameObject.SetActive(false);
    }
}
