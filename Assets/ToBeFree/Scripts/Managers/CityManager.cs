﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class CityManager : Singleton<CityManager>
    {
        private readonly City[] list;
        private readonly CityData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/City.json";

        private List<City> neareastPath;

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
        
        public void FindNearestPathToStartCity(City curCity, City startCity)
        {
            // TO DO : have to reset every city's distance
            foreach (City city in list)
            {
                city.Distance = 0;
            }
            List<City> path = new List<City>();
            path.Add(curCity);
            CalcDist(curCity, path, 0);
            Debug.Log(path.Count);
            for (int i = 0; i < path.Count; ++i)
            {
                Debug.Log(path[i].Name.ToString() + " " + path[i].Distance);
            }
            neareastPath = new List<City>(startCity.Distance);

            List<City> visited = new List<City>();
            int farDistance = 0;
            CalcNeareastPath(curCity, startCity, neareastPath, farDistance, visited);
            
            Queue<City> queue = new Queue<City>(neareastPath);
            while (queue.Count > 0)
            {
                City city = queue.Dequeue();
                Debug.LogWarning(city.Name.ToString() + " " + city.Distance);
            }
        }

        
        
        private void CalcNeareastPath(City curCity, City destination, List<City> neareastPath, int farDistance, List<City> visited)
        {
            foreach(City neighbor in curCity.NeighborList)
            {
                if(visited.Contains(neighbor))
                {
                    continue;
                }
                if(neighbor.Distance <= farDistance)
                {
                    continue;
                }

                if (neareastPath.Count <= neighbor.Distance)
                    neareastPath.Add(neighbor);
                else
                    neareastPath[neighbor.Distance] = neighbor;

                farDistance = neighbor.Distance;
                visited.Add(neighbor);
                if(destination == neighbor)
                {
                    break;
                }
                CalcNeareastPath(neighbor, destination, neareastPath, farDistance, visited);
            }
            if (neareastPath.Contains(destination))
                return;
            neareastPath.RemoveAt(neareastPath.Count - 1);
            City lastCity = neareastPath[neareastPath.Count - 1];
            farDistance = lastCity.Distance;
            CalcNeareastPath(lastCity, destination, neareastPath, farDistance, visited);
        }

        private void CalcDist(City city, List<City> path, int i)
        {
            foreach (City neighbor in city.NeighborList)
            {
                if(path.Exists(x => x == neighbor))
                {
                    continue;
                }
                neighbor.Distance = city.Distance + 1;
                path.Add(neighbor);
            }
            ++i;
            if(path.Count <= i)
            {
                return;
            }
            CalcDist(path[i], path, i);
        }

        public City GetNearestCity(City curCity)
        {
            if(neareastPath.Count <=0)
            {
                return null;
            }
            City neareastCity = new City(neareastPath[0]);
            neareastPath.RemoveAt(0);
            return neareastCity;
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