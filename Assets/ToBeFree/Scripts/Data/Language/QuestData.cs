using System;

namespace Language
{
	[Serializable]
	public class QuestData : IData
	{
		public int index;
		public string script;
		public string failureScript;
		public string uiName;
		public string uiConditionScript;
	}
}