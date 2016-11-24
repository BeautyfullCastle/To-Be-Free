using System;

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