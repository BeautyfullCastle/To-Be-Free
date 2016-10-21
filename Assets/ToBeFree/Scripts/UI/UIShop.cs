using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree {
	public class UIShop : MonoBehaviour {
		
		// 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
		public GameObject objSampleItem;
		
		public List<UIItem> basicItems = new List<UIItem>();
		public List<UIItem> cityItems;
		public List<UIItem> randomItems;

		private List<UIItem> items = new List<UIItem>();

		// 그리드를 reset position 하기위해 선언합니다.
		private UIGrid[] grids;

		private int discountNum;
		
		
		// Use this for initialization
		void Start()
		{
			grids = GetComponentsInChildren<UIGrid>();

			Item[] basicItemList = ItemManager.Instance.FindAll(ItemTag.FOOD);
			for(int i=0; i<basicItemList.Length; ++i)
			{
				basicItems[i].SetInfo(basicItemList[i]);
			}
			
			foreach(UIItem basic in basicItems)
			{
				if(GameManager.Instance.Character.Stat.Money >= basic.Item.Price)
				{
					basic.enabled = true;
				}
			}
			
			foreach (UIGrid grid in grids)
			{
				grid.Reposition();
			}

			discountNum = 0;
		}
		
		void OnEnable()
		{
			cityItems[0].SetInfo(GameManager.Instance.Character.CurCity.Item);

			List<Item> randomItemList = new List<Item>(ItemManager.Instance.List);
			randomItemList.Remove(basicItems[0].Item);
			randomItemList.Remove(basicItems[1].Item);
			randomItemList.Remove(cityItems[0].Item);
			randomItemList.RemoveAll(x => GameManager.Instance.Character.Inven.Exist(x));

			System.Random r = new System.Random();
			for (int i = 0; i < randomItems.Count; ++i)
			{
				if(GameManager.Instance.Character.CurCity.Type==eNodeType.MIDDLECITY)
				{
					if(i >= 3)
					{
						randomItems[i].gameObject.SetActive(false);
						continue;
					}
				}
				randomItems[i].gameObject.SetActive(true);
				int randIndex = r.Next(0, randomItemList.Count - 1);
				randomItems[i].SetInfo(randomItemList[randIndex]);
				randomItemList.Remove(randomItems[i].Item);
			}

			items.AddRange(basicItems);
			items.AddRange(cityItems);
			items.AddRange(randomItems);

			CheckAllItems();
		}

		// click to buy item
		public void OnClick(UIItem uiItem)
		{
			foreach (UIItem item in items)
			{
				if(item.name == uiItem.name)
				{
					GameManager.Instance.Character.Inven.BuyItem(uiItem.Item, discountNum, GameManager.Instance.Character);
					if (uiItem != basicItems[0])
					{
						uiItem.enabled = false;
						if (uiItem.Item.Buff.StartTime != eStartTime.NOW)
							uiItem.GetComponent<UIDragDropItem>().enabled = false;
					}	
					break;
				}
			}

			CheckItemsPrice();
		}

		public void CheckItems()
		{
			if (GameManager.Instance.Character.Inven.Exist(GameManager.Instance.Character.CurCity.Item))
			{
				cityItems[0].enabled = false;
			}
			else
			{
				cityItems[0].enabled = true;
			}

			for(int i=0; i<randomItems.Count; ++i)
			{
				if (randomItems[i].Item != null && GameManager.Instance.Character.Inven.Exist(randomItems[i].Item))
				{
					randomItems[i].enabled = false;
				}
				else
				{
					randomItems[i].enabled = true;
				}
			}
		}

		private void CheckItemsPrice()
		{
			foreach (UIItem uiItem in items)
			{
				if (uiItem == null || uiItem.Item == null)
				{
					uiItem.enabled = false;
					continue;
				}

				if (uiItem.enabled == false)
					continue;

				uiItem.enabled = (GameManager.Instance.Character.Stat.Money >= uiItem.Item.Price);
			}
		}

		private void CheckAllItems()
		{
			foreach (UIItem uiItem in items)
			{
				if (uiItem == null || uiItem.Item == null)
				{
					uiItem.enabled = false;
					continue;
				}

				if (basicItems.Contains(uiItem))
				{
					uiItem.enabled = true;
					continue;
				}	

				uiItem.enabled = ((GameManager.Instance.Character.Stat.Money >= uiItem.Item.Price) && (GameManager.Instance.Character.Inven.Exist(uiItem.Item) == false));
			}
		}

		public void OnExit()
		{
			GameManager.Instance.Character.AP++;
			EventManager.Instance.OnClickOK();
			this.gameObject.SetActive(false);
		}

		public int DiscountNum
		{
			get
			{
				return discountNum;
			}

			set
			{
				discountNum = value;
			}
		}
	}
}