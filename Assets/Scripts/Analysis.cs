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
    public string type;
    public string data;
}


[System.Serializable]
public class PlayerData
{
    public string player;
    public int star;
    public string time;
}


[System.Serializable]
public class LeaderboardData
{
    public int star_count;
    public string stage_level;
}