using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class DeathAnalysis : MonoBehaviour

{
    public void uploadDeathCause(string userID, string enemyname, string lastKnownLevel,  string cause, Vector3 lastKnownPosition)
    {
        userID = SessionManager.sessionID;
        StartCoroutine(UploadCause(userID, enemyname, lastKnownLevel, cause, lastKnownPosition));
    }
    

    IEnumerator UploadCause(string userID, string enemyname, string lastKnownLevel, string cause, Vector3 lastKnownPosition)
    {
        Debug.Log("Uploading death cause: " + cause);

        // Create an instance of UploadDeathData with the required fields
        UploadDeathData uploadDeathData = new UploadDeathData
        {
            enemyname = enemyname,
            scenename = SceneManager.GetActiveScene().name,
            userID = userID,
            level = lastKnownLevel,
            reason = cause,
            x = lastKnownPosition.x.ToString(),
            y = lastKnownPosition.y.ToString(),
            z = lastKnownPosition.z.ToString()
        };
        

        // Convert the data to JSON format
        string causeData = JsonUtility.ToJson(uploadDeathData);
        Debug.Log(causeData);

        // Set up the web request
        using (UnityWebRequest www = new UnityWebRequest("https://update-death-cause-814677926917.us-central1.run.app", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(causeData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for response
            yield return www.SendWebRequest();

            // Handle the response
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Data upload complete! " + cause);
            }
        }
    }
}