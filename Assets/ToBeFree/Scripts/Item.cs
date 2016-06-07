﻿using UnityEngine;

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
        private Buff buff;

        private string name;
        private int price;
        private int maximumStackableQuantity;

        //public Item(string name, Effect effect, eStartTime startTime, eDuration duration, bool isRestore,
        //            int amount, int price, int maximumStackableQuantity)
        //{
        //    this.name = name;
        //    //this.effect = effect;
        //    //this.startTime = startTime;
        //    //this.duration = duration;
        //    //this.isRestore = isRestore;
        //    //this.amount = amount;
        //    //this.price = price;
        //    this.maximumStackableQuantity = maximumStackableQuantity;
        //}

        public Item(string name, Buff buff, int price, int maximumStackableQuantity)
        {
            this.name = name;
            this.buff = buff;
            this.price = price;
            this.maximumStackableQuantity = maximumStackableQuantity;
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

            set
            {
                buff = value;
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