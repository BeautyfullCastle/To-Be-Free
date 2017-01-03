using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eTipTiming
	{
		NULL, Act, Work, Investigation, Rest, Move, PoliceMove, Inspect, Crackdown, Detention, Shop,
		PoliceAppeared, PoliceTurn, Camp, Gathering, GrabItem, Street, BigCity, Mountain
	}

	public class TipManager : Singleton<TipManager>
	{
		private Tip[] list;
		private TipData[] dataList;
		private const string fileName = "/Tip.json";
		private readonly string file = Application.streamingAssetsPath + fileName;

		private Language.TipData[] engList;
		private Language.TipData[] korList;
		private List<Language.TipData[]> languageList;

		public TipManager()
		{
		}

		public void Init()
		{
			DataList<TipData> cDataList = new DataList<TipData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Tip[dataList.Length];

			engList = new DataList<Language.TipData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.TipData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.TipData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			foreach (TipData data in dataList)
			{
				Tip tip = new Tip(data.script, EnumConvert<eTipTiming>.ToEnum(data.timing));

				if (list[data.index] != null)
				{
					throw new Exception("Tip data.index " + data.index + " is duplicated.");
				}
				list[data.index] = tip;
				Debug.Log(data.index + " : " + " : " + data.timing + " : " + data.script);
			}
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.TipData data in languageList[(int)language])
			{
				try
				{
					if (data.index < 0 || data.index >= list.Length)
					{
						continue;
					}

					list[data.index].Script = data.script;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
			}
			GameManager.Instance.uiTipManager.Refresh();
		}

		public void Show(eTipTiming timing)
		{
			Tip tip = Array.Find<Tip>(list, x => x.Timing == timing);

			if (tip == null)
				return;

			GameManager.Instance.uiTipManager.Show(tip);
			
		}
	}
}