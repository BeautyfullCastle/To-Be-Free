using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
    public class BuffList : Singleton<BuffList>
    {
        List<Item> buffList;

        public BuffList()
        {
            buffList = new List<Item>();
        }

        public Item Add(Item item)
        {
            if (buffList == null || item == null)
            {
                return null;
            }

            Item buffItem = new Item(item);
            buffList.Add(buffItem);
            
            return buffItem;
        }

        public bool Delete(Item item)
        {
            if (buffList == null || buffList.Count == 0 || item == null)
            {
                return false;
            }

            return buffList.Remove(item);
        }

        public void DoWork(eStartTime startTime, Character character)
        {
            foreach(Item item in buffList)
            {
                if (item.StartTime == startTime)
                {
                    item.ActivateEffect(character);
                    CheckDurationAndDelete(item, character.Inven);
                    
                    buffList.Remove(item);
                }
            }
        }

        private void CheckDurationAndDelete(Item item, Inventory inven)
        {
           if(item.Duration == eDuration.ONCE)
            {
                buffList.Remove(item);
                inven.DeleteItem(item);
            }
        }

        public bool Contains(Item item)
        {
            return buffList.Contains(item);
        }

        /*
        public bool CheckCondition(eActType actType) {
            foreach(Item item in buffList) {
                item.CheckCondition(actType);
            }
        }
        */
    }

    public class Buff
    {
        public Buff ItemToBuff(Item item)
        {
            return null;
        }
    }
}