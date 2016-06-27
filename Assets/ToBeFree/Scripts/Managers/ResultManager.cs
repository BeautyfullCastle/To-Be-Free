using System;
using System.Collections.Generic;
using ToBeFree;
using UnityEngine;

namespace ToBeFree
{
    public class ResultManager : Singleton<ResultManager>
    {
        private readonly Result[] list;
        private readonly ResultData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Result.json";

        public ResultManager()
        {
            DataList<ResultData> cDataList = new DataList<ResultData>(file);
            //ResultDataList cDataList = new ResultDataList(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Result[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach (ResultData data in dataList)
            {
                ResultScriptAndEffects success = MakeResultScriptAndEffects(data.successScript, data.successEffectIndexList, data.successEffectValueList);
                ResultScriptAndEffects failure = MakeResultScriptAndEffects(data.failureScript, data.failureEffectIndexList, data.failureEffectValueList);

                Result result = new Result(EnumConvert<eTestStat>.ToEnum(data.stat), success, failure);

                list[data.index] = result;
            }
        }

        private ResultScriptAndEffects MakeResultScriptAndEffects(string script, int[] indexList, int[] valueList)
        {
            Effect[] effects = new Effect[indexList.Length];
            for (int i = 0; i < effects.Length; ++i)
            {
                if(indexList[i] == -99)
                {
                    effects[i] = null;
                    continue;
                }
                effects[i] = EffectManager.Instance.List[indexList[i]];
            }

            EffectAmount[] successResultEffects = new EffectAmount[effects.Length];
            for (int i = 0; i < effects.Length; ++i)
            {
                successResultEffects[i] = new EffectAmount(effects[i], valueList[i]);
            }

            return new ResultScriptAndEffects(script, successResultEffects);
        }

        public Result[] List
        {
            get
            {
                return list;
            }
        }
    }
}
