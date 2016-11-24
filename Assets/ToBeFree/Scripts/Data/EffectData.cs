using System;

[Serializable]
public class EffectData : IData
{
    public int index;
    public string subjectType;
    public string verbType;
    public string objectType;
    
    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.

    //index subject verb object
}