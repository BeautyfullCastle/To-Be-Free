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

        public void AddItem(Item item)
        {
            InventoryRecord inventoryRecord =
                   InventoryRecords.Find(x => (x.Item.Name == item.Name));

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

            // add item's buff in buff list also.
            BuffList.Instance.Add(item.Buff);
        }

        public void Delete(Item item)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.Item.Name == item.Name));

            this.Delete(inventoryRecord);
        }
        
        public void Delete(Buff buff)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.Item.Buff == buff));

            this.Delete(inventoryRecord);
        }

        private void Delete(InventoryRecord record)
        {
            if (record == null)
            {
                throw new Exception("There's no item like this in the inventory : " + record.Item.Name);
            }
            int remainQuantity = record.DeleteToQuantity(1);
            if (remainQuantity <= 0)
            {
                InventoryRecords.Remove(record);
            }
        }

        public Item GetRand()
        {
            System.Random r = new System.Random();
            int index = r.Next(0, InventoryRecords.Count - 1);
            return InventoryRecords[index].Item;
        }

        // TO DO : have to implement
        public Item GetTagRand(int iTag) { return null; }

        public Item FindItemByType(string bigType, string middleType, string detailType = "")
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => x.Item.Buff.Effect.BigType == bigType);
            if (inventoryRecord == null)
            {
                Debug.Log("There's no " + bigType + " item in inventory");
                return null;
            }
            else
            {
                Item item = inventoryRecord.Item;
                if (string.IsNullOrEmpty(item.Buff.Effect.MiddleType) || item.Buff.Effect.MiddleType == middleType)
                {
                    if (string.IsNullOrEmpty(item.Buff.Effect.DetailType) || item.Buff.Effect.DetailType == detailType)
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