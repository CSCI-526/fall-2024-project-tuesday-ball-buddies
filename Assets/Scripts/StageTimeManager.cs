using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class StageTimeManager : MonoBehaviour
{
    private Timer timer;
    //private List<string> timeList = new List<string>();
    [SerializeField] private SerializableList<string> timeList;

    // Start is called before the first frame update
    void Start()
    {
        timer = FindObjectOfType<Timer>();
    }

    public void EnterCheckpoint()
    {
        timeList.list.Add(timer.getTime());
    }

    public void GameEnd()
    {
        timeList.list.Add(timer.getTime());

        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        UploadTimeSpentData<string> uploadTimeSpentData = new UploadTimeSpentData<string>
        {
            type = "time_spent",
            data = timeList
        };
        
        string timeData = JsonUtility.ToJson(uploadTimeSpentData);
        using (UnityWebRequest www = UnityWebRequest.Post("https://us-central1-ball-buddy-439019.cloudfunctions.net/firestore_manager", timeData, "application/json"))
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
