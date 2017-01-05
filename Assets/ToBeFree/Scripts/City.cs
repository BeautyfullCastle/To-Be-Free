using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	[Serializable]
	public class CitySaveData
	{
		public CitySaveData(int index, int itemIndex)
		{
			this.index = index;
			this.itemIndex = itemIndex;
		}

		public int index;
		public int itemIndex;
	}

	public class City
	{
		private int index;
		private string name;
		private Item item;
		private int workingMoneyMin;
		private int workingMoneyMax;
		private List<City>[] neighbors;

		private int distanceFromCharacter;
		private eNodeType type;
		private IconCity iconCity;
		

		public City(string name, eNodeType type, IconCity iconCity)
		{
			this.name = name;
			this.Type = type;
			this.iconCity = iconCity;

			//if (this.Type == eNodeType.BIGCITY)
			//{
			//	this.workingMoneyMin = 2;
			//	this.workingMoneyMax = 4;
			//}
			//else if (this.Type == eNodeType.MIDDLECITY)
			//{
			//	this.workingMoneyMin = 1;
			//	this.workingMoneyMax = 3;
			//}
			//else if (this.Type == eNodeType.SMALLCITY)
			//{
			//	this.workingMoneyMin = 0;
			//	this.workingMoneyMax = 2;
			//}
			//else
			//{
				this.workingMoneyMin = 0;
				this.workingMoneyMax = 0;
			//}

			item = CityManager.Instance.SetCityItem();
		}

		public City(City curCity) : this(curCity.name, curCity.Type, curCity.iconCity)
		{
		}
		
		public void InitNeighbors(List<City>[] neighbors)
		{
			this.neighbors = neighbors;
		}

		public int CalcRandWorkingMoney()
		{
			System.Random r = new System.Random();
			return r.Next(this.workingMoneyMin, this.workingMoneyMax);
		}

		private void Print()
		{
			Debug.Log(this.name);
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public void Load(CitySaveData data)
		{
			if (this.Type == eNodeType.BIGCITY)// || this.Type == eNodeType.MIDDLECITY)
			{
				this.item = ItemManager.Instance.GetByIndex(data.itemIndex);
			}
		}

		public List<City>[] Neighbors
		{
			get
			{
				return neighbors;
			}
		}

		public int Distance
		{
			get
			{
				return distanceFromCharacter;
			}
			set
			{
				distanceFromCharacter = value;
			}
		}

		public Item Item
		{
			get
			{
				return item;
			}
		}

		public eNodeType Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
			}
		}

		public IconCity IconCity
		{
			get
			{
				return iconCity;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}
	}
}