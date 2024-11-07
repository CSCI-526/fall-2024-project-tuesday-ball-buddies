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

    private string timeSpent;
    private int starCount; 

    private Scene currentScene;
    void Start()
    {
        stageTimeManager = FindObjectOfType<StageTimeManager>();
        firestoreApiManager = FindObjectOfType<FirestoreApiManager>();
        ballControl = FindObjectOfType<BallControl>();
        hudManager = FindObjectOfType<HUDManager>();
        currentScene =  SceneManager.GetActiveScene();
        
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

        firestoreApiManager.UploadRecordWrap(currentScene.name, SessionManager.sessionID, jsonData);

        submitButton.interactable = false;
        buttonText.text = "Submitted";
    }
    
    /* 
        !TODO: 
            1. make the OnNext function accept argument as next currentS            
            2. decide the if gameplay_id and scene is the arguments of request (as an identifier in firestore)
            3. decide the design of firestore
            4. modify cloud functions to comply with new design
            5. make sure the logic when game ended
                -> submit analytic info (stage_time, death_cause, etc)
                -> get leaderboard data
                -> open the panel 
                -> submit player info

    */

    public void OnNext()
    {
        Time.timeScale = 1; 
        StarControl.starCount = 0; //clear starCount back to 0
        SceneManager.LoadScene("Beta-Bridge");
        hudManager.setGameWon(false);
        stageTimeManager.ResetTimestamp();
        Debug.Log("hudManager.gameWon" + hudManager.getGameWon());
    }

    public void OnRestart()
    {
        endPanel.SetActive(false);
        submitButton.interactable = true;
        buttonText.text = "Submit Record";
        isGameEnded = false;
        
        StarControl.starCount = 0; //clear starCount back to 0

        stageTimeManager.ResetTimestamp();
        SceneManager.LoadScene(currentScene.name);
        ballControl.HandleRestart();

        Time.timeScale = 1;
    }

    public void HandleGameEnd()
    {
        isGameEnded = true;

        SerializableList<string> timeList = stageTimeManager.GetCheckpointList();

        timeSpent = timeList.list.Last();
        starCount = StarControl.starCount;

        star.text = $"Star: {starCount}";
        time.text = $"Time: {timeSpent}";

        if (endPanel != null)
            endPanel.SetActive(true);

        /* Create leaderboard data */
        /* !TODO: make sure later on the stage_level is the name of current scene */

        /* we don't have to record the data of tutorial */
        if (currentScene.name == "tutorial")
            return;

        firestoreApiManager.GetLeaderboardWrap(currentScene.name, SessionManager.sessionID);
        string timeListStr = JsonUtility.ToJson(timeList);
        firestoreApiManager.UploadCheckpointWrap(currentScene.name, SessionManager.sessionID, timeListStr);
    }
}
