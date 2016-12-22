using System;

namespace Language
{
	[Serializable]
	public class ItemData : IData
	{
		public int index;
		public string name;
		public string script;
	}
}