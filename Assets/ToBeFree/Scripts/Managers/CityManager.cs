using System;
using System.Collections.Generic;
using Language;
using UnityEngine;
using System.Collections;

namespace ToBeFree
{
	public enum eWay
	{
		NORMAL, MOUNTAIN, ENTIRE, BUS
	}

	public class CityManager : Singleton<CityManager>
	{
		private List<City>[] list;
		private List<City> everyCity;

		private Language.CityData[] engList;
		private Language.CityData[] korList;

		private List<City> neareastPath;

		private BezierCurveList[] curves = new BezierCurveList[3];

		private List<Item> cityItems = new List<Item>();

		private float currMoveTime;
		private float totalMoveTime;

		public void Init()
		{
			engList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/English/City.json").dataList;
			korList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/Korean/City.json").dataList;
			
			curves[(int)eWay.NORMAL] = GameObject.Find(eWay.NORMAL.ToString()+"WAY").GetComponent<BezierCurveList>();
			curves[(int)eWay.MOUNTAIN] = GameObject.Find(eWay.MOUNTAIN.ToString() + "WAY").GetComponent<BezierCurveList>();
			curves[(int)eWay.ENTIRE] = GameObject.Find(eWay.ENTIRE.ToString() + "WAY").GetComponent<BezierCurveList>();

			foreach (BezierCurveList curve in curves)
			{
				curve.Init();
			}

			currMoveTime = 0f;
			totalMoveTime = 1f;
		}

		public City GetRand()
		{
			return everyCity[UnityEngine.Random.Range(0, list.Length)];
		}

		public Item SetCityItem()
		{
			Item item = ItemManager.Instance.GetRand();
			while (true)
			{
				if (cityItems.Contains(item))
				{
					item = ItemManager.Instance.GetRand();
				}
				else
				{
					cityItems.Add(item);
					return item;
				}
			}
		}

		public void InitList()
		{
			list = new List<City>[3];

			// 중복 제거한 City List 생성
			for(int i=0; i<curves.Length; ++i)
			{
				HashSet<City> hashSet = new HashSet<City>();
				foreach (BezierCurve curve in curves[i].List)
				{
					foreach (BezierPoint point in curve.points)
					{
						hashSet.Add(point.GetComponent<IconCity>().City);
					}
				}
				list[i] = new List<City>(hashSet);
			}
			
			// initialize everyCity
			everyCity = new List<City>();
			foreach(List<City> cities in list)
			{
				foreach(City city in cities)
				{
					if(everyCity.Contains(city))
					{
						continue;
					}
					city.Distance = 0;
					everyCity.Add(city);
				}
			}
		}
		
		private void ResetCitiesDistance()
		{
			foreach (City city in everyCity)
			{
				city.Distance = 0;
			}
		}
		
		public City Find(string cityName)
		{
			return GameObject.Find(cityName).GetComponent<IconCity>().City;
		}

		public City FindRand(eSubjectType pieceType)
		{
			HashSet<City> hashSet = new HashSet<City>(list[(int)eWay.NORMAL]);
			//hashSet.ExceptWith(list.FindAll(x => x.Type == eNodeType.NULL));

			if (pieceType != eSubjectType.POLICE)
			{
				hashSet.ExceptWith(list[(int)eWay.NORMAL].FindAll(x => PieceManager.Instance.GetNumberOfPiece(pieceType, x) > 0));
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
			foreach (City city in list[(int)eWay.NORMAL])
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

			ResetCitiesDistance();
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

		public List<City> FindCitiesByDistance(City curCity, int distance, bool removeCurCity = true)
		{
			ResetCitiesDistance();

			List<City> cities = new List<City>();
			if (!cities.Contains(curCity))
			{
				cities.Add(curCity);
			}
			PutCityInNeighbors(curCity, cities, distance);

			if(removeCurCity)
			{
				cities.Remove(curCity);
			}	

			return cities;
		}

		private void PutCityInNeighbors(City city, List<City> cities, int distance)
		{
			if(cities.Count-1 >= everyCity.Count)
			{
				return;
			}

			foreach (City neighbor in city.Neighbors)
			{
				if (!cities.Contains(neighbor))
				{
					if(neighbor.Type == eNodeType.MOUNTAIN || city.Type == eNodeType.MOUNTAIN)
					{
						neighbor.Distance = city.Distance + 2;
					}
					else
					{
						neighbor.Distance = city.Distance + 1;
					}
					if (neighbor.Distance > distance)
						continue;
					cities.Add(neighbor);
					PutCityInNeighbors(neighbor, cities, distance);
				}
			}
			
		}
		
		public void FindNearestPath(City curCity, City destCity)
		{
			neareastPath = CalcPath(curCity, destCity);
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
			City neareastCity = neareastPath[0];
			neareastPath.RemoveAt(0);
			return neareastCity;
		}

		public List<City> CalcPath(City curCity, City destCity)
		{
			eNodeType currCityType = curCity.Type;
			eNodeType destCityType = destCity.Type;

			BezierPoint currentPoint = curCity.IconCity.GetComponent<BezierPoint>();
			BezierPoint destinationPoint = destCity.IconCity.GetComponent<BezierPoint>();
			
			List<BezierPoint> path = null;
			if (currCityType == eNodeType.MOUNTAIN || destCityType == eNodeType.MOUNTAIN)
			{
				path = curves[(int)eWay.ENTIRE].GetPath(currentPoint, destinationPoint);
			}
			else
			{
				path = curves[(int)eWay.NORMAL].GetPath(currentPoint, destinationPoint);
			}

			List<City> cityList = new List<City>();
			foreach(BezierPoint point in path)
			{
				cityList.Add(point.GetComponent<IconCity>().City);
			}
			return cityList;
		}

		public IEnumerator MoveTo(Transform character, City curCity, City city)
		{
			if (curCity ==null || city == null)
			{
				yield break;
			}
			IconCity curIconCity = curCity.IconCity;
			IconCity iconcity = city.IconCity;
			if (curIconCity == null || iconcity == null)
			{
				yield break;
			}

			yield return MoveTo(character, curIconCity.GetComponent<BezierPoint>(), iconcity.GetComponent<BezierPoint>());
		}

		public IEnumerator MoveTo(Transform character, BezierPoint curPoint, BezierPoint point)
		{
			if (curPoint == null || point == null)
			{
				yield break;
			}

			currMoveTime = 0f;
			while (currMoveTime < totalMoveTime)
			{
				currMoveTime += Time.deltaTime;

				character.position = BezierCurve.GetPoint(curPoint, point, currMoveTime);
				yield return GameManager.Instance.MoveDirectingCam(character.position, character.position, Time.deltaTime);
			}

			yield return null;
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

		public BezierCurveList[] Curves
		{
			get
			{
				return curves;
			}
		}

		public List<City> EveryCity
		{
			get
			{
				return everyCity;
			}
		}
	}
}