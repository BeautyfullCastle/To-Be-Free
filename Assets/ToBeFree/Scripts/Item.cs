using UnityEngine;

namespace ToBeFree
{
    public enum eStartTime
    {
        NOW, WORK, MOVE, REST, SHOP, INSPECT, INFO, BROKER, QUEST, ESCAPE, TEST, SPECIALACT,
        NULL
    }

    public enum eDuration
    {
        ONCE, EQUIP, DAY, PAT_TEST_REST,
        NULL
    }

    public class Item
    {
        private Buff buff;

        private string name;
        private int price;
        private int maximumStackableQuantity;
        

        public Item(string name, Buff buff, int price, int maximumStackableQuantity=1)
        {
            this.name = name;
            this.buff = new Buff(buff);
            this.price = price;
            this.maximumStackableQuantity = maximumStackableQuantity;

            ItemManager.Instance.Add(this);
        }

        public Item(Item item)
            : this(item.name, item.buff, item.price, item.maximumStackableQuantity) { }

        //public Item(Item item)
        // : this(item.name, item.effect, item.startTime, item.duration, item.isRestore, item.amount, item.price, item.maximumStackableQuantity)
        //{
        //}

        public Item DeepCopy()
        {
            Item item = (Item)this.MemberwiseClone();
            item.name = this.name;
            item.buff = this.buff;
            item.maximumStackableQuantity = this.maximumStackableQuantity;

            return item;
        }
        

        public string Name { get { return name; } }

        //public Effect Effect
        //{
        //    get
        //    {
        //        return effect;
        //    }

        //    set
        //    {
        //        effect = value;
        //    }
        //}

        //public int Amount
        //{
        //    get
        //    {
        //        return amount;
        //    }
        //}

        public int MaximumStackableQuantity { get; private set; }

        public Buff Buff
        {
            get
            {
                return buff;
            }            
        }

        //public eStartTime StartTime
        //{
        //    get
        //    {
        //        return startTime;
        //    }

        //    set
        //    {
        //        startTime = value;
        //    }
        //}

        //public eDuration Duration
        //{
        //    get
        //    {
        //        return duration;
        //    }

        //    set
        //    {
        //        duration = value;
        //    }
        //}

        //public bool IsRestore
        //{
        //    get
        //    {
        //        return isRestore;
        //    }

        //    set
        //    {
        //        isRestore = value;
        //    }
        //}
    }
}