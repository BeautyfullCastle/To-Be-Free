using System;
using UnityEngine;

namespace ToBeFree
{
	public enum ItemTag
	{
		NULL = 0, FOOD
	}

	public class ItemManager : Singleton<ItemManager>
	{
		private readonly Item[] list;
		private readonly ItemData[] dataList;
		private readonly string file = Application.streamingAssetsPath + "/Item.json";

		
		public ItemManager()
		{
			DataList<ItemData> cDataList = new DataList<ItemData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Item[dataList.Length];

			ParseData();
		}

		private void ParseData()
		{
			foreach (ItemData data in dataList)
			{
				EffectAmount[] effectAmountList = new EffectAmount[data.effectIndexList.Length];
				for (int i = 0; i < data.effectIndexList.Length; ++i)
				{
					if(data.effectIndexList[i] == -99)
					{
						continue;
					}
					EffectAmount effectAmount = new EffectAmount(EffectManager.Instance.List[data.effectIndexList[i]], data.amountList[i]);
					effectAmountList[i] = effectAmount;
				}

				Buff buff = new Buff(data.name, data.script, effectAmountList, bool.Parse(data.restore),
										EnumConvert<eStartTime>.ToEnum(data.startTime), EnumConvert<eDuration>.ToEnum(data.duration));

				Item item = new Item(data.index, data.name, buff, data.price, EnumConvert<ItemTag>.ToEnum(data.tag));

				if (list[data.index] != null)
				{
					throw new Exception("Item data.index " + data.index + " is duplicated.");
				}
				list[data.index] = item;
			}
		}

		public Item[] FindAll(ItemTag tag)
		{
			return Array.FindAll<Item>(list, x => x.Tag == tag);
		}

		public Item GetRand()
		{
			System.Random r = new System.Random();
			int index = r.Next(0, list.Length - 1);
			return list[index];
		}

		public Item GetTagRand(int iTag)
		{
			// TO DO : compare tag list to iTag
			//List<Item> tagItems = list.FindAll(x => x.Tag == ToString(iTag));
			//System.Random r = new System.Random();
			//int index = r.Next(0, tagItems.Count - 1);
			//return tagItems[index];

			return null;
		}
		
		public Item GetByIndex(int index)
		{
			return list[index];
		}

		public Item[] List
		{
			get
			{
				return list;
			}
		}
	}
}