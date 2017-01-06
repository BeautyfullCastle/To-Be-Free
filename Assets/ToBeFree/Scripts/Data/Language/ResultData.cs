using System;

namespace Language
{
	[Serializable]
	public class ResultData : IData
	{
		public int index;
		public string successScript;
		public string failureScript;
	}
}
