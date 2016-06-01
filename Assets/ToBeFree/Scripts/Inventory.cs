using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Inventory
    {
        private int maxSlots;
        public List<InventoryRecord> InventoryRecords;

        public Inventory(int maxSlots)
        {
            this.maxSlots = maxSlots;
            InventoryRecords = new List<InventoryRecord>();
        }

        public void UseItem(Item item, Character character)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.InventoryItem == item));

            if (inventoryRecord == null)
            {
                throw new Exception("There's no item like this in the inventory : " + item.Name);
            }
            inventoryRecord.InventoryItem.ActivateEffect(character);
        }

        public List<Item> CheckItemStartTime(eStartTime startTime, Character character)
        {
            List<Item> itemsToDeactive = new List<Item>();

            foreach (InventoryRecord record in InventoryRecords)
            {
                if (record.InventoryItem.StartTime == startTime)
                {
                    UseItem(record.InventoryItem, character);
                    if (record.InventoryItem.IsRestore)
                    {
                        itemsToDeactive.Add(record.InventoryItem.DeepCopy());
                    }
                }
            }

            InventoryRecords.RemoveAll(x => (x.InventoryItem.Duration == eDuration.ONCE)
                                            && (x.InventoryItem.StartTime == startTime));

            return itemsToDeactive;
        }

        public List<Item> UseStatTestItems(string testStat, Character character)
        {
            List<Item> itemsToDeactive = new List<Item>();
            foreach (InventoryRecord record in InventoryRecords)
            {
                if ((record.InventoryItem.StartTime == eStartTime.TEST)
                    && (record.InventoryItem.Effect.BigType == "STAT")
                    && (record.InventoryItem.Effect.MiddleType == testStat))
                {
                    UseItem(record.InventoryItem, character);
                    if (record.InventoryItem.IsRestore)
                    {
                        itemsToDeactive.Add(record.InventoryItem);
                    }
                }
            }

            InventoryRecords.RemoveAll(x => (x.InventoryItem.StartTime == eStartTime.TEST)
                                            && (x.InventoryItem.Duration == eDuration.ONCE)
                                            && (x.InventoryItem.Effect.BigType == "STAT")
                                           && (x.InventoryItem.Effect.MiddleType == testStat));

            return itemsToDeactive;
        }

        public void AddItem(Item item)
        {
            InventoryRecord inventoryRecord =
                   InventoryRecords.Find(x => (x.InventoryItem.Name == item.Name));

            if (inventoryRecord != null)
            {
                inventoryRecord.AddToQuantity(1);
            }
            else if (inventoryRecord == null)
            {
                if (InventoryRecords.Count >= maxSlots)
                {
                    throw new Exception("There is no more space in the inventory.");
                }
                InventoryRecords.Add(new InventoryRecord(item, 1));
            }
        }

        public void DeleteItem(Item item)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.InventoryItem.Name == item.Name));

            if (inventoryRecord == null)
            {
                throw new Exception("There's no item like this in the inventory : " + item.Name);
            }
            int remainQuantity = inventoryRecord.DeleteToQuantity(1);
            if (remainQuantity <= 0)
            {
                InventoryRecords.Remove(inventoryRecord);
            }
        }

        public Item GetRand()
        {
            System.Random r = new System.Random();
            int index = r.Next(0, InventoryRecords.Count - 1);
            return InventoryRecords[index].InventoryItem;
        }

        // TO DO : have to implement
        public Item GetTagRand(int iTag) { return null; }

        public Item FindItemByType(string bigType, string middleType, string detailType = "")
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => x.InventoryItem.Effect.BigType == bigType);
            if (inventoryRecord == null)
            {
                Debug.Log("There's no " + bigType + " item in inventory");
                return null;
            }
            else
            {
                Item item = inventoryRecord.InventoryItem;
                if (string.IsNullOrEmpty(item.Effect.MiddleType) || item.Effect.MiddleType == middleType)
                {
                    if (string.IsNullOrEmpty(item.Effect.DetailType) || item.Effect.DetailType == detailType)
                    {
                        return item;
                    }
                }
                Debug.Log("There's no " + bigType + " " + middleType + " " + detailType + " item in inventory");
                return null;
            }
        }

        public void AddSlot()
        {
            this.maxSlots++;
        }

        public class InventoryRecord
        {
            public Item InventoryItem { get; private set; }
            public int Quantity { get; private set; }

            public InventoryRecord(Item item, int quantity)
            {
                InventoryItem = item;
                Quantity = quantity;
            }

            public void AddToQuantity(int amountToAdd)
            {
                if (Quantity + amountToAdd > InventoryItem.MaximumStackableQuantity)
                {
                    Debug.LogError(InventoryItem.Name + "'s quantity is full : " + Quantity);
                    return;
                }
                Quantity += amountToAdd;
            }

            public int DeleteToQuantity(int amount)
            {
                if (Quantity - amount < 0)
                {
                    Debug.LogError(InventoryItem.Name + "'s quantity is lower than the amount you want. ");
                    return Quantity;
                }
                Quantity -= amount;
                return Quantity;
            }
        }
    }
}