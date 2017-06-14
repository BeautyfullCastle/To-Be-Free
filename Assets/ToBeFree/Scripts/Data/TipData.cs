using System;

[Serializable]
public class TipData : IData
{
	public int index;
	public string title;
	public string timing;
	public string script;
	public string sprite;
	public int nextIndex;
}