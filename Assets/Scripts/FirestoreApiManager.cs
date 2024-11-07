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
}
