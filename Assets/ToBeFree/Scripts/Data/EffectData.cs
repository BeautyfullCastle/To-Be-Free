﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EffectData : IData
{
    public int index;
    public string subjectType;
    public string verbType;
    public string objectType;
    
    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.

    //index subject verb object
}

public class EffectDataList
{
    public EffectData[] dataList;

    public EffectDataList(string file)
    {
        StreamReader reader = new StreamReader(file);
        string json = reader.ReadToEnd();

        var dataList = JsonUtility.FromJson<EffectDataList>(json);
        this.dataList = dataList.dataList;
    }
}