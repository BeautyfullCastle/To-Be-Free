using UnityEngine;
using System.Collections;

namespace ToBeFree
{
    public enum eStartTime
    {
        NOW, ACT
    }
    public enum eDuration
    {
        ONCE, DAY, EQUIP, FOREVER
    }

    public class Item
    {
        private string name;
        private Effect effect;
        private eStartTime startTime;
        private eDuration duration;
        private int amount;
        private int number;
        private int price;

        private BuffList buffList;

        public string Name { get { return name; } }

        public Item(string name, Effect effect, eStartTime startTime, eDuration duration,
                    int amount, int number, int price)
        {
            this.name = name;
            this.effect = effect;
            this.startTime = startTime;
            this.duration = duration;
            this.amount = amount;
            this.number = number;
            this.price = price;
        }

        public Item(Item item)
         : this(item.name, item.effect, item.startTime, item.duration, item.amount, item.number, item.price)
        {

        }

        public void Use(Character character)
        { //, Effect esfeffect) {

            if (effect == null)
                return;

            if (startTime == eStartTime.NOW)
            {
                effect.Activate(character, amount);
                Debug.Log("Item.Use");
            }
            //buffList.Add(this);
        }

        /*
        public bool CheckActCondition() {
            if(type == eType)
            if(duration == eDuration.EQUIP) {

            }
        }
        */
    }
}