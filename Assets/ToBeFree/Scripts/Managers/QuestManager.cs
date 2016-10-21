using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class QuestManager : Singleton<QuestManager>
	{
		private Quest[] list;
		private QuestData[] dataList;
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
			
		}

		public void Init()
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
					try {
						event_ = EventManager.Instance.List[data.eventIndex];
					} catch (UnityException e)
					{
						if(e == null)
						{
							Debug.LogError(e.Message);
						}
					}
				}

				Quest quest = new Quest(EnumConvert<eSubjectType>.ToEnum(data.subjectType), EnumConvert<eObjectType>.ToEnum(data.objectType),
					data.comparisonOperator, data.compareAmount, EnumConvert<eQuestActionType>.ToEnum(data.actionType), 
					EnumConvert<eRegion>.ToEnum(data.region), data.cityName, EnumConvert<eDifficulty>.ToEnum(data.difficulty), data.script, 
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
			int index = UnityEngine.Random.Range(0, list.Length);
			return list[index];
		}
		
		public Quest FindRand(eQuestActionType questActionType)
		{
			Quest[] array = Array.FindAll(list, x => x.ActionType == questActionType);
			return array[UnityEngine.Random.Range(0, array.Length)];
		}

		public int IndexOf(Quest quest)
		{
			return Array.IndexOf(list, quest as Quest);
		}

		public void ActivateResultEffects(EffectAmount[] effectAmounts, Character character)
		{
			EventManager.Instance.ActivateResultEffects(effectAmounts, character);
		}

		public IEnumerator ActivateQuest(Quest quest, bool testResult, Character character)
		{
			yield return EventManager.Instance.ActivateEvent(quest.Event_, character);
		}

		public IEnumerator Load(Quest selectedQuest, Character character)
		{
			City city = null;
			
			if(selectedQuest.Region == eRegion.CITY)
			{
				city = CityManager.Instance.Find(selectedQuest.CityName);
			}
			else if(selectedQuest.Region == eRegion.RANDOM)
			{
				city = CityManager.Instance.FindRand(eSubjectType.QUEST);
			}
			else if(selectedQuest.Region == eRegion.CURRENT)
			{
				city = character.CurCity;
			}

			QuestPiece questPiece = new QuestPiece(selectedQuest, character, city, eSubjectType.QUEST);
			GameManager.Instance.uiEventManager.OpenUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, selectedQuest.Script);
			yield return EventManager.Instance.WaitUntilFinish();

			PieceManager.Instance.Add(questPiece);
		}
	}
}
