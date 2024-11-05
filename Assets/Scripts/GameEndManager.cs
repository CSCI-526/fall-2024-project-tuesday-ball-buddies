using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using System.Text.Json;

public class GameEndManager : MonoBehaviour
{
    private StageTimeManager stageTimeManager;
    private FirestoreApiManager firestoreApiManager;
    private BallControl ballControl;

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
    }

    public bool GetIfGameEnded(){
        return isGameEnded;
    }

    public void OnSubmit()
    {
        string playerName = inputField.text;

        string jsonData = $"{{ \"player\": \"{playerName}\", \"star\": \"{starCount}\", \"time\": \"{timeSpent}\" }}";

        firestoreApiManager.UploadAllWrap(jsonData);

        submitButton.interactable = false;
        buttonText.text = "Submitted";
    }

    public void OnNext()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("Beta-Bridge");
    }

    public void OnRestart()
    {
        endPanel.SetActive(false);
        submitButton.interactable = true;
        buttonText.text = "Submit Record";
        isGameEnded = false;
        stageTimeManager.ResetTimestamp();

        // ballControl.HandleRestart();
    }

    public void HandleGameEnd()
    {
        isGameEnded = true;

        List<string> timeList = stageTimeManager.GetCheckpointTime();

        timeSpent = timeList.Last();
        starCount = StarControl.starCount;

        star.text = $"Star: {starCount}";
        time.text = $"Time: {timeSpent}";

        // Show the end game panel
        if (endPanel != null)
            endPanel.SetActive(true);

        string jsonParam = $"{{ \"star_count\": \"{starCount}\", \"stage_level\": \"1\" }}";
        firestoreApiManager.GetLeaderboardWrap(jsonParam);

    }
}
