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
