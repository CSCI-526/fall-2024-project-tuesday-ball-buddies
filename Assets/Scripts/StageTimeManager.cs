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

    public void AddTimestamp()
    {
        Debug.Log(timer.getTime());
        timeList.list.Add(timer.getTime());
        Debug.Log(timeList.list[0]);
    }

    public void ResetTimestamp()
    {
        timeList = new SerializableList<string>();
    }
    public SerializableList<string> GetCheckpointList()
    {
        return timeList;
    }

}
