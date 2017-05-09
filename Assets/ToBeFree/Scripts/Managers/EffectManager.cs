using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class EffectManager : Singleton<EffectManager>
	{
		private readonly Effect[] list;
		private readonly EffectData[] dataList;
		private readonly string file;
		private readonly string fileName = "/Effect.json";
		private Language.EffectData[] engList;
		private Language.EffectData[] korList;
		private List<Language.EffectData[]> languageList;

		public EffectManager()
		{
			file = Application.streamingAssetsPath + fileName;
			DataList<EffectData> cDataList = new DataList<EffectData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Effect[dataList.Length];

			engList = new DataList<Language.EffectData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.EffectData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.EffectData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			for(int i=0; i<dataList.Length; ++i)
			{
				EffectData data = dataList[i];
				Effect effect = new Effect(data.index, EnumConvert<eSubjectType>.ToEnum(data.subjectType),
											EnumConvert<eVerbType>.ToEnum(data.verbType),
											EnumConvert<eObjectType>.ToEnum(data.objectType),
											data.script);

				if (list[i] != null)
				{
					throw new Exception(i + "th Effect is duplicated.");
				}
				list[i] = effect;
			}
		}
		
		public Effect GetByIndex(int index)
		{
			return Array.Find<Effect>(list, x => x.Index == index);
		}

		public Effect Find(eSubjectType subjectType, eVerbType verbType, eObjectType objectType = eObjectType.NULL)
		{
			return Array.Find<Effect>(list, x => x.SubjectType == subjectType && x.VerbType == verbType && x.ObjectType == objectType);
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.EffectData data in languageList[(int)language])
			{
				try
				{
					GetByIndex(data.index).Script = data.script;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + e);
				}

			}
		}
	}
}