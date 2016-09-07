using System;

[Serializable]
public class CharacterData : IData
{
    public int index;
    public string name;
    public string script;
    public int HP;
    public int mental;
    public int strength;
    public int agility;
    public int observation;
    public int bargain;
    public int patience;
    public int luck;
    public int startMoney;
    public int startInven;
    public string startCity;
    public int[] itemIndex; 
}
