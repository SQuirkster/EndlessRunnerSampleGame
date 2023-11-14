using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Netflix;
using System.Text;

public class AchievementsPanel : MonoBehaviour
{
    public GameObject textResult;

    private string achievementName;
    private List<Transform> resultRows = new List<Transform>();

 	void Start()
    {
		InputField achievementInputField = GameObject.Find("achievementNameInputField").GetComponent<InputField>();
		achievementInputField.onValueChanged.AddListener(OnAchievementNameChanged);
	}

    public void OnAchievementNameChanged(string achievementName)
    {
        this.achievementName = achievementName;
    }

    public void OnUnlockAchievement()
    {
        Netflix.Achievements.UnlockAchievement(achievementName);

        ShowResultMessage("Achievement unlock fired and forgotten!");
    }

    public async void ListAchievements()
    {
        var result = await Netflix.Achievements.GetAchievements();
        RenderEntries(result);
    }

    public void ShowAchievementsPanel()
    {
        Netflix.Achievements.ShowAchievementsPanel();
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    private void ClearResultsTable()
    {
        foreach (Transform existingRow in resultRows)
        {
            Destroy(existingRow.gameObject);
        }

        resultRows = new List<Transform>();
        textResult.SetActive(false);
    }

    private void ShowResultMessage(string message)
    {
        ClearResultsTable();
        textResult.SetActive(true);
        textResult.GetComponent<Text>().text = message;
    }

    private void RenderEntries(Netflix.AchievementsResult result)
    {
        ClearResultsTable();

        if (result.status != Netflix.AchievementStatus.Ok)
        {
            ShowResultMessage("Error result, status: " + result.status);
            return;
        }

        for (int i = 0; i < result.achievements.Count; i++)
        {
            Transform newRow = DrawEntry(i, result.achievements[i]);
        }
    }

    private Transform DrawEntry(int i, Achievement entry)
    {
        var achievementsPanel = GameObject.Find("AchievementsPanelContent");
        var rowTemplate = GameObject.Find("ResultRowTemplate");

        Transform newRow = Instantiate(rowTemplate.transform, achievementsPanel.transform);
        newRow.gameObject.SetActive(true);

        resultRows.Add(newRow);

        RectTransform rowRectTransform = newRow.GetComponent<RectTransform>();
        rowRectTransform.anchoredPosition = new Vector2(0, -700 - (75 * i));

        resultRows.Add(newRow);

        newRow.Find("Name").GetComponent<Text>().text = entry.name;
        newRow.Find("isLocked").GetComponent<Text>().text = "" + entry.isLocked;

        return newRow;
    }
}
