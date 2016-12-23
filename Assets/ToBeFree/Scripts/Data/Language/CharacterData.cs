using System;

namespace Language
{
	[Serializable]
	public class CharacterData : IData
	{
		public int index;
		public string name;
		public string script;
		public string skillScript;
	}
}