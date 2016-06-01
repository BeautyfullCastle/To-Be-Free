using UnityEngine;

namespace ToBeFree
{
    public enum eStartTime
    {
        NOW, WORK, MOVE, REST, SHOP, INSPECT, INFO, BROKER, QUEST, ESCAPE, TEST, SPECIALACTION
    }

    public enum eDuration
    {
        ONCE, EQUIP, DAY, PAT_TEST_REST
    }

    public class Item
    {
        private string name;
        private Effect effect;
        private eStartTime startTime;
        private eDuration duration;
        private bool isRestore;
        private int amount;
        private int price;
        private int maximumStackableQuantity;

        public Item(string name, Effect effect, eStartTime startTime, eDuration duration, bool isRestore,
                    int amount, int price, int maximumStackableQuantity)
        {
            this.name = name;
            this.effect = effect;
            this.startTime = startTime;
            this.duration = duration;
            this.isRestore = isRestore;
            this.amount = amount;
            this.price = price;
            this.maximumStackableQuantity = maximumStackableQuantity;
        }

        public Item(Item item)
         : this(item.name, item.effect, item.startTime, item.duration, item.isRestore, item.amount, item.price, item.maximumStackableQuantity)
        {
        }

        public Item DeepCopy()
        {
            Item item = (Item)this.MemberwiseClone();
            item.name = this.name;
            item.effect = this.effect;
            item.startTime = this.startTime;
            item.duration = this.duration;
            item.isRestore = this.isRestore;
            item.amount = this.amount;
            item.price = this.price;
            item.maximumStackableQuantity = this.maximumStackableQuantity;

            return item;
        }

        public void ActivateEffect(Character character)
        {
            Debug.Log("Item effect activate");

            if (effect == null)
                return;

            effect.Activate(character, amount);
        }

        public void DeactivateEffect(Character character)
        {
            if (effect == null)
                return;

            effect.Activate(character, -amount);

            Debug.Log("Item.Use");
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

        public eStartTime StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                startTime = value;
            }
        }

        public eDuration Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }

        public bool IsRestore
        {
            get
            {
                return isRestore;
            }

            set
            {
                isRestore = value;
            }
        }
    }
}