using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public GameObject mainUI, pausePanel, endPanel;
    private GameObject buttonPause, buttonPlay, buttonReplay, buttonReturn;
    private PauseManager pauseManager;
    private StarManager starManager;

    private void Start()
    {
        if (mainUI == null)
            throw new System.NullReferenceException("CanvasManager: 'mainUI' is not assigned in the inspector.");
        if (pausePanel == null)
            throw new System.NullReferenceException("CanvasManager: 'pausePanel' is not assigned in the inspector.");
        if (endPanel == null)
            throw new System.NullReferenceException("CanvasManager: 'endPanel' is not assigned in the inspector.");

        pauseManager = FindObjectOfType<PauseManager>();
        buttonPause = mainUI.transform.Find("Button-Pause").gameObject;
        buttonPlay = mainUI.transform.Find("Button-Play").gameObject;
        buttonReplay = pausePanel.transform.Find("Panel (1)/Button-Replay").gameObject;
        buttonReturn = pausePanel.transform.Find("Panel (1)/Button-Return").gameObject;
        buttonPause.GetComponent<Button>().onClick.AddListener(() => pauseManager.TogglePause());
        buttonPlay.GetComponent<Button>().onClick.AddListener(() => pauseManager.TogglePause());
        buttonReplay.GetComponent<Button>().onClick.AddListener(() => pauseManager.RestartLevel());
        buttonReturn.GetComponent<Button>().onClick.AddListener(() => pauseManager.ReturnToMainMenu());
    }

    public void ScoreboardPanelGetToggles(out List<Toggle> level1Toggles, out List<Toggle> level2Toggles, out List<Toggle> level3Toggles)
    {
        level1Toggles = new List<Toggle>();
        level2Toggles = new List<Toggle>();
        level3Toggles = new List<Toggle>();

        if (SceneManager.GetActiveScene().name == "Tutorial")
            return;

        GameObject scoreboardPanel = mainUI.transform.Find("Panel-Scoreboard").gameObject;
        Toggle[] toggles = scoreboardPanel.GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            if (toggle.transform.parent.name == "Row1")
                level1Toggles.Add(toggle);
            else if (toggle.transform.parent.name == "Row2")
                level2Toggles.Add(toggle);
            else if (toggle.transform.parent.name == "Row3")
                level3Toggles.Add(toggle);
        }
    }

    public void PausePanelTogglePause(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
        buttonPause.SetActive(!isPaused);
        buttonPlay.SetActive(isPaused);
    }

    public void EndPanelHandleGameEnd(string timeSpent, int starCount)
    {
        mainUI.SetActive(false);
        endPanel.SetActive(true);

        TMP_Text star = endPanel.transform.Find("Panel/PlayerRecord/Record/Star").GetComponent<TMP_Text>();
        TMP_Text time = endPanel.transform.Find("Panel/PlayerRecord/Record/Time").GetComponent<TMP_Text>();
        star.text = $"Star: {starCount}";
        time.text = $"Time: {timeSpent}";
    }

    public string EndPanelGetPlayerName()
    {
        TMP_InputField inputField = endPanel.transform.Find("Panel/PlayerRecord/Panel/InputField (TMP)").GetComponent<TMP_InputField>();
        return inputField.text;
    }

    public void EndPanelOnSubmit()
    {
        GameObject submitButton = endPanel.transform.Find("Panel/PlayerRecord/Panel/Button").gameObject;
        submitButton.GetComponent<Button>().interactable = false;
        submitButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "Submitted";

        GameObject leaderboardPanel = endPanel.transform.Find("Panel/Leaderboard/Panel").gameObject;
        leaderboardPanel.GetComponent<LeaderboardManager>().ClearGrid();
        leaderboardPanel.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        leaderboardPanel.transform.Find("StatusText").gameObject.SetActive(true);

        GameObject currentPlayerRankPanel = endPanel.transform.Find("Panel/Leaderboard/CurrentPlayerRank").gameObject;
        currentPlayerRankPanel.SetActive(true);
    }
}
