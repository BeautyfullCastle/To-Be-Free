using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum ItemTag
	{
		NULL = 0, FOOD
	}

	public class ItemManager : Singleton<ItemManager>
	{
		private Item[] list;
		private ItemData[] dataList;
		private const string fileName = "/Item.json";
		private readonly string file = Application.streamingAssetsPath + fileName;

		private Language.ItemData[] engList;
		private Language.ItemData[] korList;
		private List<Language.ItemData[]> languageList;

		public void Init()
		{
			DataList<ItemData> cDataList = new DataList<ItemData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Item[dataList.Length];

			engList = new DataList<Language.ItemData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.ItemData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.ItemData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		public List<Item> CopyTo()
		{
			return new List<Item>(this.list);
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
					EffectAmount effectAmount = new EffectAmount(EffectManager.Instance.GetByIndex(data.effectIndexList[i]), data.amountList[i]);
					effectAmountList[i] = effectAmount;
				}

				Buff buff = new Buff(data.index, data.name, data.script, effectAmountList, bool.Parse(data.restore),
										EnumConvert<eStartTime>.ToEnum(data.startTime), EnumConvert<eDuration>.ToEnum(data.duration));

				Item item = new Item(data.index, data.name, buff, data.price, EnumConvert<ItemTag>.ToEnum(data.tag));

				if (list[data.index] != null)
				{
					throw new Exception("Item data.index " + data.index + " is duplicated.");
				}
				list[data.index] = item;
			}
		}

		public string GetEngName(int index)
		{
			if(index < 0 || index >= engList.Length)
			{
				return string.Empty;
			}
			return this.engList[index].name;
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.ItemData data in languageList[(int)language])
			{
				try
				{
					list[data.index].Name = data.name;
					list[data.index].Buff.Name = data.name;
					list[data.index].Buff.Script = data.script;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
			}
			GameManager.Instance.uiInventory.Refresh();
			if(GameManager.Instance.shopUIObj)
			{
				GameManager.Instance.shopUIObj.GetComponent<UIShop>().Refresh();
			}
		}

		public Item[] FindAll(ItemTag tag)
		{
			if (list == null)
				return null;
			else if (list.Length <= 0)
				return null;

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
			if (index < 0 || index >= list.Length)
				return null;

			return list[index];
		}

		public Item GetByType(eObjectType objectType, int amount)
		{
			Item item = null;
			if (objectType == eObjectType.TAG)
			{
				item = ItemManager.Instance.GetTagRand(amount);
			}
			else if (objectType == eObjectType.INDEX)
			{
				item = ItemManager.Instance.GetByIndex(amount);
			}
			else
			{
				item = ItemManager.Instance.GetRand();
			}

			return item;
		}
	}
}