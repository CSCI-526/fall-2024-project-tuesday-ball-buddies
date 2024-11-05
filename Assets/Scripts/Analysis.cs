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