using UnityEngine;

namespace ToBeFree
{
	public enum eEventAction
	{
		NULL = 0x00,
		MOVE = 0x01, MOVE_BUS = 0x02,
		WORK = 0x04, WORK_START = 0x08, WORK_END = 0x10,
		INVESTIGATION_BROKER = 0x20, INVESTIGATION_POLICE = 0x40, INVESTIGATION_CITY = 0x80,
		GATHERING = 0x100, INSPECT = 0x200, DETENTION = 0x400, REST = 0x800, HIDE = 0x1000, BROKER = 0x2000, CAMP = 0x4000,
		START = 0x8000, END = 0x10000,
		INSPECT_SPECIAL = 0x20000, DETENTION_SPECIAL = 0x40000, GATHERING_SPECIAL = 0x80000,
		REST_SPECIAL = 0x100000, HIDE_SPECIAL = 0x200000,
		QUEST = 0x400000,
		INVESTIGATION_BROKER_SPECIAL = 0x800000, INVESTIGATION_POLICE_SPECIAL = 0x1000000, INVESTIGATION_CITY_SPECIAL = 0x2000000,
		QUEST_BROKERINFO = 0x4000000,
		ABILITY = 0x8000000
	}

	public enum eDifficulty
	{
		EASY, NORMAL, HARD
	}

	public class Event
	{
		private eEventAction actionType;
		private eDifficulty difficulty;
		private string script;
		private int resultIndex;
		private int[] selectIndexList;        
		

		public Event(eEventAction actionType, eDifficulty difficulty, string script, int resultIndex, int[] selectIndexList)
		{
			this.actionType = actionType;
			this.difficulty = difficulty;
			this.script = script;
			this.resultIndex = resultIndex;
			this.selectIndexList = selectIndexList;
		}

		public Event(Event event_)
		{
			this.actionType = event_.actionType;
			this.difficulty = event_.difficulty;
			this.script = string.Copy(event_.script);
			this.resultIndex = event_.resultIndex;
			this.selectIndexList = event_.selectIndexList;
		}
		
		public eEventAction ActionType { get { return actionType; } }
		public string Script { get { return script; } set { script = value; } }

		public bool HasSelect
		{
			get
			{
				if(selectIndexList[0] == -99)
				{
					return false;
				}
				return true;
			}
		}

		public Result Result
		{
			get
			{
				if(resultIndex == -99)
				{
					return null;
				}
				return ResultManager.Instance.List[resultIndex];
			}
		}

		public int[] SelectIndexList
		{
			get
			{
				if (HasSelect)
				{
					return selectIndexList;
				}
				return null;
			}
		}
	}
}