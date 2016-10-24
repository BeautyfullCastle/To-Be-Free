using System;

[Serializable]
public class CharacterData : IData
{
    public int index;
    public string name;
    public string script;
    public int HP;
    public int strength;
    public int agility;
    public int concentration;
    public int talent;
    public int patience;
    public int luck;
    public int startMoney;
    public int startInven;
    public string startCity;
    public int[] itemIndex; 
}