using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SerializableList<T>
{
    public List<T> list;
}

[System.Serializable]
public class UploadTimeSpentData<T>
{
    public string type;
    public SerializableList<T> data;
}

[System.Serializable]
public class UploadDeathData
{
    public string enemyname;
    public string scenename;
    public string userID;
    public string level;
    public string reason;
    public string x;
    public string y;
    public string z;
}


[System.Serializable]
public class PlayerData
{
    public string player;
    public int star;
    public string time;
}

[System.Serializable]
public class PerformanceData
{
    public long timeSpentInMilli;
    public int collectedStar;
}

[System.Serializable]
public class TrackingResponse
{
    public bool tracked;
}

[System.Serializable]
public class StarCollectionData
{
    public List<int> collectedStarList;
}
