using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class City
	{
		private string name;
		private Item[] itemList;
		private int workingMoneyMin;
		private int workingMoneyMax;
		private List<City> neighbors;

		private int distanceFromCharacter;
		private eNodeType type;

		public City()
		{
			ItemList = null;
		}

		public City(string name, eNodeType type)
		{
			this.name = name;
			this.Type = type;
			if (this.Type == eNodeType.BIGCITY)
			{
				this.workingMoneyMin = 2;
				this.workingMoneyMax = 4;
			}
			else if (this.Type == eNodeType.MIDDLECITY)
			{
				this.workingMoneyMin = 1;
				this.workingMoneyMax = 3;
			}
			else if (this.Type == eNodeType.SMALLCITY)
			{
				this.workingMoneyMin = 0;
				this.workingMoneyMax = 2;
			}
			else
			{
				this.workingMoneyMin = 0;
				this.workingMoneyMax = 0;
			}
		}

		public City(City curCity) : this(curCity.name, curCity.Type)
		{
		}

		public void InitNeighbors(List<City> neighbors)
		{
			this.neighbors = neighbors;
		}

		public void PrintNeighbors()
		{
			Debug.Log("Print " + this.name + "'s neighbors under below :");
			for (int i = 0; i < Neighbors.Count; ++i)
			{
				Neighbors[i].Print();
			}
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

		public bool IsNeighbor(string cityName)
		{
			foreach(City neighbor in Neighbors)
			{
				if(neighbor.Name == cityName)
				{
					return true;
				}
			}
			return false;
		}
		
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public List<City> Neighbors
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

		public Item[] ItemList
		{
			get
			{
				return itemList;
			}
			private set
			{
				itemList = value;
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
	}
}