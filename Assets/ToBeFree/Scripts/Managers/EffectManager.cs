using System;
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

				if (list[data.index] != null)
				{
					throw new Exception("Effect data.index " + data.index + " is duplicated.");
				}
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

		public Effect Find(eSubjectType subjectType, eVerbType verbType, eObjectType objectType = eObjectType.NULL)
		{
			return Array.Find<Effect>(list, x => x.SubjectType == subjectType && x.VerbType == verbType && x.ObjectType == objectType);
		}
	}
}