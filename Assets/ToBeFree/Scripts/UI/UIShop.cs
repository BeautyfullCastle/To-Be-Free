using UnityEngine;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
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
		
		public void Init()
		{
			grids = GetComponentsInChildren<UIGrid>();
			
			foreach (UIGrid grid in grids)
			{
				grid.Reposition();
			}

			discountNum = 0;

			this.gameObject.SetActive(false);
		}
		
		void OnEnable()
		{
			// 기본 아이템 세팅
			Item[] basicItemList = ItemManager.Instance.FindAll(ItemTag.FOOD);
			if (basicItemList == null)
				return;

			for (int i = 0; i < basicItemList.Length; ++i)
			{
				basicItems[i].SetInfo(basicItemList[i], UIItem.eBelong.SHOP);
			}

			// 도시 아이템 세팅
			cityItems[0].SetInfo(GameManager.Instance.Character.CurCity.Item, UIItem.eBelong.SHOP);

			// 랜덤 아이템 리스트 세팅
			List<Item> randomItemList = ItemManager.Instance.CopyTo();
			// 기본 아이템들 제외
			foreach (UIItem basicItem in basicItems)
			{
				randomItemList.Remove(basicItem.Item);
			}
			// 도시 아이템 제외
			randomItemList.Remove(cityItems[0].Item);
			// 인벤토리에 있는 아이템들 제외
			randomItemList.RemoveAll(x => GameManager.Instance.Character.Inven.Exist(x));

			System.Random r = new System.Random();
			for (int i = 0; i < randomItems.Count; ++i)
			{
				// 중도시에서는 랜덤 아이템이 3개만 나오게.
				//if(GameManager.Instance.Character.CurCity.Type==eNodeType.MIDDLECITY)
				//{
				//	if(i >= 3)
				//	{
				//		randomItems[i].gameObject.SetActive(false);
				//		continue;
				//	}
				//}
				randomItems[i].gameObject.SetActive(true);
				int randIndex = r.Next(0, randomItemList.Count - 1);
				randomItems[i].SetInfo(randomItemList[randIndex], UIItem.eBelong.SHOP);
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
			UIItem item = items.Find(x => x.name == uiItem.name);
			if(item == null)
			{
				return;
			}
			GameManager.Instance.Character.Inven.BuyItem(uiItem.Item, discountNum, GameManager.Instance.Character);
			
			CheckAllItems();
		}

		public void CheckAllItems()
		{
			foreach (UIItem uiItem in items)
			{
				if (uiItem == null || uiItem.Item == null)
				{
					uiItem.SetEnable(false);
					continue;
				}
				else if(uiItem.gameObject.activeSelf == false)
				{
					continue;
				}

				bool isEnoughMoney = (GameManager.Instance.Character.Stat.Money >= uiItem.Item.Price);
				if (basicItems.Contains(uiItem))
				{
					uiItem.SetEnable(isEnoughMoney);
					continue;
				}

				uiItem.SetEnable(isEnoughMoney && (GameManager.Instance.Character.Inven.Exist(uiItem.Item) == false));
			}
		}

		public void Refresh()
		{
			foreach(UIItem uiItem in this.items)
			{
				uiItem.Refresh();
			}
		}

		public void OnExit()
		{
			GameManager.Instance.Character.AP++;
			EventManager.Instance.OnClickOK();
			items.Clear();
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