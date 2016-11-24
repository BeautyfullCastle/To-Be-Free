using System;
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
		private readonly string file = Application.streamingAssetsPath + "/Tip.json";

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

		public void Show(eTipTiming timing)
		{
			Tip tip = Array.Find<Tip>(list, x => x.Timing == timing);
			tip.Show();
		}
	}
}