using System;

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
		ONCE, EQUIP, DAY, TODAY,
		NULL,
		REST_PATIENCE,
		DAY_TEST
	}

	[Serializable]
	public class ItemSaveData
	{
		public ItemSaveData(int index)
		{
			this.index = index;
		}
		public int index;
	}

	public class Item
	{
		private readonly int index;
		private string name;
		private readonly Buff buff;
		private readonly ItemTag tag;
		private readonly int price;
		private readonly int maximumStackableQuantity;

		public Item(int index, string name, Buff buff, int price, ItemTag tag, int maximumStackableQuantity=1)
		{
			this.index = index;
			this.name = name;
			this.tag = tag;
			this.buff = buff;
			this.price = price;
			this.maximumStackableQuantity = maximumStackableQuantity;
		}

		public Item(Item item)
			: this(item.index, item.name, item.buff, item.price, item.tag, item.maximumStackableQuantity) { }
		
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public int MaximumStackableQuantity
		{
			get
			{
				return maximumStackableQuantity;
			}
		}

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

		public int Index
		{
			get
			{
				return index;
			}
		}

		public string EngName
		{
			get
			{
				return ItemManager.Instance.GetEngName(this.index);
			}
		}
	}
}