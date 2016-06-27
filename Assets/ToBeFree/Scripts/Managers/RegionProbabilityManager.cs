using System;
using UnityEngine;

namespace ToBeFree
{
    public class RegionProbabilityManager : Singleton<RegionProbabilityManager>
    {
        private readonly RegionProbability[] list;
        private readonly RegionProbabilityData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/RegionProbability.json";

        public RegionProbabilityManager()
        {
            DataList<RegionProbabilityData> cDataList = new DataList<RegionProbabilityData>(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new RegionProbability[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach (RegionProbabilityData data in dataList)
            {
                RegionProbability statProb = new RegionProbability(data.valueList);

                list[data.index] = statProb;
            }
        }

        public RegionProbability Prob
        {
            get
            {
                if (list.Length == 0)
                {
                    return null;
                }
                return list[0];
            }
        }
    }
}