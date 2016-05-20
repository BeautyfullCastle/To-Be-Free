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
        private int maximumStackableQuantity;

        private BuffList buffList;
        

        public Item(string name, Effect effect, eStartTime startTime, eDuration duration,
                    int amount, int price, int maximumStackableQuantity)
        {
            this.name = name;
            this.effect = effect;
            this.startTime = startTime;
            this.duration = duration;
            this.amount = amount;
            this.price = price;
            this.maximumStackableQuantity = maximumStackableQuantity;
        }

        public Item(Item item)
         : this(item.name, item.effect, item.startTime, item.duration, item.amount, item.price, item.maximumStackableQuantity)
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
            item.maximumStackableQuantity = this.maximumStackableQuantity;

            return item;
        }

        public void Use(Character character)
        { 
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

        public int MaximumStackableQuantity { get; private set; }
    }
}