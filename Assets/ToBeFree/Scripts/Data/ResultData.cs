using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ResultData : IData
{
    public int index;
    public string stat;
    public string successScript;
    public int[] successEffectIndexList;
    public int[] successEffectValueList;
    public string failureScript;
    public int[] failureEffectIndexList;
    public int[] failureEffectValueList;
}