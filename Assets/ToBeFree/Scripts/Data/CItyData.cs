using System;

[Serializable]
public class CityData : IData
{
    public int index;
    public string name;
    public string size;
    public string area;
    public int[] itemIndexList;
    public int[] workingMoneyRange;
    public int[] neighborList;
}