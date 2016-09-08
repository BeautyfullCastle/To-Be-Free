using System;
using System.Collections.Generic;
using Language;
using UnityEngine;

namespace ToBeFree
{
	public class CityManager : Singleton<CityManager>
	{
		private List<City> list;

		private Language.CityData[] engList;
		private Language.CityData[] korList;

		private List<City> neareastPath;
		private IconCity nextCity;

		private BezierCurveList curves;

		public void Init()
		{
			engList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/English/City.json").dataList;
			korList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/Korean/City.json").dataList;

			curves = GameObject.FindObjectOfType<BezierCurveList>();
			curves.Init();
		}

		public void InitList()
		{ 
			// 중복 제거한 City List 생성
			HashSet<City> hashSet = new HashSet<City>();
			foreach(BezierCurve curve in curves.List)
			{
				foreach(BezierPoint point in curve.points)
				{
					hashSet.Add(point.GetComponent<IconCity>().City);
				}
			}
			list = new List<City>(hashSet);
		}
		
		
		public City Find(string cityName)
		{
			return GameObject.Find(cityName).GetComponent<IconCity>().City;
		}

		public City FindRand(eSubjectType pieceType)
		{
			HashSet<City> hashSet = new HashSet<City>(list);
			//hashSet.ExceptWith(list.FindAll(x => x.Type == eNodeType.NULL));

			if (pieceType != eSubjectType.POLICE)
			{
				hashSet.ExceptWith(list.FindAll(x => PieceManager.Instance.GetNumberOfPiece(pieceType, x) > 0));
			}

			if (hashSet.Count <= 0)
			{
				return null;
			}

			System.Random r = new System.Random();
			List<City> cityList = new List<City>(hashSet);
			int index = r.Next(0, cityList.Count - 1);

			return cityList[index];
		}

		public List<City> FindCitiesByType(eNodeType size)
		{
			List<City> cityList = new List<City>();
			foreach (City city in list)
			{
				if (city.Type == size)
				{
					cityList.Add(city);
				}
			}
			return cityList;
		}

		public City FindRandCityByDistance(City curCity, int distance, eSubjectType pieceType)
		{
			System.Random r = new System.Random();
			// put a police in random cities by distance.
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(curCity, distance);

			cityList.RemoveAll(x => x.Type == eNodeType.NULL);

			if (pieceType != eSubjectType.POLICE)
			{
				cityList.RemoveAll(x => PieceManager.Instance.GetNumberOfPiece(pieceType, x) > 0);
			}

			if(cityList.Count <= 0)
			{
				return FindRand(pieceType);
			}

			int randCityIndex = r.Next(0, cityList.Count-1);

			return cityList[randCityIndex];
		}

		public List<City> FindCitiesByDistance(City curCity, int distance)
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

			foreach (City neighbor in city.Neighbors)
			{
				if (!cities.Contains(neighbor))
				{
					cities.Add(neighbor);
					if(neighbor.Type == eNodeType.MOUNTAIN || city.Type == eNodeType.MOUNTAIN)
					{
						distance -= 2;
					}
					else
					{
						distance--;
					}
					PutCityInNeighbors(neighbor, cities, distance);
				}
			}
		}
		
		public void FindNearestPath(City curCity, City destCity)
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
			neareastPath = new List<City>(destCity.Distance);
			neareastPath.Add(curCity);
			List<City> visited = new List<City>();
			visited.Add(curCity);

			int farDistance = 0;
			CalcNeareastPath(curCity, destCity, neareastPath, farDistance, visited);

			// if find neareast path, erase current city.
			neareastPath.Remove(curCity);

			Queue<City> queue = new Queue<City>(neareastPath);
			while (queue.Count > 0)
			{
				City city = queue.Dequeue();
				Debug.LogWarning(city.Name.ToString() + " " + city.Distance);
			}
		}
		
		private void CalcNeareastPath(City curCity, City destination, List<City> neareastPath, int farDistance, List<City> visited)
		{
			foreach(City neighbor in curCity.Neighbors)
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
			{
				return;
			}
				
			// remove leaf city and restart from last city of leaf.
			neareastPath.RemoveAt(neareastPath.Count - 1);
			City lastCity = neareastPath[neareastPath.Count - 1];
			farDistance = lastCity.Distance;
			CalcNeareastPath(lastCity, destination, neareastPath, farDistance, visited);
		}

		private void CalcDist(City city, List<City> path, int i)
		{
			foreach (City neighbor in city.Neighbors)
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

		public List<BezierPoint> CalcPath()
		{
			BezierPoint currentPoint = Array.Find<IconCity>(GameManager.Instance.iconCities, x=>x.City==GameManager.Instance.Character.CurCity).GetComponent<BezierPoint>();
			List<BezierPoint> path = curves.GetPath(currentPoint, NextCity.GetComponent<BezierPoint>());
			return path;
		}

		public List<City> List
		{
			get
			{
				return list;
			}
		}

		public Language.CityData[] EngList
		{
			get
			{
				return engList;
			}
		}

		public Language.CityData[] KorList
		{
			get
			{
				return korList;
			}
		}

		public IconCity NextCity
		{
			get
			{
				return nextCity;
			}

			set
			{
				nextCity = value;
			}
		}
	}
}