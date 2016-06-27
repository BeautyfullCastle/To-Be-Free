using System;

[Serializable]
public class RegionProbabilityData : IData
{
    public int index;
    // { AREA, CITY, ALL }
    public int[] valueList;
}

[Serializable]
public class StatProbabilityData : IData
{
    public int index;
    public string actionType;
    // { STRENGTH, AGILITY, OBSERVATION, BARGAIN, PATIENCE, LUCK }
    public int[] valueList;
}
