using System;
using UnityEngine;

namespace ToBeFree
{
    public class StatProbabilityManager : Singleton<StatProbabilityManager>
    {
        private readonly StatProbability[] list;
        private readonly StatProbabilityData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/StatProbability.json";

        public StatProbabilityManager()
        {
            DataList<StatProbabilityData> cDataList = new DataList<StatProbabilityData>(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new StatProbability[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach (StatProbabilityData data in dataList)
            {
                StatProbability statProb = new StatProbability(EnumConvert<eEventAction>.ToEnum(data.actionType), data.valueList);

                list[data.index] = statProb;
            }
        }

        
        public StatProbability FindProbByAction(eEventAction actionType)
        {
            return Array.Find<StatProbability>(list, x => x.ActionType == actionType);
        }
    }
}