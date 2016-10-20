using System;
using System.Collections.Generic;
using Language;
using UnityEngine;
using System.Collections;

namespace ToBeFree
{
	public enum eWay
	{
		NORMAL, MOUNTAIN, HIGH, ENTIRE
	}

	public class CityManager : Singleton<CityManager>
	{
		private List<City>[] list;
		private List<City> everyCity;

		private Language.CityData[] engList;
		private Language.CityData[] korList;

		private List<City> neareastPath;

		private BezierCurveList[] curves = new BezierCurveList[(int)eWay.ENTIRE + 1];

		private List<Item> cityItems = new List<Item>();

		private float currMoveTime;
		private float totalMoveTime;

		public void Init()
		{
			engList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/English/City.json").dataList;
			korList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/Korean/City.json").dataList;
			
			curves[(int)eWay.NORMAL] = GameObject.Find(eWay.NORMAL.ToString()+"WAY").GetComponent<BezierCurveList>();
			curves[(int)eWay.MOUNTAIN] = GameObject.Find(eWay.MOUNTAIN.ToString() + "WAY").GetComponent<BezierCurveList>();
			curves[(int)eWay.HIGH] = GameObject.Find(eWay.HIGH.ToString() + "WAY").GetComponent<BezierCurveList>();
			curves[(int)eWay.ENTIRE] = GameObject.Find(eWay.ENTIRE.ToString() + "WAY").GetComponent<BezierCurveList>();

			foreach (BezierCurveList curve in curves)
			{
				curve.Init();
			}

			currMoveTime = 0f;
			totalMoveTime = 6f;
		}

		public City GetRand()
		{
			return everyCity[UnityEngine.Random.Range(0, list.Length)];
		}

		public Item SetCityItem()
		{
			Item item = ItemManager.Instance.GetRand();
			//while (true)
			{
				if (cityItems.Contains(item))
				{
					item = ItemManager.Instance.GetRand();
				}
				else
				{
					cityItems.Add(item);
					
				}
				return item;
			}
		}

		public void InitList()
		{
			list = new List<City>[(int)eWay.ENTIRE + 1];

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

		public City FindRandCityByDistance(City curCity, int distance, eSubjectType pieceType, eEventAction actionType = eEventAction.MOVE)
		{
			System.Random r = new System.Random();

			ResetCitiesDistance();
			// put a police in random cities by distance.
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(curCity, distance, actionType);

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

		public List<City> FindCitiesByDistance(City curCity, int distance, eEventAction actionType = eEventAction.MOVE, bool removeCurCity = true)
		{
			ResetCitiesDistance();

			List<City> cities = new List<City>();
			List<City> way = null;
			if (actionType == eEventAction.MOVE)
			{
				List<City> newWay = list[(int)eWay.NORMAL];
				newWay.AddRange(list[(int)eWay.MOUNTAIN]);
				HashSet<City> hashSet = new HashSet<City>(newWay);
				way = new List<City>(hashSet);
			}
			else if(actionType == eEventAction.MOVE_BUS)
			{
				way = list[(int)eWay.HIGH];
			}
			
			if (!cities.Contains(curCity))
			{
				cities.Add(curCity);
			}
			PutCityInNeighbors(curCity, cities, way, distance);

			if(removeCurCity)
			{
				cities.Remove(curCity);
			}

			return cities;
		}

		private void PutCityInNeighbors(City city, List<City> cities, List<City> way, int distance)
		{
			if(cities.Count-1 >= everyCity.Count)
			{
				return;
			}

			foreach (City neighbor in city.Neighbors)
			{
				if (!way.Contains(neighbor))
				{
					continue;
				}
					
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
					PutCityInNeighbors(neighbor, cities, way, distance);
				}
			}
		}
		
		public void FindNearestPath(List<City> path1, List<City> path2)
		{
			if (path1.Count < path2.Count)
				neareastPath = path1;
			else
				neareastPath = path2;
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
			if (neareastPath == null)
				return null;

			if(neareastPath.Count <=0)
			{
				return null;
			}
			City neareastCity = neareastPath[0];
			neareastPath.RemoveAt(0);
			return neareastCity;
		}

		public List<City> CalcPath(City curCity, City destCity, eEventAction actionType)
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
				if(actionType == eEventAction.MOVE)
					path = curves[(int)eWay.NORMAL].GetPath(currentPoint, destinationPoint);
				else if(actionType == eEventAction.MOVE_BUS)
					path = curves[(int)eWay.HIGH].GetPath(currentPoint, destinationPoint);
			}

			if (path == null)
				return null;

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
			float currTimeCounter = totalMoveTime;

			while (currMoveTime < totalMoveTime)
			{
				if (character.gameObject.name == "Character")
				{
					if (currTimeCounter >= (totalMoveTime / 6))
					{
						TimeTable.Instance.Hour++;
						currTimeCounter = 0f;
					}
				}

				currMoveTime += Time.deltaTime;
				currTimeCounter += Time.deltaTime;

				character.position = BezierCurve.GetPoint(curPoint, point, currMoveTime / totalMoveTime);
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