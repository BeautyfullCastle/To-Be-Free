using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class AbnormalConditionData : IData
{
    public int index;
    public string name;
    public string spawnCondition;
    public int effectIndex;
    public int amount;
    public string isRestore;
    public string stack;
    public string startTime;
    public string duration;
    public string script;
    public string isBody;
    public string isPositive;
}

public class AbnormalConditionDataList
{
    private List<AbnormalConditionData> abnormalConditionDataList;

    public AbnormalConditionDataList(string file)
    {
        StreamReader reader = new StreamReader(file);
        string json = reader.ReadToEnd();

        var dataList = JsonUtility.FromJson<AbnormalConditionDataList>(json);
        this.abnormalConditionDataList = dataList.abnormalConditionDataList;
    }

    public List<AbnormalConditionData> List
    {
        get
        {
            return abnormalConditionDataList;
        }

        set
        {
            abnormalConditionDataList = value;
        }
    }
}