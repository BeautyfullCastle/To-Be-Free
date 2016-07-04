using System;
using UnityEngine;

namespace ToBeFree
{
    public class AbnormalConditionManager : Singleton<AbnormalConditionManager>
    {
        private readonly AbnormalCondition[] list;
        private readonly AbnormalConditionData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/AbnormalCondition.json";

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
                Condition spawnCondition = new Condition(EnumConvert<eSubjectType>.ToEnum(splitedList[0]), splitedList[1], int.Parse(splitedList[2]));
                AbnormalCondition abnormalCondition = new AbnormalCondition(data.name, buff, spawnCondition, bool.Parse(data.isBody), bool.Parse(data.isPositive));

                list[data.index] = abnormalCondition;
            }
        }
    }
}
