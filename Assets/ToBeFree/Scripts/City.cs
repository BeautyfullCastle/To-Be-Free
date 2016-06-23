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

        private List<City> neighborList;
        private int distanceFromCharacter;

        public City()
        {
            neighborList = new List<City>();
            itemList = null;
        }

        public City(eCity name, eCitySize size, eArea area, Item[] itemList, int workingMoneyMin, int workingMoneyMax)
         : this()
        {
            this.name = name;
            this.size = size;
            this.area = area;
            this.itemList = itemList;
            this.workingMoneyMin = workingMoneyMin;
            this.workingMoneyMax = workingMoneyMax;
        }

        public City(City city)
         : this(city.name, city.size, city.area, city.itemList, city.workingMoneyMin, city.workingMoneyMax)
        {
        }

        public void Link(City city)
        {
            neighborList.Add(city);
            Debug.Log(this.name + " Added neighbor : " + city.Name);
        }

        public void PrintNeighbors()
        {
            Debug.Log("Print " + this.name + "'s neighbors under below :");
            for (int i = 0; i < neighborList.Count; ++i)
            {
                neighborList[i].Print();
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
                return neighborList;
            }

            set
            {
                neighborList = value;
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