using System;

[Serializable]
public class QuestData : IData
{
    public int index;
    public string subjectType;
    public string objectType;
    public string comparisonOperator;
    public int compareAmount;
    public string actionType;
    public string region;
    public string stat;
    public string difficulty;
    public string script;
    public string failureScript;
    public int resultIndex;
    public int duration;
    public string uiName;
    public string uiConditionScript;
}