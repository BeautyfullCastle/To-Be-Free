using System;
using System.IO;
using UnityEngine;

[Serializable]
public class EventData : IData
{
    public int index;
    public string actionType;
    public string difficulty;
    public string script;
    public int resultIndex;
    public int[] selectIndexList;
}

