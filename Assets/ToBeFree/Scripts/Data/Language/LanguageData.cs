using System;

namespace Language
{
	[Serializable]
	public class LanguageData : IData
	{
		public int index;
		public string key;
		public string script;
	}
}