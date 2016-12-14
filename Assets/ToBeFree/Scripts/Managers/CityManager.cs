using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
	public enum eWay
	{
		NORMALWAY = 0, MOUNTAINWAY = 1, HIGHWAY = 2, ENTIREWAY = 3
	}

	public class CityManager : Singleton<CityManager>
	{
		private List<City>[] list;
		private List<City> everyCity;

		private Language.CityData[] engList;
		private Language.CityData[] korList;

		private List<City> neareastPath;

		private BezierCurveList[] curves = new BezierCurveList[(int)eWay.ENTIREWAY + 1];

		private List<Item> cityItems = new List<Item>();

		private float currMoveTime;
		private float totalMoveTime;

		public void Init()
		{
			engList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/English/City.json").dataList;
			korList = new DataList<Language.CityData>(Application.streamingAssetsPath + "/Language/Korean/City.json").dataList;

			curves[(int)eWay.MOUNTAINWAY] = GameObject.Find(eWay.MOUNTAINWAY.ToString()).GetComponent<BezierCurveList>();
			curves[(int)eWay.NORMALWAY] = GameObject.Find(eWay.NORMALWAY.ToString()).GetComponent<BezierCurveList>();
			curves[(int)eWay.HIGHWAY] = GameObject.Find(eWay.HIGHWAY.ToString()).GetComponent<BezierCurveList>();
			curves[(int)eWay.ENTIREWAY] = GameObject.Find(eWay.ENTIREWAY.ToString()).GetComponent<BezierCurveList>();

			foreach (BezierCurveList curve in curves)
			{
				curve.Init();
			}

			currMoveTime = 0f;
			totalMoveTime = 6f;
		}

		public void Save(List<CitySaveData> cityList)
		{
			for(int i=0; i<everyCity.Count; ++i)
			{
				if (everyCity[i].Type == eNodeType.BIGCITY || everyCity[i].Type == eNodeType.MIDDLECITY)
				{
					CitySaveData data = new CitySaveData(i, everyCity[i].Item.Index);
					if (i >= cityList.Count)
						cityList.Add(data);
					else
						cityList[i] = data;
				}
			}
		}

		public void Load(List<CitySaveData> cityList)
		{
			for (int i = 0; i < cityList.Count; ++i)
			{
				everyCity[i].Load(cityList[i]);
			}
		}

		public City GetRand()
		{
			return everyCity[UnityEngine.Random.Range(0, list.Length)];
		}

		public Item SetCityItem()
		{
			List<Item> list = new List<Item>(ItemManager.Instance.List);
			list.RemoveAll(x => x.Tag == ItemTag.FOOD);
			list.RemoveAll(x => cityItems.Contains(x));

			Item item = null;
			if(list.Count > 0)
			{
				item = list[UnityEngine.Random.Range(0, list.Count - 1)];
			}			
			else
			{
				item = cityItems[UnityEngine.Random.Range(0, cityItems.Count - 1)];
			}
			cityItems.Add(item);

			return item;
		}

		public void InitList()
		{
			list = new List<City>[(int)eWay.ENTIREWAY + 1];

			// 중복 제거한 City List 생성
			for(int i=0; i<curves.Length; ++i)
			{
				HashSet<City> hashSet = new HashSet<City>();
				foreach (BezierCurve curve in curves[i].List)
				{
					foreach (BezierPoint point in curve.Points)
					{
						hashSet.Add(point.GetComponent<IconCity>().City);
					}
				}
				list[i] = new List<City>(hashSet);
			}
			
			// initialize everyCity
			everyCity = new List<City>();
			int curIndex = 0;
			foreach(List<City> cities in list)
			{
				foreach(City city in cities)
				{
					if (city == null)
						continue;

					if(everyCity.Contains(city))
					{
						continue;
					}
					city.Distance = 0;
					city.Index = curIndex++;
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
			HashSet<City> hashSet = new HashSet<City>(list[(int)eWay.NORMALWAY]);
			
			if (pieceType != eSubjectType.POLICE)
			{
				hashSet.ExceptWith(list[(int)eWay.NORMALWAY].FindAll(x => PieceManager.Instance.GetNumberOfPiece(pieceType, x) > 0));
				if (pieceType == eSubjectType.BROKER)
				{
					hashSet.ExceptWith(list[(int)eWay.NORMALWAY].FindAll(x => x.Type == eNodeType.SMALLCITY || x.Type == eNodeType.TOWN || x.Type == eNodeType.MOUNTAIN));
				}
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
			foreach (City city in list[(int)eWay.NORMALWAY])
			{
				if (city.Type == size)
				{
					cityList.Add(city);
				}
			}
			return cityList;
		}

		public List<City> GetCityList(eWay way)
		{
			return list[(int)way];
		}

		public City FindRandCityByDistance(City curCity, int distance, eSubjectType pieceType, eWay way)
		{
			System.Random r = new System.Random();

			ResetCitiesDistance();
			// put a police in random cities by distance.
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(curCity, distance, way);

			cityList.RemoveAll(x => x.Type == eNodeType.NULL);

			if (pieceType != eSubjectType.POLICE)
			{
				cityList.RemoveAll(x => PieceManager.Instance.GetNumberOfPiece(pieceType, x) > 0);
				if(pieceType == eSubjectType.BROKER)
				{
					cityList.RemoveAll(x => x.Type == eNodeType.SMALLCITY || x.Type == eNodeType.TOWN || x.Type == eNodeType.MOUNTAIN);
				}
			}

			if(cityList.Count <= 0)
			{
				return FindRand(pieceType);
			}

			int randCityIndex = r.Next(0, cityList.Count-1);

			return cityList[randCityIndex];
		}

		public List<City> FindCitiesByDistance(City curCity, int distance, eWay kindOfWay, bool removeCurCity = true)
		{
			ResetCitiesDistance();

			List<City> cities = new List<City>();
			cities.Add(curCity);

			PutCityInNeighbors(curCity, cities, kindOfWay, distance);

			if(removeCurCity)
			{
				cities.Remove(curCity);
			}

			return cities;
		}

		private void PutCityInNeighbors(City city, List<City> cities, eWay kindOfWay, int distance)
		{
			if(cities.Count-1 >= everyCity.Count)
			{
				return;
			}

			foreach (City neighbor in city.Neighbors[(int)kindOfWay])
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
					PutCityInNeighbors(neighbor, cities, kindOfWay, distance);
				}
			}
		}
		
		public void FindNearestPath(List<City> path1, List<City> path2)
		{
			if (path1 == null || path2 == null)
				neareastPath = null;
			else if (path1.Count < path2.Count)
				neareastPath = path1;
			else
				neareastPath = path2;
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
				path = curves[(int)eWay.ENTIREWAY].GetPath(currentPoint, destinationPoint);
			}
			else
			{
				//if(actionType == eEventAction.MOVE)
					path = curves[(int)eWay.NORMALWAY].GetPath(currentPoint, destinationPoint);
				//else if(actionType == eEventAction.MOVE_BUS)
				//	path = curves[(int)eWay.HIGHWAY].GetPath(currentPoint, destinationPoint);
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

		public IEnumerator MoveTo(Transform character, City curCity, City city, int moveTimePerCity = 0)
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

			yield return MoveTo(character, curIconCity.GetComponent<BezierPoint>(), iconcity.GetComponent<BezierPoint>(), moveTimePerCity);
		}

		public IEnumerator MoveTo(Transform character, BezierPoint curPoint, BezierPoint point, int moveTimePerCity = 0)
		{
			if (curPoint == null || point == null)
			{
				yield break;
			}

			currMoveTime = 0f;
			float currTimeCounter = 1f;
			while (currMoveTime <= moveTimePerCity)
			{
				if (character.gameObject.name == "Character")
				{
					if (currTimeCounter >= 1f)
					{
						TimeTable.Instance.Hour++;
						currTimeCounter = 0f;
					}
				}
				else
				{
					yield return new WaitForEndOfFrame();
				}

				currMoveTime += Time.deltaTime;
				currTimeCounter += Time.deltaTime;

				character.position = BezierCurve.GetPoint(curPoint, point, currMoveTime / moveTimePerCity);
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

		public List<City>[] List
		{
			get
			{
				return list;
			}
		}
	}
}