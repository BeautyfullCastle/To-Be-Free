using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class AbnormalConditionManager : Singleton<AbnormalConditionManager>
	{
		private AbnormalCondition[] list;
		private AbnormalConditionData[] dataList;
		private const string fileName = "/AbnormalCondition.json";
		private readonly string file = Application.streamingAssetsPath + fileName;

		private Language.AbnormalConditionData[] engList;
		private Language.AbnormalConditionData[] korList;
		private List<Language.AbnormalConditionData[]> languageList;

		public void Init()
		{
			DataList<AbnormalConditionData> cDataList = new DataList<AbnormalConditionData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new AbnormalCondition[dataList.Length];

			engList = new DataList<Language.AbnormalConditionData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.AbnormalConditionData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.AbnormalConditionData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			foreach (AbnormalConditionData data in dataList)
			{
				Effect effect = EffectManager.Instance.GetByIndex(data.effectIndex);
				EffectAmount effectAmount = new EffectAmount(effect, data.amount);
				EffectAmount[] effectAmountList = new EffectAmount[] { effectAmount };
				Buff buff = new Buff(data.index, data.name, data.script, effectAmountList, bool.Parse(data.isRestore), 
								EnumConvert<eStartTime>.ToEnum(data.startTime), EnumConvert<eDuration>.ToEnum(data.duration));

				string[] splitedList = data.spawnCondition.Split(' ');
				Condition spawnCondition = null;
				if (splitedList.Length == 3)
				{
					spawnCondition = new Condition(EnumConvert<eSubjectType>.ToEnum(splitedList[0]), splitedList[1], int.Parse(splitedList[2]));
				}
				else
				{
					spawnCondition = new Condition(EnumConvert<eSubjectType>.ToEnum(data.spawnCondition), string.Empty, -99);
				}

				AbnormalCondition abnormalCondition = null;
				
				if (data.name == typeof(Hunger).Name)
				{
					abnormalCondition = new Hunger(data.index, data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
				}
				else if (data.name == typeof(Detention).Name)
				{
					abnormalCondition = new Detention(data.index, data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
				}
				else
				{
					abnormalCondition = new AbnormalCondition(data.index, data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
				}

				if (list[data.index] != null)
				{
					throw new Exception("AbnormalCondition data.index " + data.index + " is duplicated.");
				}
				list[data.index] = abnormalCondition;
			}
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.AbnormalConditionData data in languageList[(int)language])
			{
				try
				{
					list[data.index].Name = data.name;
					list[data.index].Buff.Script = data.script;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
			}
			GameManager.Instance.uiBuffManager.Refresh();
			GameManager.Instance.uiInventory.Refresh();
			if (GameManager.Instance.shopUIObj)
			{
				GameManager.Instance.shopUIObj.GetComponent<UIShop>().Refresh();
			}
		}

		public AbnormalCondition GetByIndex(int index)
		{
			if(index < 0 || index >= list.Length)
			{
				return null;
			}
			return list[index];
		}

		public void Save(List<AbnormalConditionSaveData> abnormalList)
		{
			for(int i = 0; i < list.Length; ++i)
			{
				AbnormalConditionSaveData data = new AbnormalConditionSaveData(i, list[i].Stack, list[i].Amount);
				abnormalList.Add(data);
			}
		}

		public void Load(List<AbnormalConditionSaveData> abnormalList)
		{
			for (int i = 0; i < abnormalList.Count; ++i)
			{
				list[i].Stack = abnormalList[i].stack;
				list[i].Amount = abnormalList[i].amount;
			}
		}

		public AbnormalCondition Find(string name)
		{
			return Array.Find(list, x => x.Name == name);
		}

		public IEnumerator ActiveByCondition()
		{
			foreach(AbnormalCondition ab in list)
			{
				if(ab.CheckCondition(GameManager.Instance.Character))
				{
					yield return ab.Activate(GameManager.Instance.Character);
				}
			}
		}
	}
}
