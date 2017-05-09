using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class ResultManager : Singleton<ResultManager>
	{
		private Result[] list;
		private ResultData[] dataList;
		private const string fileName = "/Result.json";
		private readonly string file = Application.streamingAssetsPath + fileName;

		private Language.ResultData[] engList;
		private Language.ResultData[] korList;
		private List<Language.ResultData[]> languageList;

		public void Init()
		{
			DataList<ResultData> cDataList = new DataList<ResultData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Result[dataList.Length];

			engList = new DataList<Language.ResultData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.ResultData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.ResultData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			foreach (ResultData data in dataList)
			{
				ResultScriptAndEffects success = MakeResultScriptAndEffects(data.successScript, data.successEffectIndexList, data.successEffectValueList);
				ResultScriptAndEffects failure = MakeResultScriptAndEffects(data.failureScript, data.failureEffectIndexList, data.failureEffectValueList);

				Result result = new Result(EnumConvert<eTestStat>.ToEnum(data.stat), success, failure);

				if(list[data.index] != null)
				{
					continue;
				}
				list[data.index] = result;
			}
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.ResultData data in languageList[(int)language])
			{
				try
				{
					list[data.index].Success.Script = data.successScript;
					list[data.index].Failure.Script = data.failureScript;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + e);
				}
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
				effects[i] = EffectManager.Instance.GetByIndex(indexList[i]);
			}

			EffectAmount[] successResultEffects = new EffectAmount[effects.Length];
			for (int i = 0; i < effects.Length; ++i)
			{
				successResultEffects[i] = new EffectAmount(effects[i], valueList[i]);
			}

			return new ResultScriptAndEffects(script, successResultEffects);
		}

		public Result GetByIndex(int index)
		{
			if (index < 0 || index >= list.Length)
				return null;

			return list[index];
		}
	}
}
