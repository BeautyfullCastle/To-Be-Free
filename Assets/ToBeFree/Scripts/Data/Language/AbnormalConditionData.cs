using System;

namespace Language
{
	[Serializable]
	public class AbnormalConditionData : IData
	{
		public int index;
		public string name;
		public string script;
	}
}