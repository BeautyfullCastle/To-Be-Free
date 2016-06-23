using System;

[Serializable]
public class ItemData : IData
{
    public int index;
    public string name;
    public string startTime;
    public string duration;
    public string restore;
    public int[] effectIndexList;
    public int[] amountList;
    public int price;
    public string tag;
}
