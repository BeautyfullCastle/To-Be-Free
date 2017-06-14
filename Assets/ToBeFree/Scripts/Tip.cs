using System;

namespace ToBeFree
{
	[Serializable]
	public class TipSaveData
	{
		public int index;
		public bool watched;

		public TipSaveData(int index, bool watched)
		{
			this.index = index;
			this.watched = watched;
		}
	}

	public class Tip
	{
		private int index;
		private string title;
		private eTipTiming timing;
		private string script;
		private string spriteName;
		private int nextIndex;
		private bool watched;

		public Tip(int index, string title, eTipTiming timing, string script, string spriteName, int nextIndex)
		{
			this.index = index;
			this.title = title;
			this.timing = timing;
			this.script = script;
			this.spriteName = spriteName;
			this.nextIndex = nextIndex;

			this.watched = false;
		}

		public Tip(Tip tip) : this(tip.index, tip.title, tip.timing, tip.script, tip.spriteName, tip.nextIndex)
		{
		}

		public eTipTiming Timing
		{
			get
			{
				return timing;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}

			set
			{
				title = value;
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

		public string SpriteName
		{
			get
			{
				return spriteName;
			}
		}

		public int NextIndex
		{
			get
			{
				return nextIndex;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public bool Watched
		{
			get
			{
				return watched;
			}

			set
			{
				watched = value;
			}
		}
	}
}
