using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Inventory
    {
        private List<Item> itemList;

        public Inventory()
        {
            itemList = new List<Item>();
        }
        
        public Inventory(List<Item> itemList)
        {
            this.itemList = itemList;
        }
        
        public Inventory(Inventory inven) : this(inven.itemList)
        {
        }

        public bool UseItem(Character character, int index, Effect effect)
        {
            if (itemList.Count <= index)
                return false;
            if (itemList[index] == null)
                return false;

            itemList[index].Use(character);//, effect);
            itemList.RemoveAt(index);

            return true;
        }

        public bool AddItem(Item item)
        {
            itemList.Add(item.DeepCopy());
            return true;
        }

        public Item FindItemByType(string bigType, string detailType)
        {
            for(int i=0; i<itemList.Count; ++i)
            {
                if (itemList[i].Effect.BigType == bigType)
                {
                    if (string.IsNullOrEmpty(detailType)) {
                        return itemList[i];
                    }
                    else
                    {
                        if(itemList[i].Effect.DetailType == detailType)
                        {
                            return itemList[i];
                        }
                    }
                }
            }

            Debug.Log("There's no " + bigType + " " + detailType + " item in inventory");
            return null;            
        }
    }
}