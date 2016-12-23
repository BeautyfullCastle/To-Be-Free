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
		private const string fileName = "/Quest.json";
		private readonly string file = Application.streamingAssetsPath + fileName;

		private Language.QuestData[] engList;
		private Language.QuestData[] korList;
		private List<Language.QuestData[]> languageList;

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

			engList = new DataList<Language.QuestData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.QuestData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.QuestData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			foreach (QuestData data in dataList)
			{
				EffectAmount failureEffect = null;
				if (data.failureEffectIndexList[0] != -99) {
					failureEffect = new EffectAmount(EffectManager.Instance.GetByIndex(data.failureEffectIndexList[0]), data.failureEffectValueList[0]);
				}
				EffectAmount[] failureEffects = new EffectAmount[] { failureEffect };
				ResultScriptAndEffects failureResultEffects = new ResultScriptAndEffects(data.failureScript, failureEffects);

				Event event_ = null;
				if (data.eventIndex != -99)
				{
					try {
						event_ = EventManager.Instance.List[data.eventIndex];
					} catch (Exception e)
					{
						if(e == null)
						{
							Debug.LogError(data.eventIndex + " : " + e.Message);
						}
					}
				}

				Quest quest = new Quest(data.index, EnumConvert<eSubjectType>.ToEnum(data.subjectType), EnumConvert<eObjectType>.ToEnum(data.objectType),
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

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.QuestData data in languageList[(int)language])
			{
				try
				{
					Quest quest = this.GetByIndex(data.index);
					if (quest == null)
						continue;

					quest.Script = data.script;
					quest.FailureEffects.Script = data.failureScript;
					quest.UiName = data.uiName;
					quest.UiConditionScript = data.uiConditionScript;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
			}
			GameManager.Instance.uiQuestManager.Refresh();
		}

		public void Save(List<QuestSaveData> questList)
		{
			for(int i=0; i<list.Length; ++i)
			{
				QuestSaveData data = new QuestSaveData(i, list[i].PastDays);
				questList.Add(data);
			}
		}

		public void Load(List<QuestSaveData> questList)
		{
			for (int i = 0; i < questList.Count; ++i)
			{
				list[i].PastDays = questList[i].pastDays;
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

		public Quest GetByIndex(int index)
		{
			if(index < 0 || index >= list.Length)
			{
				return null;
			}
			return list[index];
		}

		public int IndexOf(Quest quest)
		{
			return Array.IndexOf(list, quest as Quest);
		}

		public void ActivateResultEffects(EffectAmount[] effectAmounts, Character character)
		{
			EventManager.Instance.ActivateResultEffects(effectAmounts, character);
		}

		public IEnumerator ActivateQuest(Quest quest, Character character)
		{
			yield return EventManager.Instance.ActivateEvent(quest.Event_, character);
		}

		public IEnumerator Load(Quest selectedQuest, Character character)
		{
			yield return GameManager.Instance.uiEventManager.OnChanged(selectedQuest.Script);

			City city = null;

			if (selectedQuest.Region != eRegion.NULL)
			{
				if (selectedQuest.Region == eRegion.CITY)
				{
					city = CityManager.Instance.Find(selectedQuest.CityName);
				}
				else if (selectedQuest.Region == eRegion.RANDOM)
				{
					city = CityManager.Instance.FindRandCityByDistance(character.CurCity, 3, eSubjectType.QUEST, eWay.NORMALWAY);
				}
				else if (selectedQuest.Region == eRegion.CURRENT)
				{
					city = character.CurCity;
				}
			}
			QuestPiece questPiece = new QuestPiece(selectedQuest, city, eSubjectType.QUEST);
			PieceManager.Instance.Add(questPiece);
		}
	}
}
