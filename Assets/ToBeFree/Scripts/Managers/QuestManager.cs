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
                Quest quest = new Quest(EnumConvert<eSubjectType>.ToEnum(data.subjectType), EnumConvert<eObjectType>.ToEnum(data.objectType),
                    data.comparisonOperator, data.compareAmount, EnumConvert<eQuestActionType>.ToEnum(data.actionType), EnumConvert<eRegion>.ToEnum(data.region),
                    EnumConvert<eTestStat>.ToEnum(data.stat), EnumConvert<eDifficulty>.ToEnum(data.difficulty), data.script, data.failureScript,
                    ResultManager.Instance.List[data.resultIndex], data.duration, data.uiName, data.uiConditionScript);

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
