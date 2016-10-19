using UnityEngine;

namespace ToBeFree
{
	public enum eStartTime
	{
		NOW, WORK, MOVE, REST, SHOP, INSPECT, INVESTIGATION, BROKER, QUEST, ESCAPE, TEST, SPECIALACT, DAY, NIGHT,
		DETENTION, NULL,
		WEEK,
		ABILITY,
	}

	public enum eDuration
	{
		ONCE, EQUIP, DAY, WEEK,
		NULL,
		REST_PATIENCE,
		DAY_TEST
	}

	public class Item
	{
		private Buff buff;

		private string name;
		private ItemTag tag;
		private int price;
		
		private int maximumStackableQuantity;

		public Item(string name, Buff buff, int price, ItemTag tag, int maximumStackableQuantity=1)
		{
			this.name = name;
			this.tag = tag;
			this.buff = buff;
			this.price = price;
			this.maximumStackableQuantity = maximumStackableQuantity;
		}

		public Item(Item item)
			: this(item.name, item.buff, item.price, item.tag, item.maximumStackableQuantity) { }
		
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

		public ItemTag Tag
		{
			get
			{
				return tag;
			}
		}
	}
}