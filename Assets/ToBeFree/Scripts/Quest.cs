﻿using System;

namespace ToBeFree
{
	public enum eQuestActionType
	{
		NULL, QUEST, QUEST_BROKERINFO
	}
	
	public enum eRegion
	{
		NULL, RANDOM, CITY, CURRENT
	}

	[Serializable]
	public class QuestSaveData
	{
		public QuestSaveData(int index, int pastDays, int cityIndex)
		{
			this.index = index;
			this.pastDays = pastDays;
			this.cityIndex = cityIndex;
		}

		public int index;
		public int pastDays;
		public int cityIndex;
	}

	public class Quest
	{
		private readonly int index;
		private readonly eSubjectType subjectType;
		private readonly eObjectType objectType;
		private readonly string comparisonOperator;
		private readonly int compareAmount;
		private readonly Condition condition;
		private readonly eQuestActionType actionType;
		private readonly eRegion region;
		private readonly string cityName;
		private readonly eDifficulty difficulty;
		private string script;
		private readonly ResultScriptAndEffects failureEffects;
		private readonly Event event_;
		private readonly int duration;
		private string uiName;
		private string uiConditionScript;

		public Quest(int index, eSubjectType subjectType, eObjectType objectType, string comparisonOperator,
			int compareAmount, eQuestActionType actionType, eRegion region, string cityName, eDifficulty difficulty,
			string script, ResultScriptAndEffects failureEffects, Event event_, int duration, string uiName, string uiConditionScript)
		{
			this.index = index;
			this.subjectType = subjectType;
			this.objectType = objectType;
			this.comparisonOperator = comparisonOperator;
			this.compareAmount = compareAmount;
			this.condition = new Condition(subjectType, comparisonOperator, compareAmount);
			this.actionType = actionType;
			this.region = region;
			this.cityName = cityName;
			this.difficulty = difficulty;
			this.script = script;
			this.failureEffects = failureEffects;
			this.event_ = event_;
			this.duration = duration;
			this.uiName = uiName;
			this.uiConditionScript = uiConditionScript;
		}

		public void TreatPastQuest(Character character)
		{
			QuestManager.Instance.ActivateResultEffects(failureEffects.EffectAmounts, character);
		}

		public bool CheckCondition(Character character, int pastDays)
		{
			return condition.CheckCondition(character, pastDays);
		}

		public int Duration
		{
			get
			{
				return duration;
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

		public string UiName
		{
			get
			{
				return uiName;
			}
			set
			{
				uiName = value;
			}
		}

		public string UiConditionScript
		{
			get
			{
				return uiConditionScript;
			}
			set
			{
				uiConditionScript = value;
			}
		}

		public Event Event_
		{
			get
			{
				return event_;
			}
		}

		public ResultScriptAndEffects FailureEffects
		{
			get
			{
				return failureEffects;
			}
		}

		public eQuestActionType ActionType
		{
			get
			{
				return actionType;
			}
		}

		public eRegion Region
		{
			get
			{
				return region;
			}
		}

		public string CityName
		{
			get
			{
				return cityName;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}
	}
}
