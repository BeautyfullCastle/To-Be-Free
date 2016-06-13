using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    enum eCity
    {
        KUNMING, CHENDU, CHONGQING, XINNIG, LANZHOU, XIAN, ZHENGZHOU, WUHAN, CHANGSHA, NANCHANG, NANNING, GUANGZHOU, XIANGGANG, FUZHOU,
        NANJING, SHANGHAI, JINAN, QINGDAO, YANTAI, SHIJIAZHUANG, TIANJIN, BEIJING, DALIAN, SHENYANG, DANDONG ,CHANGCHUN, JILIN, YANBIAN, HAERBIN
    }

    enum eArea
    {
        YUNNAN, SICHUAN, QINGHAI, SHANXI, HUNAN, GUANGXI, GUANGDONG, FUJIAN, JIANGSU, SHANDONG, HEBEI, LIAONING, JILIN
    }

    public class City
    {
        private string name;
        private string size;
        private string area;
        private List<City> neighborList;
        private List<Item> itemList;
        private int workingMoneyMin;
        private int workingMoneyMax;
        private int distanceFromCharacter;

        public City()
        {
            neighborList = new List<City>();
            itemList = null;
        }

        public City(string name, string size, string area, List<Item> itemList, int workingMoneyMin, int workingMoneyMax)
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

        public string Name
        {
            get { return name; }
        }

        public string Area { get; internal set; }

        public string Size
        {
            get
            {
                return size;
            }

            set
            {
                size = value;
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