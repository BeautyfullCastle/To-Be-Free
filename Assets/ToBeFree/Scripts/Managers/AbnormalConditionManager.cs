using System;
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
                Buff buff = new Buff(data.name, effectAmountList, bool.Parse(data.isRestore), 
                                EnumConvert<eStartTime>.ToEnum(data.startTime), EnumConvert<eDuration>.ToEnum(data.duration));

                string[] splitedList = data.spawnCondition.Split(' ');
                Condition spawnCondition = null;
                if (splitedList.Length == 3)
                {
                    spawnCondition = new Condition(EnumConvert<eSubjectType>.ToEnum(splitedList[0]), splitedList[1], int.Parse(splitedList[2]));
                }

                AbnormalCondition abnormalCondition = null;
                
                
                if (data.name == typeof(Despair).Name)
                {
                    abnormalCondition = new Despair(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if(data.name == typeof(LegInjury).Name)
                {
                    abnormalCondition = new LegInjury(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(WristInjury).Name)
                {
                    abnormalCondition = new WristInjury(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(InternalInjury).Name)
                {
                    abnormalCondition = new InternalInjury(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(Fatigue).Name)
                {
                    abnormalCondition = new Fatigue(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(AnxietyDisorder).Name)
                {
                    abnormalCondition = new AnxietyDisorder(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(Exhilaration).Name)
                {
                    abnormalCondition = new Exhilaration(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(Full).Name)
                {
                    abnormalCondition = new Full(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(Concentration).Name)
                {
                    abnormalCondition = new Concentration(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(Detention).Name)
                {
                    abnormalCondition = new Detention(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreaseStrength).Name)
                {
                    abnormalCondition = new DecreaseStrength(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreaseAgility).Name)
                {
                    abnormalCondition = new DecreaseAgility(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreaseObservation).Name)
                {
                    abnormalCondition = new DecreaseObservation(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreaseBargain).Name)
                {
                    abnormalCondition = new DecreaseBargain(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreasePatience).Name)
                {
                    abnormalCondition = new DecreasePatience(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreaseLuck).Name)
                {
                    abnormalCondition = new DecreaseLuck(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else if (data.name == typeof(DecreaseAllStat).Name)
                {
                    abnormalCondition = new DecreaseAllStat(data.name, buff, spawnCondition, bool.Parse(data.stack), EnumConvert<eBodyMental>.ToEnum(data.isBody), EnumConvert<ePositiveNegative>.ToEnum(data.isPositive));
                }
                else
                {
                    Debug.LogError("Wrong name : " + data.name);
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
    }
}
