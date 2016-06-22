using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class EffectManager : Singleton<EffectManager>
    {
        private readonly Effect[] list;
        private readonly EffectData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Effect.json";
        
        public EffectManager()
        {
            DataList<EffectData> cDataList = new DataList<EffectData>(file);
            //EffectDataList cDataList = new EffectDataList(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Effect[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach(EffectData data in dataList)
            {
                Effect effect = new Effect(EnumConvert<eSubjectType>.ToEnum(data.subjectType),
                                            EnumConvert<eVerbType>.ToEnum(data.verbType),
                                            EnumConvert<eObjectType>.ToEnum(data.objectType));

                list[data.index] = effect;
            }
        }

        public Effect[] List
        {
            get
            {
                return list;
            }
        }
    }
}
