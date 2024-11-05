using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SerializableList<T>
{
    public List<T> list;
}

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
        timeList.list.Add(timer.getTime());
    }

    public void ResetTimestamp()
    {
        timeList = new SerializableList<string>();
    }
    public List<string> GetCheckpointTime()
    {
        return timeList.list;
    }

}
