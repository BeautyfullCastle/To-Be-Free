using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public enum eCity
    {
        KUNMING, CHENDU, CHONGQING, XINNIG, LANZHOU, XIAN, ZHENGZHOU, WUHAN, CHANGSHA, NANCHANG, NANNING, GUANGZHOU, XIANGGANG, FUZHOU,
        NANJING, SHANGHAI, JINAN, QINGDAO, YANTAI, SHIJIAZHUANG, TIANJIN, BEIJING, DALIAN, SHENYANG, DANDONG ,CHANGCHUN, JILIN, YANBIAN, HAERBIN,
        NONE, NULL
    }

    public enum eArea
    {
        YUNNANSHENG, SICHUANSHENG, QINGHAISHENG, SHANXISHENG, HUNANSHENG, GUANGXISHENG, GUANGDONGSHENG, FUJIANSHENG, JIANGSUSHENG,
        SHANDONGSHENG, HEBEISHENG, LIAONINGSHENG, JILINSHENG,
        NONE, NULL,
        MONGOLIA,
        SOUTHEAST_ASIA
    }

    public enum eRegion
    {
        AREA = 0, CITY, ALL,
        NULL
    }

    public enum eCitySize
    {
        SMALL, MIDDLE, BIG
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

        public City()
        {
            itemList = null;
        }

        public City(eCity name, eCitySize size, eArea area, Item[] itemList, int workingMoneyMin, int workingMoneyMax, int[] neighborList)
         : this()
        {
            this.name = name;
            this.size = size;
            this.area = area;
            this.itemList = itemList;
            this.workingMoneyMin = workingMoneyMin;
            this.workingMoneyMax = workingMoneyMax;
            this.neighborList = neighborList;
        }

        public City(City city)
         : this(city.name, city.size, city.area, city.itemList, city.workingMoneyMin, city.workingMoneyMax, city.neighborList)
        {
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
    }
}