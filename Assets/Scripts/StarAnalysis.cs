using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class StarCollectionData
{
    public int stage1;
    public int stage2;
    public int stage3;
}

public class StarAnalysis : MonoBehaviour
{
    private StarCollectionData starData;

    private void Start()
    {
        // Initialize star data
        starData = new StarCollectionData
        {
            stage1 = 0,
            stage2 = 0,
            stage3 = 0
        };
    }

    // Call this method when a star is collected
    public void RecordStar(int stageIndex)
    {
        // Log the current state of starData before modification
        Debug.Log($"Current Star Data: Stage 1: {starData.stage1}, Stage 2: {starData.stage2}, Stage 3: {starData.stage3}");

        // Log the stage index received
        Debug.Log($"Stage Index Received: {stageIndex}");

        switch (stageIndex)
        {
            case 1:
                starData.stage1++;
                Debug.Log($"Stage 1 incremented: {starData.stage1}");
                break;
            case 2:
                starData.stage2++;
                Debug.Log($"Stage 2 incremented: {starData.stage2}");
                break;
            case 3:
                starData.stage3++;
                Debug.Log($"Stage 3 incremented: {starData.stage3}");
                break;
            default:
                Debug.LogWarning("Invalid stage index!");
                break;
        }

        // Log the updated state of starData after modification
        Debug.Log($"Updated Star Data: Stage 1: {starData.stage1}, Stage 2: {starData.stage2}, Stage 3: {starData.stage3}");
    }

    // Call this method when the game is finished
    public void UploadStarData()
    {
        StartCoroutine(UploadData());
    }

    private IEnumerator UploadData()
    {
        // Prepare the data to upload
        var uploadData = new
        {
            type = "star",  // Specify the type as "star"
            data = starData // Use the starData object
        };

        string jsonData = JsonUtility.ToJson(uploadData);
        
        // Create the request
        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-ball-buddy-439019.cloudfunctions.net/firestore_manager", jsonData, "application/json"))
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
                Debug.Log("Star data upload complete!");
            }
        }
    }
}
