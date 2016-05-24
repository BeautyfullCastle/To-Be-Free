using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Inventory
    {
        private int maxSlots;
        public List<InventoryRecord> InventoryRecords = new List<InventoryRecord>();
        
        public Inventory(int maxSlots)
        {
            this.maxSlots = maxSlots;
        }

        public void UseItem(Item item, Character character)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.InventoryItem.Name == item.Name));

            if (inventoryRecord == null)
            {
                throw new Exception("There's no item like this in the inventory : " + item.Name);
            }
            inventoryRecord.InventoryItem.Use(character);

            DeleteItem(item);
        }

        public void AddItem(Item item)
        {
            InventoryRecord inventoryRecord =
                   InventoryRecords.Find(x => (x.InventoryItem.Name == item.Name));

            if (inventoryRecord != null)
            {
                inventoryRecord.AddToQuantity(1);
            }
            else if(inventoryRecord == null)
            {
                if (InventoryRecords.Count > maxSlots)
                {
                    throw new Exception("There is no more space in the inventory.");
                }
                InventoryRecords.Add(new InventoryRecord(item, 1));
            }
        }

        public void DeleteItem(Item item)
        {
            InventoryRecord inventoryRecord = InventoryRecords.Find(x => (x.InventoryItem.Name == item.Name));

            if(inventoryRecord == null)
            {
                throw new Exception("There's no item like this in the inventory : " + item.Name);
            }
            inventoryRecord.DeleteToQuantity(1);
        }

        public Item FindItemByType(string bigType, string detailType)
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
                if (string.IsNullOrEmpty(item.Effect.DetailType) || item.Effect.DetailType == detailType)
                {
                    return item;
                }
                Debug.Log("There's no " + bigType + " " + detailType + " item in inventory");
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
                if(Quantity + amountToAdd > InventoryItem.MaximumStackableQuantity)
                {
                    Debug.LogError(InventoryItem.Name + "'s quantity is full : " + Quantity);
                    return;
                }
                Quantity += amountToAdd;
            }

            public void DeleteToQuantity(int amount)
            {
                if(Quantity - amount < 0)
                {
                    Debug.LogError(InventoryItem.Name + "'s quantity is lower than the amount you want. ");
                    return;
                }
                Quantity -= amount;
            }
        }
    }
}