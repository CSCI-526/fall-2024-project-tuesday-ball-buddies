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
    private BallControl ballControl;
    private StageTimeManager stageTimeManager;
    private FirestoreApiManager firestoreApiManager;
    private CanvasManager canvasManager;

    /*public Button submitButton;
    public TMP_Text submitButtonText;*/

    private bool isGameEnded = false;
    private string timeSpent;
    private int starCount; 
    private Scene currentScene;

    void Start()
    {
        stageTimeManager = FindObjectOfType<StageTimeManager>();
        firestoreApiManager = FindObjectOfType<FirestoreApiManager>();
        ballControl = FindObjectOfType<BallControl>();
        canvasManager = FindObjectOfType<CanvasManager>();
        currentScene =  SceneManager.GetActiveScene();
        
    }

    public bool IsGameEnded(){
        return isGameEnded;
    }

    public void OnSubmit()
    {
        string playerName = canvasManager.EndPanelGetPlayerName();

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
        canvasManager.EndPanelOnSubmit();
    }

    public void OnNext()
    {
        Time.timeScale = 1;
        StarControl.starCount = 0; //clear starCount back to 0
        stageTimeManager.ResetTimestamp();

        // Get the index of the current scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index of the next scene
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the valid range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            canvasManager.CompleteUIShow();
    }

    public void OnRestart()
    {
        isGameEnded = false;

        //track player (only world 1 & 2) to see if he plays better
        if (currentScene.name != "Tutorial")
        {
            TrackPlayerPerformance();
        }
        
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

        canvasManager.EndPanelHandleGameEnd(timeSpent, starCount);   

        /* Create leaderboard data */
        /* !TODO: make sure later on the stage_level is the name of current scene */

        /* we don't have to record the data of tutorial */
        if (currentScene.name == "Tutorial")
            return;

        firestoreApiManager.GetLeaderboardWrap(currentScene.name, SessionManager.sessionID);
        string timeListStr = JsonUtility.ToJson(timeList);
        firestoreApiManager.UploadCheckpointWrap(currentScene.name, SessionManager.sessionID, timeListStr);

        //check if this player performance is tracked
        firestoreApiManager.IsPlayerPerformanceTrackedWrap(currentScene.name, SessionManager.sessionID, (isTracked) =>
        {
            if (isTracked)
            {
                Debug.Log("Player is tracked.");
                TrackPlayerPerformance();
            }
            else
            {
                Debug.Log("Player is not tracked.");
            }
        });
    }

    private void TrackPlayerPerformance()
    {
        long timeSpentInMilli = ConvertToMilliseconds(timeSpent);
        Debug.Log($"Tracked Player {SessionManager.sessionID} uses {timeSpentInMilli} milliseconds");

        //upload performance
        PerformanceData data = new PerformanceData
        {
            timeSpentInMilli = timeSpentInMilli,
            collectedStar = StarControl.starCount
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(data);
        firestoreApiManager.UploadTimeWithReplayWrap(currentScene.name, SessionManager.sessionID, jsonData);
    }

    private long ConvertToMilliseconds(string time)
    {
        TimeSpan timeSpan;
        
        // Parse the time string into a TimeSpan object
        if (TimeSpan.TryParseExact(time, @"mm\:ss\:fff", null, out timeSpan))
        {
            long milliseconds = (long)timeSpan.TotalMilliseconds;
            Debug.Log($"ConvertToMilliseconds: {time} is converted to {milliseconds} milliseconds");
            return (long)(timeSpan.TotalMilliseconds);
        }
        else
        {
            Debug.LogError($"Invalid time format. {time} Expected format: mm:ss:fff");
            return -1;
        }
    }
}
