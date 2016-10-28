using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class AbnormalConditionManager : Singleton<AbnormalConditionManager>
	{
		private readonly AbnormalCondition[] list;
		private readonly AbnormalConditionData[] dataList;
		private readonly string file = Application.streamingAssetsPath + "/AbnormalCondition.json";

		public AbnormalCondition[] List
		{
			get
			{
				return list;
			}
		}

		public AbnormalConditionManager()
		{
			DataList<AbnormalConditionData> cDataList = new DataList<AbnormalConditionData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new AbnormalCondition[dataList.Length];

			ParseData();
		}

		private void ParseData()
		{
			foreach (AbnormalConditionData data in dataList)
			{
				Effect effect = EffectManager.Instance.List[data.effectIndex];
				EffectAmount effectAmount = new EffectAmount(effect, data.amount);
				EffectAmount[] effectAmountList = new EffectAmount[] { effectAmount };
				Buff buff = new Buff(data.name, data.script, effectAmountList, bool.Parse(data.isRestore), 
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
					abnormalCondition = new Hunger(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
				}
				else if (data.name == typeof(Detention).Name)
				{
					abnormalCondition = new Detention(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
				}
				else
				{
					abnormalCondition = new AbnormalCondition(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
				}

				if (list[data.index] != null)
				{
					throw new Exception("AbnormalCondition data.index " + data.index + " is duplicated.");
				}
				list[data.index] = abnormalCondition;
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
