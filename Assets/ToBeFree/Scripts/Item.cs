using UnityEngine;

namespace ToBeFree
{
    public enum eStartTime
    {
        NOW, WORK, MOVE, REST, SHOP, INSPECT, INFO, BROKER, QUEST, ESCAPE, TEST, SPECIALACT, DAY, NIGHT,
        DETENTION, NULL,
        WEEK,
    }

    public enum eDuration
    {
        ONCE, EQUIP, DAY, WEEK,
        NULL,
        REST_PATIENCE
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
            this.buff = buff;
            this.price = price;
            this.maximumStackableQuantity = maximumStackableQuantity;
        }

        public Item(Item item)
            : this(item.name, item.buff, item.price, item.maximumStackableQuantity) { }
        
        public Item DeepCopy()
        {
            Item item = (Item)this.MemberwiseClone();
            item.name = this.name;
            item.buff = this.buff;
            item.maximumStackableQuantity = this.maximumStackableQuantity;

            return item;
        }
        
        public string Name { get { return name; } }
        
        public int MaximumStackableQuantity { get; private set; }

        public Buff Buff
        {
            get
            {
                return buff;
            }            
        }

        public int Price
        {
            get
            {
                return price;
            }
        }
    }
}