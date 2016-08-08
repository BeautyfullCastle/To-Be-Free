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

        public IEnumerator BuyItem(Item item, int discountNum, Character character)
        {
            if (list.Count >= maxSlots)
            {
                NGUIDebug.Log("There is no more space in the inventory.");
                yield break;
            }

            yield return AddItem(item, character);

            int price = Mathf.Max(item.Price - discountNum, 1, item.Price);
            if (character.Stat.Money < price)
            {
                NGUIDebug.Log("Shop : Money is not enough to buy.");
                yield break;
            }
            character.Stat.Money -= price;
        }

        public IEnumerator AddItem(Item item, Character character)
        {
            if(item.Buff.StartTime == eStartTime.NOW)
            {
                yield return item.Buff.ActivateEffect(character);
                yield break;
            }

            AddingItem(item, character);

            yield return null;
        }

        public void AddingItem(Item item, Character character)
        {
            if (list.Count >= maxSlots)
            {
                NGUIDebug.Log("There is no more space in the inventory.");
            }
            else
            {
                list.Add(new Item(item));
                character.Stat.AddFood(item);
                GameObject.FindObjectOfType<UIInventory>().AddItem(item);
                NGUIDebug.Log(item.Name);
            }
        }

        public bool Exist(Item item)
        {
            return list.Exists(x => x == item);
        }

        public IEnumerator Delete(Item item, Character character)
        {
            Item findedItem = list.Find(x => (x.Name == item.Name));
            list.Remove(findedItem);
            character.Stat.DeleteFood(item);
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

        public IEnumerator CheckItem(eStartTime startTime, Character character)
        {
            List<Item> items = list.FindAll(x => x.Buff.StartTime == startTime);
            
            for (int i = 0; i < items.Count; ++i)
            {
                yield return UseItem(items[i], character);
                if (startTime == eStartTime.NIGHT)
                {
                    yield break;
                }
            }
        }

        private IEnumerator UseItem(Item item, Character character)
        {
            yield return item.Buff.ActivateEffect(character);
            if (item.Buff.Duration == eDuration.ONCE)
            {
                yield return Delete(item, character);
            }
        }
    }
}