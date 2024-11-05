using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    private StageTimeManager stageTimeManager;
    private FirestoreApiManager firestoreApiManager;
    private BallControl ballControl;
    private HUDManager hudManager; 
    public GameObject endPanel;
    public TMP_Text star;
    public TMP_Text time;
    public TMP_InputField inputField;
    public Button submitButton;
    public TMP_Text buttonText;

    private bool isGameEnded = false;
    // stop the game
    // display won
    // leader board

    // replay button

    private string timeSpent;
    private int starCount;

    // Start is called before the first frame update
    void Start()
    {
        stageTimeManager = FindObjectOfType<StageTimeManager>();
        firestoreApiManager = FindObjectOfType<FirestoreApiManager>();
        ballControl = FindObjectOfType<BallControl>();
        hudManager = FindObjectOfType<HUDManager>();
    }

    public bool GetIfGameEnded(){
        return isGameEnded;
    }

    public void OnSubmit()
    {
        string playerName = inputField.text;

        // Populate the data
        PlayerData data = new PlayerData
        {
            player = playerName,
            star = starCount,
            time = timeSpent
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(data);

        firestoreApiManager.UploadAllWrap(jsonData);

        submitButton.interactable = false;
        buttonText.text = "Submitted";
    }

    public void OnNext()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("Beta-Bridge");
        hudManager.setGameWon(false);
        Debug.Log("hudManager.gameWon" + hudManager.getGameWon());
    }

    public void OnRestart()
    {
        endPanel.SetActive(false);
        submitButton.interactable = true;
        buttonText.text = "Submit Record";
        isGameEnded = false;
        stageTimeManager.ResetTimestamp();

        SceneManager.LoadScene("Beta-Bridge");
        ballControl.HandleRestart();
    }

    public void HandleGameEnd()
    {
        isGameEnded = true;

        List<string> timeList = stageTimeManager.GetCheckpointTime();

        timeSpent = timeList.Last();
        starCount = StarControl.starCount;

        star.text = $"Star: {starCount}";
        time.text = $"Time: {timeSpent}";

        if (endPanel != null)
            endPanel.SetActive(true);

        // Create leaderboard data
        LeaderboardData leaderboardData = new LeaderboardData
        {
            star_count = starCount,
            stage_level = "1"
        };

        // Convert to JSON
        string jsonParam = JsonUtility.ToJson(leaderboardData);
        firestoreApiManager.GetLeaderboardWrap(jsonParam);
    }
}
