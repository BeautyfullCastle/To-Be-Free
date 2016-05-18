using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree
{
    public class BuffList : MonoBehaviour
    {
        List<Item> buffList = null;

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

        public Item Delete(Item item)
        {
            if (buffList == null || buffList.Count == 0 || item == null)
            {
                return null;
            }

            return null;
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