using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System;

public class FirestoreApiManager : MonoBehaviour
{
    private LeaderboardManager leaderboardManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void GetLeaderboardWrap(string sceneName, string sessionID)
    {
        StartCoroutine(GetLeaderboard(sceneName, sessionID));
    }
    IEnumerator GetLeaderboard(string sceneName, string sessionID)
    {
        string arguments = $"scene_name={sceneName}&session_id={sessionID}";
        using (UnityWebRequest www = UnityWebRequest.Get($"https://us-central1-ball-buddy-439019.cloudfunctions.net/get_leaderboard?{arguments}"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                leaderboardManager = FindObjectOfType<LeaderboardManager>();

                string json = www.downloadHandler.text;
                if (leaderboardManager != null)
                    leaderboardManager.PopulateBoard(json);
                else
                    Debug.Log("leaderboard is null");
            }
        }
    }

    public void UploadRecordWrap(string sceneName, string sessionID, string jsonData)
    {
        StartCoroutine(UploadRecord(sceneName, sessionID, jsonData));
    }
    IEnumerator UploadRecord(string sceneName, string sessionID, string jsonData)
    {
        string arguments = $"scene_name={sceneName}&session_id={sessionID}";
        using (UnityWebRequest www = UnityWebRequest.Post($"https://us-central1-ball-buddy-439019.cloudfunctions.net/firestore_manager?{arguments}", jsonData, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Data upload complete!");
            }
        }
    }

    public void UploadCheckpointWrap(string sceneName, string sessionID, string timeList)
    {
        StartCoroutine(UploadCheckpoint(sceneName, sessionID, timeList));
    }
    IEnumerator UploadCheckpoint(string sceneName, string sessionID, string timeList)
    {
        string arguments = $"scene_name={sceneName}&session_id={sessionID}";
        using (UnityWebRequest www = UnityWebRequest.Post($"https://us-central1-ball-buddy-439019.cloudfunctions.net/upload_time?{arguments}", timeList, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Data upload complete!");
            }
        }
    }

    public void UploadCollectedStarWrap(string collectedStarList)
    {
        StartCoroutine(UploadCollectedStar(collectedStarList));
    }

    IEnumerator UploadCollectedStar(string collectedStarList)
    {
        // Create the request
        using (UnityWebRequest www = UnityWebRequest.Post("https://upload-collectedstar-814677926917.us-central1.run.app/upload_collectedstar", collectedStarList, "application/json"))
        {
            // Set the content type
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                //Debug.Log("Star data upload complete!");
                string responseText = www.downloadHandler.text;
                Debug.Log($"Server response: {responseText}");
            }
        }
    }

    public void UploadTimeWithReplayWrap(string sceneName, string sessionID, string performance)
    {
        StartCoroutine(UploadTimeWithReplay(sceneName, sessionID, performance));
    }

    IEnumerator UploadTimeWithReplay(string sceneName, string sessionID, string performance)
    {
        string arguments = $"scene_name={sceneName}&session_id={sessionID}";
        // Create the request
        using (UnityWebRequest www = UnityWebRequest.Post($"https://track-replay-performance-814677926917.us-central1.run.app/track_performance?{arguments}", performance, "application/json"))
        {
            // Set the content type
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log($"Server response: {responseText}");
            }
        }
    }

    public void IsPlayerPerformanceTrackedWrap(string sceneName, string sessionID, Action<bool> callback)
    {
        StartCoroutine(IsPlayerPerformanceTracked(sceneName, sessionID, callback));
    }

    IEnumerator IsPlayerPerformanceTracked(string sceneName, string sessionID,  Action<bool> callback)
    {
        string arguments = $"scene_name={sceneName}&session_id={sessionID}";
        // Create the request
        using (UnityWebRequest www = UnityWebRequest.Post($"https://is-player-tracked-814677926917.us-central1.run.app/is_player_tracked?{arguments}","", "application/json"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                callback(false); // Return false if request fails
            }
            else
            {
                // Parse the server response
                string responseText = www.downloadHandler.text;
                Debug.Log($"Server response: {responseText}");
                
                try
                {
                    // Deserialize JSON response
                    TrackingResponse response = JsonUtility.FromJson<TrackingResponse>(responseText);
                    callback(response.tracked); // Return true if tracked, false otherwise
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse server response: " + e.Message);
                    callback(false); // Return false if parsing fails
                }
            }
        }
    }
}
