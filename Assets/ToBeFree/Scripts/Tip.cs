namespace ToBeFree
{
	public class Tip
	{
		private string script;
		private eTipTiming timing;

		public Tip(string script, eTipTiming timing)
		{
			this.script = script;
			this.timing = timing;
		}

		public Tip(Tip tip) : this(tip.script, tip.timing)
		{
		}

		public eTipTiming Timing
		{
			get
			{
				return timing;
			}
		}

		public string Script
		{
			get
			{
				return script;
			}

			set
			{
				script = value;
			}
		}
	}
}
