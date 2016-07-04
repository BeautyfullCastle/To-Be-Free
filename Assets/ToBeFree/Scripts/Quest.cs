using System;

namespace ToBeFree
{
    public enum eQuestActionType
    {
        QUEST, GLOBAL_QUEST
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
        private readonly eTestStat stat;
        private readonly eDifficulty difficulty;
        private readonly string script;
        private readonly string failureScript;
        private readonly Result result;
        private readonly int duration;
        private readonly string uiName;
        private readonly string uiConditionScript;
        

        public Quest(eSubjectType subjectType, eObjectType objectType, string comparisonOperator,
            int compareAmount, eQuestActionType actionType, eRegion region, eTestStat stat, eDifficulty difficulty,
            string script, string failureScript, Result result, int duration, string uiName, string uiConditionScript)
        {
            this.subjectType = subjectType;
            this.objectType = objectType;
            this.comparisonOperator = comparisonOperator;
            this.compareAmount = compareAmount;
            this.condition = new Condition(subjectType, comparisonOperator, compareAmount);
            this.actionType = actionType;
            this.region = region;
            this.stat = stat;
            this.difficulty = difficulty;
            this.script = script;
            this.failureScript = failureScript;
            this.result = result;
            this.duration = duration;
            this.uiName = uiName;
            this.uiConditionScript = uiConditionScript;
        }
        
        public Result Result
        {
            get
            {
                return result;
            }
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

        public void TreatPastQuest(Character character)
        {
            QuestManager.Instance.ActivateResultEffects(result.Failure.EffectAmounts, character);
        }

        public bool CheckCondition(Character character)
        {
            return condition.CheckCondition(character);
        }
    }
}
