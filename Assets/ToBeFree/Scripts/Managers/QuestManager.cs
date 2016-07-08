using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class QuestManager : Singleton<QuestManager>
    {
        private readonly Quest[] list;
        private readonly QuestData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Quest.json";

        public delegate void DeleteQuestHandler(Quest quest);
        public static DeleteQuestHandler DeleteQuest;

        public Quest[] List
        {
            get
            {
                return list;
            }
        }

        public QuestManager()
        {
            DataList<QuestData> cDataList = new DataList<QuestData>(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Quest[dataList.Length];
            

            ParseData();
        }

        private void ParseData()
        {
            foreach (QuestData data in dataList)
            {
                EffectAmount effect = null;
                if (data.failureEffectIndexList[0] != -99) {
                    effect = new EffectAmount(EffectManager.Instance.List[data.failureEffectIndexList[0]], data.failureEffectValueList[0]);
                }
                EffectAmount[] effects = new EffectAmount[] { effect };
                ResultScriptAndEffects resultEffects = new ResultScriptAndEffects(data.failureScript, effects);

                Event event_ = null;
                if (data.eventIndex != -99)
                {
                    event_ = EventManager.Instance.List[data.eventIndex];
                }

                Quest quest = new Quest(EnumConvert<eSubjectType>.ToEnum(data.subjectType), EnumConvert<eObjectType>.ToEnum(data.objectType),
                    data.comparisonOperator, data.compareAmount, EnumConvert<eQuestActionType>.ToEnum(data.actionType), EnumConvert<eRegion>.ToEnum(data.region),
                    EnumConvert<eTestStat>.ToEnum(data.stat), EnumConvert<eDifficulty>.ToEnum(data.difficulty), data.script, 
                    resultEffects, event_, data.duration, data.uiName, data.uiConditionScript);

                if(list[data.index] != null)
                {
                    throw new Exception("Quest data.index " + data.index + " is duplicated.");
                }
                list[data.index] = quest;
            }
        }
        
        public Quest FindRand()
        {
            System.Random r = new System.Random();
            int index = r.Next(0, list.Length);
            return list[index];
        }

        public void ActivateResultEffects(EffectAmount[] effectAmounts, Character character)
        {
            EventManager.Instance.ActivateResultEffects(effectAmounts, character);
        }

        public void ActivateQuest(Quest quest, bool testResult, Character character)
        {
            EventManager.Instance.ActivateQuest(quest, testResult, character);
            DeleteQuest(quest);
        }
    }
}
