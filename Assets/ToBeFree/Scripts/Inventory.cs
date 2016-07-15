using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Inventory
    {
        private int maxSlots;
        public List<InventoryRecord> InventoryRecords;

        public delegate void UpdateListHandler(Item item);
        static public event UpdateListHandler AddedItem = delegate {};
        static public event UpdateListHandler DeletedItem = delegate { };

        public Inventory(int maxSlots)
        {
            this.maxSlots = maxSlots;
            InventoryRecords = new List<InventoryRecord>();
        }

        public IEnumerator BuyItem(Item item, Character character)
        {
            if (character.Stat.Money < item.Price)
            {
                NGUIDebug.Log("Shop : Money is not enough to buy.");
                yield break;
            }
            character.Stat.Money -= item.Price;

            yield return AddItem(item, character);
        }

        public IEnumerator AddItem(Item item, Character character)
        {
            if(item.Buff.StartTime == eStartTime.NOW)
            {
                yield return item.Buff.ActivateEffect(character);

                yield break;
            }

            {
                if (InventoryRecords.Count >= maxSlots)
                {
                    throw new Exception("There is no more space in the inventory.");
                }
                InventoryRecords.Add(new InventoryRecord(item, 1));
                AddedItem(item);
            }

            // add item's buff in buff list also.
            //yield return BuffManager.Instance.Add(item.Buff);
        }

        public bool Exist(Item item)
        {
            return InventoryRecords.Exists(x => x.Item == item);
        }

        public void Delete(Item item, Character character)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.Item.Name == item.Name));
            InventoryRecords.Remove(inventoryRecord);

            BuffManager.Instance.Delete(item.Buff, character);

            DeletedItem(item);
        }
        
        public void Delete(Buff buff, Character character)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.Item.Buff == buff));

            if (inventoryRecord==null)
            {
                return;
            }
            this.Delete(inventoryRecord.Item, character);
        }

        public Item GetRand()
        {
            if(InventoryRecords.Count <= 0)
            {
                return null;
            }
            System.Random r = new System.Random();
            int index = r.Next(0, InventoryRecords.Count - 1);
            return InventoryRecords[index].Item;
        }

        // TO DO : have to implement
        public Item GetTagRand(int iTag) { return null; }

        // TODO : have to fix to EffectAmount list.
        public Item FindItemByType(eSubjectType subjectType, eVerbType verbType, eObjectType objectType = eObjectType.NONE)
        {
            //InventoryRecord inventoryRecord = InventoryRecords.Find(x => x.Item.Buff.Effect.SubjectType == subjectType);
            //if (inventoryRecord == null)
            //{
            //    Debug.Log("There's no " + subjectType + " item in inventory");
            //    return null;
            //}
            //else
            //{
            //    Item item = inventoryRecord.Item;

            //    // WARNING : Maybe this code have bug cause of NONE checking.
                
            //    if ((item.Buff.Effect.VerbType == eVerbType.NONE) || item.Buff.Effect.VerbType == verbType)
            //    {
            //        if ((item.Buff.Effect.ObjectType == eObjectType.NONE) || item.Buff.Effect.ObjectType == objectType)
            //        {
            //            return item;
            //        }
            //    }
            //    Debug.Log("There's no " + subjectType + " " + verbType + " " + objectType + " item in inventory");
            //    return null;
            //}
            return null;
        }

        public void AddSlot()
        {
            this.maxSlots++;
        }

        public IEnumerator CheckItem(eStartTime startTime, Character character)
        {
            List<InventoryRecord> records = InventoryRecords.FindAll(x => x.Item.Buff.StartTime == startTime);
            for (int i = 0; i < records.Count; ++i)
            {
                yield return UseItem(records[i].Item, character);
            }
        }

        private IEnumerator UseItem(Item item, Character character)
        {
            yield return item.Buff.ActivateEffect(character);
            if (item.Buff.Duration == eDuration.ONCE)
            {
                Delete(item, character);
            }
        }

        public class InventoryRecord
        {
            public Item Item { get; private set; }
            public int Quantity { get; private set; }

            public InventoryRecord(Item item, int quantity)
            {
                Item = item;
                Quantity = quantity;
            }

            public void AddToQuantity(int amountToAdd)
            {
                if (Quantity + amountToAdd > Item.MaximumStackableQuantity)
                {
                    Debug.LogError(Item.Name + "'s quantity is full : " + Quantity);
                    return;
                }
                Quantity += amountToAdd;
            }

            public int DeleteToQuantity(int amount)
            {
                if (Quantity - amount < 0)
                {
                    Debug.LogError(Item.Name + "'s quantity is lower than the amount you want. ");
                    return Quantity;
                }
                Quantity -= amount;
                return Quantity;
            }
        }
    }
}