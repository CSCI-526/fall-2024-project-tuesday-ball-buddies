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


    public void GetLeaderboardWrap(string parameter)
    {
        StartCoroutine(GetLeaderboard(parameter));
    }
    IEnumerator GetLeaderboard(string parameter)
    {
        // string timeData = JsonUtility.ToJson(parameter);
        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-ball-buddy-439019.cloudfunctions.net/get_leaderboard", parameter, "application/json"))
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

    public void UploadAllWrap(string jsonData)
    {
        StartCoroutine(UploadAll(jsonData));
    }
    IEnumerator UploadAll(string jsonData)
    {
        // string timeData = JsonUtility.ToJson(jsonData);
        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-ball-buddy-439019.cloudfunctions.net/firestore_manager", jsonData, "application/json"))
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

    public void UploadCheckpointWrap(string timeList)
    {
        StartCoroutine(UploadCheckpoint(timeList));
    }
    IEnumerator UploadCheckpoint(string timeList)
    {
        // string timeData = JsonUtility.ToJson(timeList);
        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-ball-buddy-439019.cloudfunctions.net/firestore_manager", timeList, "application/json"))
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
        using (UnityWebRequest www = UnityWebRequest.Post("https://upload-collectedstar-814677926917.us-central1.run.app/upload_collectedstar", "", "application/json"))
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
}
