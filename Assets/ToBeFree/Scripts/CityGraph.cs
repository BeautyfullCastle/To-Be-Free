using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
    public class CityGraph : Singleton<CityGraph>
    {
        private List<City> list;

        public CityGraph()
        {
            Debug.Log("CityGraph constructor");

            this.list = new List<City>();
        }

        public City Add(City city)
        {
            list.Add(city);
            return city;
        }

        public City Find(string name)
        {
            return list.Find(x => (x.Name == name));
        }

        public City FindRand()
        {
            System.Random r = new System.Random();
            int randIndex = r.Next(0, list.Count);
            return list[randIndex];
        }

        public void Link(City cityA, City cityB)
        {
            cityA.Link(cityB);
            cityB.Link(cityA);
        }

        public void Init()
        {
            List<City> bigCityList = FindCitiesBySize("Big");
            foreach (City city in bigCityList)
            {
                city.PieceList.Add(new Police() as Piece);
            }
        }

        private List<City> FindCitiesBySize(string size)
        {
            List<City> cityListBySize = new List<City>();
            foreach(City city in list)
            {
                if(city.Size == size)
                {
                    cityListBySize.Add(city);
                }
            }
            return cityListBySize;
        }

        public void PutRandomPiece(Piece piece, City curCity)
        {
            System.Random r = new System.Random();
            // put a police in random cities.
            int randCityIndex = r.Next(0, list.Count);
            list[randCityIndex].PieceList.Add(piece);
        }

        public City PutRandomPieceByDistance(Piece piece, City curCity, int distance)
        {
            System.Random r = new System.Random();
            // put a police in random cities by distance.
            List<City> cityList = CityGraph.Instance.FindCitiesByDistance(curCity, distance);
            int randCityIndex = r.Next(0, cityList.Count);
            
            cityList[randCityIndex].PieceList.Add(piece);
            Debug.Log(piece.GetType() + " is added to city " + cityList[randCityIndex].Name);
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
                    PutCityInNeighbors(neighbor, cities, distance-1);
                }
            }
        }

        public void CalculateDistance(City curCity)
        {
            curCity.Distance = 0;
            CalcDist(curCity, 0);
        }

        private void CalcDist(City city, int dist)
        {
            if(city == null || dist >= city.Distance)
            {
                return;
            }

            foreach (City neighbor in city.NeighborList)
            {
                neighbor.Distance = city.Distance + 1;
                CalcDist(neighbor, neighbor.Distance);
            }
        }
    }
}