﻿using System.Collections.Generic;
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

			curves[(int)eWay.MOUNTAINWAY] = GameObject.Find(EnumConvert<eWay>.ToString(eWay.MOUNTAINWAY)).GetComponent<BezierCurveList>();
			curves[(int)eWay.NORMALWAY] = GameObject.Find(EnumConvert<eWay>.ToString(eWay.NORMALWAY)).GetComponent<BezierCurveList>();
			curves[(int)eWay.HIGHWAY] = GameObject.Find(EnumConvert<eWay>.ToString(eWay.HIGHWAY)).GetComponent<BezierCurveList>();
			curves[(int)eWay.ENTIREWAY] = GameObject.Find(EnumConvert<eWay>.ToString(eWay.ENTIREWAY)).GetComponent<BezierCurveList>();

			foreach (BezierCurveList curve in curves)
			{
				curve.Init();
			}

			currMoveTime = 0f;
			totalMoveTime = 6f;
		}

		public string GetName(City city)
		{
			Language.CityData[] dataList = this.engList;
			if(LanguageManager.Instance.CurrentLanguage == eLanguage.KOREAN)
			{
				dataList = this.korList;
			}
			if (dataList == null)
				return string.Empty;

			Language.CityData data = Array.Find(dataList, x => x.index == city.Name);
			if (data == null)
				return string.Empty;

			return data.name;
		}

		public City GetbyIndex(int cityIndex)
		{
			if (cityIndex == -1)
				return null;

			if (cityIndex < 0 || cityIndex >= everyCity.Count)
			{
				Debug.LogError(this.GetType().ToString() + " : GetbyIndex(..) : " + cityIndex + " is out of range.");
				return null;
			}

			return everyCity[cityIndex];
		}

		public void Save(List<CitySaveData> cityList)
		{
			for(int i=0; i<everyCity.Count; ++i)
			{
				if (everyCity[i].Type == eNodeType.BIGCITY)// || everyCity[i].Type == eNodeType.MIDDLECITY)
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
			List<Item> list = ItemManager.Instance.CopyTo();
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
		
		public void FindNearestPathToCamp()
		{
			City city = GameManager.Instance.Character.CurCity;
			List<City> pathToTumen = CityManager.Instance.CalcPath(city, CityManager.Instance.Find("TUMEN"), eEventAction.MOVE);
			List<City> pathToDandong = CityManager.Instance.CalcPath(city, CityManager.Instance.Find("DANDONG"), eEventAction.MOVE);

			if (pathToTumen == null || pathToDandong == null)
				neareastPath = null;
			else if (pathToTumen.Count < pathToDandong.Count)
				neareastPath = pathToTumen;
			else
				neareastPath = pathToDandong;
		}

		public City GetNearestCity(City curCity)
		{
			if (neareastPath == null)
			{
				FindNearestPathToCamp();
			}
			else if(neareastPath.Count <=0)
			{
				FindNearestPathToCamp();
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
				path = curves[(int)eWay.NORMALWAY].GetPath(currentPoint, destinationPoint);
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

		public IEnumerator MoveTo(Transform character, City curCity, City city, float moveTimePerCity = 0, bool flowTime = true)
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

			yield return MoveTo(character, curIconCity.GetComponent<BezierPoint>(), iconcity.GetComponent<BezierPoint>(), moveTimePerCity, flowTime);
		}

		public IEnumerator MoveTo(Transform character, BezierPoint curPoint, BezierPoint point, float moveTimePerCity = 0, bool flowTime = true)
		{
			if (curPoint == null || point == null)
			{
				yield break;
			}

			//List<BezierPoint> path = null;
			//if (curPoint.GetComponent<IconCity>().type == eNodeType.MOUNTAIN || point.GetComponent<IconCity>().type == eNodeType.MOUNTAIN)
			//{
			//	path = curves[(int)eWay.ENTIREWAY].GetPath(curPoint, point);
			//}
			//else
			//{
			//	path = curves[(int)eWay.NORMALWAY].GetPath(curPoint, point);
			//}

			//if (path == null)
			//	yield break;

			currMoveTime = 0f;
			float currTimeCounter = moveTimePerCity;
			while (currMoveTime <= moveTimePerCity)
			{
				if (character.gameObject.name == "Character" && flowTime)
				{
					TimeTable.Instance.Hour += Time.deltaTime / TimeTable.Instance.TimePerHour;
					//if (currTimeCounter >=  TimeTable.Instance.TimePerHour)
					//{
					//	TimeTable.Instance.Hour++;
					//	currTimeCounter = 0f;
					//}
				}
				yield return new WaitForEndOfFrame();

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

		public List<City>[] List
		{
			get
			{
				return list;
			}
		}
	}
}