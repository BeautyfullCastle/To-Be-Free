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
        private int price;

        private BuffList buffList;

        public string Name { get { return name; } }

        public Effect Effect
        {
            get
            {
                return effect;
            }

            set
            {
                effect = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
        }

        public Item(string name, Effect effect, eStartTime startTime, eDuration duration,
                    int amount, int price)
        {
            this.name = name;
            this.effect = effect;
            this.startTime = startTime;
            this.duration = duration;
            this.amount = amount;
            this.price = price;
        }

        public Item(Item item)
         : this(item.name, item.effect, item.startTime, item.duration, item.amount, item.price)
        {

        }

        public Item DeepCopy()
        {
            Item item = (Item)this.MemberwiseClone();
            item.name = this.name;
            item.effect = this.effect;
            item.startTime = this.startTime;
            item.duration = this.duration;
            item.amount = this.amount;
            item.price = this.price;

            return item;
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