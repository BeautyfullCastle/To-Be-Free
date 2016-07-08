using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class CityManager : Singleton<CityManager>
    {
        private readonly City[] list;
        private readonly CityData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/City.json";

        public CityManager()
        {
            DataList<CityData> cDataList = new DataList<CityData>(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new City[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach (CityData data in dataList)
            {
                if((data.workingMoneyRange.Length != 2) || data.workingMoneyRange[0] > data.workingMoneyRange[1])
                {
                    throw new System.Exception("data.workingMoneyRange is wrong.");
                }

                Item[] itemList = new Item[data.itemIndexList.Length];
                for(int i=0; i<data.itemIndexList.Length; ++i)
                {
                    Item item = ItemManager.Instance.List[data.itemIndexList[i]];
                    itemList[i] = item;
                }

                City city = new City(EnumConvert<eCity>.ToEnum(data.name), EnumConvert<eCitySize>.ToEnum(data.size),
                                    EnumConvert<eArea>.ToEnum(data.area), itemList, data.workingMoneyRange[0], data.workingMoneyRange[1], 
                                    data.neighborList);

                if (list[data.index] != null)
                {
                    throw new Exception("City data.index " + data.index + " is duplicated.");
                }
                list[data.index] = city;
            }
        }
        
        public City Find(eCity cityName)
        {
            return Array.Find<City>(list, x => (x.Name == cityName));
        }

        public City Find(string cityName)
        {
            return Find(EnumConvert<eCity>.ToEnum(cityName));
        }

        public City FindRand()
        {
            System.Random r = new System.Random();
            int randIndex = r.Next(0, list.Length-1);
            return list[randIndex];
        }

        public List<City> FindCitiesBySize(eCitySize size)
        {
            List<City> cityListBySize = new List<City>();
            foreach (City city in list)
            {
                if (city.Size == size)
                {
                    cityListBySize.Add(city);
                }
            }
            return cityListBySize;
        }

        public City FindRandCityByDistance(City curCity, int distance)
        {
            System.Random r = new System.Random();
            // put a police in random cities by distance.
            List<City> cityList = CityManager.Instance.FindCitiesByDistance(curCity, distance);
            int randCityIndex = r.Next(0, cityList.Count-1);

            return cityList[randCityIndex];
        }

        private List<City> FindCitiesByDistance(City curCity, int distance)
        {
            List<City> cities = new List<City>();

            if (!cities.Contains(curCity))
            {
                cities.Add(curCity);
            }
            PutCityInNeighbors(curCity, cities, distance);

            return cities;
        }

        private void PutCityInNeighbors(City city, List<City> cities, int distance)
        {
            if (distance <= 0)
                return;

            foreach (City neighbor in city.NeighborList)
            {
                if (!cities.Contains(neighbor))
                {
                    cities.Add(neighbor);
                    PutCityInNeighbors(neighbor, cities, distance - 1);
                }
            }
        }

        public void CalculateDistance(City curCity)
        {
            // TO DO : have to reset every city's distance
            foreach (City city in list)
            {
                city.Distance = 1000;
            }

            curCity.Distance = 0;
            CalcDist(curCity, 0);
        }

        internal City GetNearestCity(City curCity)
        {
            throw new NotImplementedException();
        }

        private void CalcDist(City city, int dist)
        {
            if (city == null || dist >= city.Distance)
            {
                return;
            }

            foreach (City neighbor in city.NeighborList)
            {
                neighbor.Distance = city.Distance + 1;
                CalcDist(neighbor, neighbor.Distance);
            }
        }

        public City[] List
        {
            get
            {
                return list;
            }
        }
    }
}