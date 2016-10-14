using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class Inventory
	{
		private int maxSlots;
		public List<Item> list;
		
		public Inventory(int maxSlots)
		{
			this.maxSlots = maxSlots;
			list = new List<Item>();
		}

		public void BuyItem(Item item, int discountNum, Character character)
		{
			if (list.Count >= maxSlots)
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

			AddItem(item, character);
			character.Stat.Money -= price;
		}

		public void AddItem(Item item, Character character)
		{
			if (list.Count >= maxSlots)
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
			return list.Exists(x => x.Name == item.Name);
		}

		public IEnumerator Delete(Item item, Character character)
		{
			Item findedItem = list.Find(x => (x.Name == item.Name));
			list.Remove(findedItem);
			GameObject.FindObjectOfType<UIInventory>().DeleteItem(findedItem);

			yield return BuffManager.Instance.Delete(item.Buff, character);
			
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
			this.maxSlots++;
		}

		public IEnumerator CheckItem(eStartTime startTime, bool isActive, Character character)
		{
			List<Item> items = list.FindAll(x => x.Buff.StartTime == startTime);
			
			for (int i = 0; i < items.Count; ++i)
			{
				
				// 장비템은 알아서 사용됨
				if (items[i].Buff.Duration == eDuration.EQUIP)
				{
					if (isActive)
						yield return UseItem(items[i], character);
					else
						yield return DeactvieItems(items[i], character);
				}
				// 소모 아이템은 활성화/비활성화
				else if (items[i].Buff.Duration == eDuration.ONCE)
				{
					SetItemEnabled(items[i], isActive);
				}
			}
		}

		public void SetItemEnabled(Item item, bool isEnabled)
		{
			List<UIItem> uiItems = GameObject.FindObjectOfType<UIInventory>().FindAll(item);

			foreach(UIItem uiItem in uiItems)
			{
				uiItem.GetComponent<UIButton>().isEnabled = isEnabled;
			}
		}

		public IEnumerator UseItem(Item item, Character character)
		{
			yield return item.Buff.ActivateEffect(character);
			if (item.Buff.Duration == eDuration.ONCE)
			{
				yield return Delete(item, character);
			}
		}

		public IEnumerator DeactvieItems(Item item, Character character)
		{
			yield return item.Buff.DeactivateEffect(character);
		}
	}
}