using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class Inventory
	{
		private int maxSlot;
		public List<Item> list;

		public Inventory(int maxSlots)
		{
			this.maxSlot = maxSlots;
			list = new List<Item>();
		}

		public Inventory(List<ItemSaveData> dataList, int maxSlot)
		{
			this.maxSlot = maxSlot;
			list = new List<Item>(dataList.Count);
			for (int i = 0; i < dataList.Count; ++i)
			{
				this.list.Add(new Item(ItemManager.Instance.GetByIndex(dataList[i].index)));
			}
		}
		
		public void BuyItem(Item item, int discountNum, Character character)
		{
			if (list.Count >= maxSlot)
			{
				NGUIDebug.Log("There is no more space in the inventory.");
				return;
			}

			int price = Mathf.Max(item.Price - discountNum, 1, item.Price);
			if (character.Stat.Money < price)
			{
				NGUIDebug.Log("Shop : Money is not enough to buy.");
				return;
			}

			AddItem(item);
			character.Stat.Money -= price;
			AudioManager.Instance.Find("buy_item").Play();
		}

		public void AddItem(Item item)
		{
			if (list.Count >= maxSlot)
			{
				NGUIDebug.Log("There is no more space in the inventory.");
			}
			else
			{
				list.Add(new Item(item));
				GameObject.FindObjectOfType<UIInventory>().AddItem(item);

				NGUIDebug.Log(item.Name);
			}
		}

		public bool Exist(Item item)
		{
			return list.Exists(x => x.Index == item.Index);
		}

		public List<Item> FindAll(ItemTag tag)
		{
			return list.FindAll(x => x.Tag == tag);
		}

		public Item Find(Item item)
		{
			return list.Find(x => x.Index == item.Index);
		}

		public IEnumerator Delete(Item item, Character character, int siblingIndex = -1)
		{
			if (item == null)
			{
				yield break;
			}
			Item foundItem = this.Find(item);
			if(foundItem == null)
			{
				Debug.LogError("Item " + item.Name + "is not exist in this inventory.");
				yield break;
			}

			UIInventory uiInventory = GameManager.Instance.uiInventory;
			if(uiInventory == null)
			{
				yield break;
			}
			
			if (siblingIndex == -1)
			{
				uiInventory.DeleteItem(foundItem);
			}
			else
			{
				UIItem uiItem = uiInventory.GetByGridIndex(siblingIndex);
				if(uiItem != null)
				{
					uiInventory.DeleteItem(uiItem);
				}
			}
			
			list.Remove(foundItem);

			yield return BuffManager.Instance.Delete(foundItem.Buff, character);
		}
		
		public IEnumerator Delete(Buff buff, Character character)
		{
			Item findedItem = list.Find(x => (x.Buff == buff));

			if (findedItem != null)
			{
				yield return this.Delete(findedItem, character);
			}
		}

		public Item GetRand()
		{
			if(list.Count <= 0)
			{
				return null;
			}
			System.Random r = new System.Random();
			int index = r.Next(0, list.Count - 1);
			return list[index];
		}

		// TO DO : have to implement
		public Item GetTagRand(int iTag) { return null; }
		

		public void AddSlot()
		{
			this.maxSlot++;
		}

		public IEnumerator CheckItem(eStartTime startTime, bool isActive, Character character)
		{
			// 아이템 활/비활성화
			foreach(Item item in list)
			{
				SetItemEnabled(item, item.Buff.StartTime == startTime && isActive);
			}

			// 장비템 Effect Activate/Deactivate
			List<Item> items = list.FindAll(x => x.Buff.StartTime == startTime);
			for (int i = 0; i < items.Count; ++i)
			{
				if (items[i].Buff.Duration == eDuration.EQUIP)
				{
					if (isActive)
						yield return UseItem(items[i], character);
					else
						yield return DeactvieItems(items[i], character);
				}
				
			}
		}

		public void SetItemEnabled(Item item, bool isEnabled)
		{
			List<UIItem> uiItems = GameObject.FindObjectOfType<UIInventory>().FindAll(item);

			foreach(UIItem uiItem in uiItems)
			{
				uiItem.SetEnable(isEnabled);
			}
		}

		public IEnumerator UseItem(Item item, Character character)
		{
			if (item.Buff.Duration == eDuration.ONCE)
			{
				if(item.Buff.EffectAmountList[0].Effect.SubjectType == eSubjectType.STAT)
				{
					yield return DiceTester.Instance.demo.AddDie();
				}
				else
				{
					yield return item.Buff.ActivateEffect(character);
				}

				AudioManager.Instance.Find("use_item").Play();
				yield return Delete(item, character);
			}
			else
			{
				yield return item.Buff.ActivateEffect(character);
			}
		}

		public IEnumerator DeactvieItems(Item item, Character character)
		{
			yield return item.Buff.DeactivateEffect(character);
		}

		public int MaxSlot
		{
			get
			{
				return maxSlot;
			}

			set
			{
				maxSlot = value;
			}
		}
	}
}