using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eCity
	{
		NULL, TUMEN, YANBIAN, ANTU, DUNHUA, JIAOHE, JILIN, CHANGCHUN, GONGZHUULING, SIPING, YITONG,
		LIAOYUAN, KAIYUAN, TIELING, FUSHUN, SHENYANG, BENXI, LIAOYANG, ANSHAN, HAICHENG, YINGKOU, PENJIN,
		SHUANGLIAO, DIAOBINGSHAN, KANGPING, XINMIN, ZHNGWU, HEISHAN, TAIAN, LIAOZHONG, HORQINZUOYIHOUQI,
		BAISHAN, TONGLIAO, GAIZHOU, WAFANGDIAN, PULANDIAN, DALIAN, ZHUANGHE, DONGGANG, DANDONG, KUANDIAN,
		HUANREN, TONGHUA
	}

	public enum eArea
	{
		YUNNANSHENG, SICHUANSHENG, QINGHAISHENG, SHANXISHENG, HUNANSHENG, GUANGXISHENG, GUANGDONGSHENG, FUJIANSHENG, JIANGSUSHENG,
		SHANDONGSHENG, HEBEISHENG, LIAONINGSHENG, JILINSHENG,        
		MONGOLIA, SOUTHEAST_ASIA, NORTHKOREA,
		NONE, NULL,
	}

	public enum eRegion
	{
		AREA = 0, CITY, ALL,
		NULL
	}

	public enum eCitySize
	{
		SMALL, MIDDLE, BIG, NULL
	}

	public class City
	{
		private eCity name;
		private eCitySize size;
		private eArea area;
		private Item[] itemList;
		private int workingMoneyMin;
		private int workingMoneyMax;
		private int[] neighborList;

		private int distanceFromCharacter;
		private string name1;
		private eNodeType type;

		public City()
		{
			ItemList = null;
		}

		public City(eCity name, eCitySize size, eArea area, Item[] itemList, int workingMoneyMin, int workingMoneyMax, int[] neighborList)
		 : this()
		{
			this.name = name;
			this.size = size;
			this.area = area;
			this.ItemList = itemList;
			this.workingMoneyMin = workingMoneyMin;
			this.workingMoneyMax = workingMoneyMax;
			this.neighborList = neighborList;
		}

		public City(City city)
		 : this(city.name, city.size, city.area, city.ItemList, city.workingMoneyMin, city.workingMoneyMax, city.neighborList)
		{
		}

		public City(string name, eNodeType type)
		{
			this.name = EnumConvert<eCity>.ToEnum(name);
			this.type = type;
		}

		public void PrintNeighbors()
		{
			Debug.Log("Print " + this.name + "'s neighbors under below :");
			for (int i = 0; i < NeighborList.Count; ++i)
			{
				NeighborList[i].Print();
			}
		}

		public int CalcRandWorkingMoney()
		{
			System.Random r = new System.Random();
			return r.Next(this.workingMoneyMin, this.workingMoneyMax);
		}

		private void Print()
		{
			Debug.Log(this.name + ", " + this.size + ", " + this.area);
		}

		public bool IsNeighbor(string cityName)
		{
			foreach(City neighbor in NeighborList)
			{
				if(neighbor.Name == EnumConvert<eCity>.ToEnum(cityName))
				{
					return true;
				}
			}
			return false;
		}
		
		public eCity Name
		{
			get { return name; }
			set { name = value; }
		}

		public eArea Area
		{
			get
			{
				return area;
			}
		}

		public eCitySize Size
		{
			get
			{
				return size;
			}
		}

		public List<City> NeighborList
		{
			get
			{
				List<City> neighbors = new List<City>();
				for(int i=0; i<neighborList.Length; ++i)
				{
					neighbors.Add(CityManager.Instance.List[neighborList[i]]);
				}
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
	}
}