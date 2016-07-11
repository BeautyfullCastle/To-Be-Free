using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class QuestManager : Singleton<QuestManager>
    {
        private readonly Quest[] list;
        private readonly QuestData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Quest.json";
        
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
                EffectAmount failureEffect = null;
                if (data.failureEffectIndexList[0] != -99) {
                    failureEffect = new EffectAmount(EffectManager.Instance.List[data.failureEffectIndexList[0]], data.failureEffectValueList[0]);
                }
                EffectAmount[] failureEffects = new EffectAmount[] { failureEffect };
                ResultScriptAndEffects failureResultEffects = new ResultScriptAndEffects(data.failureScript, failureEffects);

                Event event_ = null;
                if (data.eventIndex != -99)
                {
                    event_ = EventManager.Instance.List[data.eventIndex];
                }

                Quest quest = new Quest(EnumConvert<eSubjectType>.ToEnum(data.subjectType), EnumConvert<eObjectType>.ToEnum(data.objectType),
                    data.comparisonOperator, data.compareAmount, EnumConvert<eQuestActionType>.ToEnum(data.actionType), EnumConvert<eRegion>.ToEnum(data.region),
                    EnumConvert<eTestStat>.ToEnum(data.stat), EnumConvert<eDifficulty>.ToEnum(data.difficulty), data.script, 
                    failureResultEffects, event_, data.duration, data.uiName, data.uiConditionScript);

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

        public IEnumerator ActivateQuest(Quest quest, bool testResult, Character character)
        {
            yield return EventManager.Instance.ActivateEvent(quest.Event_, character);
            if(EventManager.Instance.CurrEventTestResult)
            {
                GameManager.FindObjectOfType<UIQuestManager>().DeleteQuest(quest);
            }
        }

        public IEnumerator Load(Quest selectedQuest, Character character)
        {
            City city = null;
            int distance = 2;
            if (selectedQuest.Event_ != null)
            {
                city = CityManager.Instance.FindRandCityByDistance(character.CurCity, distance);
            }
            QuestPiece questPiece = new QuestPiece(selectedQuest, character, city, eSubjectType.QUEST);
            GameManager.Instance.uiEventManager.OpenUI();
            GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, selectedQuest.Script);
            yield return EventManager.Instance.WaitUntilFinish();

            PieceManager.Instance.Add(questPiece);
        }
    }
}
