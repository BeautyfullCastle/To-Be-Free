using System;
using System.IO;
using UnityEngine;

[Serializable]
public class EventData : IData
{
    public int index;
    public string actionType;
    public string region;
    public string stat;
    public string difficulty;
    public string script;
    public int resultIndex;
    public int[] selectIndexList;
}

public class EventDataList
{
    public EventData[] dataList;

    public EventDataList(string file)
    {
        StreamReader reader = new StreamReader(file);
        string json = reader.ReadToEnd();

        var dataList = JsonUtility.FromJson<EventDataList>(json);
        this.dataList = dataList.dataList;
    }
}
