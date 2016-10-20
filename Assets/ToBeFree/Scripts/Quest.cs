﻿using System;

namespace ToBeFree
{
	public enum eQuestActionType
	{
		NULL, QUEST, QUEST_BROKERINFO
	}
	
	public enum eRegion
	{
		NULL, RANDOM, CITY
	}

	public class Quest
	{
		private readonly eSubjectType subjectType;
		private readonly eObjectType objectType;
		private readonly string comparisonOperator;
		private readonly int compareAmount;
		private readonly Condition condition;
		private readonly eQuestActionType actionType;
		private readonly eRegion region;
		private readonly int amount;
		private readonly eDifficulty difficulty;
		private readonly string script;
		private readonly ResultScriptAndEffects failureEffects;
		private readonly Event event_;
		private readonly int duration;
		private readonly string uiName;
		private readonly string uiConditionScript;
		

		public Quest(eSubjectType subjectType, eObjectType objectType, string comparisonOperator,
			int compareAmount, eQuestActionType actionType, eRegion region, int amount, eDifficulty difficulty,
			string script, ResultScriptAndEffects failureEffects, Event event_, int duration, string uiName, string uiConditionScript)
		{
			this.subjectType = subjectType;
			this.objectType = objectType;
			this.comparisonOperator = comparisonOperator;
			this.compareAmount = compareAmount;
			this.condition = new Condition(subjectType, comparisonOperator, compareAmount);
			this.actionType = actionType;
			this.region = region;
			this.amount = amount;
			this.difficulty = difficulty;
			this.script = script;
			this.failureEffects = failureEffects;
			this.event_ = event_;
			this.duration = duration;
			this.uiName = uiName;
			this.uiConditionScript = uiConditionScript;
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
		}

		public string UiName
		{
			get
			{
				return uiName;
			}
		}

		public string UiConditionScript
		{
			get
			{
				return uiConditionScript;
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

		public int Amount
		{
			get
			{
				return amount;
			}
		}

		public void TreatPastQuest(Character character)
		{
			QuestManager.Instance.ActivateResultEffects(failureEffects.EffectAmounts, character);
		}

		public bool CheckCondition(Character character)
		{
			return condition.CheckCondition(character);
		}
	}
}
