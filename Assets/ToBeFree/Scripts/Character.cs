﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class Character
	{
		private Stat stat;
		private string name;
		private string script;

		private City curCity;
		private City nextCity;
		readonly private City startCity;

		private BezierPoint curPoint;

		private Inventory inven;

		private bool isDetention;
		private bool isActionSkip;

		private float moveTime;

		public delegate void MoveCityHandler(string cityName);
		public static event MoveCityHandler MoveCity = delegate { };
		
		// Todo : skill

		public Character(string name, string script, Stat stat, City curCity, Inventory inven)
		{
			this.name = name;
			this.script = script;
			this.stat = stat;
			this.curCity = curCity;
			this.startCity = new City(curCity);
			this.inven = inven;

			CantCure = false;
			CantMove = false;
			IsFull = false;
			IsDetention = false;
			isActionSkip = false;
		}
		
		public int GetDiceNum(eTestStat stat)
		{
			switch (stat)
			{
				case eTestStat.STRENGTH:
					return this.Stat.Strength;
				case eTestStat.AGILITY:
					return this.Stat.Agility;
				case eTestStat.OBSERVATION:
					return this.Stat.Observation;
				case eTestStat.BARGAIN:
					return this.Stat.Bargain;
				case eTestStat.PATIENCE:
					return this.Stat.Patience;
				case eTestStat.LUCK:
					return this.Stat.Luck;
				case eTestStat.ALL:
				case eTestStat.NULL:
					return -99;
				default:
					Debug.LogError("GetDiceNum : Stat name is not correct : " + stat);
					return -1;
			}
		}

		public void Rest()
		{
			if(CantCure)
			{
				return;
			}
			Stat.HP++;
			Stat.MENTAL++;
		}
		
		public IEnumerator MoveTo(City city)
		{
			if (city == null)
			{
				yield break;
			}
			if(CantMove)
			{
				yield break;
			}
			
			
			yield return GameManager.Instance.MoveDirectingCam(
				GameManager.Instance.FindGameObject(this.curCity.Name.ToString()).transform.position,
				GameManager.Instance.FindGameObject(city.Name.ToString()).transform.position,
				2f);

			this.CurCity = city;
			MoveCity(EnumConvert<eCity>.ToString(city.Name));

			Debug.Log("character is moved to " + this.curCity.Name);

			yield return null;
		}

		public IEnumerator MoveTo(BezierPoint point)
		{
			if (point == null)
			{
				yield break;
			}
			if (CantMove)
			{
				yield break;
			}


			//yield return GameManager.Instance.MoveDirectingCam(
			//	GameManager.Instance.FindGameObject(this.curCity.Name.ToString()).transform.position,
			//	point.gameObject.transform.position,
			//	2f);

			moveTime = 0f;
			while (moveTime < 1f)
			{
				moveTime += Time.deltaTime;
				GameObject.Find("Character").transform.position = BezierCurve.GetPoint(CurPoint, point, moveTime);
				yield return new WaitForFixedUpdate();
			}
			this.CurCity = point.GetComponent<IconCity>().City;

			

			//MoveCity(EnumConvert<eCity>.ToString(city.Name));

			//Debug.Log("character is moved to " + this.curCity.Name);

			yield return null;
		}

		public void PrintMovableCity()
		{
			curCity.PrintNeighbors();
		}
		
		public Stat Stat
		{
			get
			{
				return stat;
			}

			set
			{
				stat = value;
			}
		}

		public City CurCity
		{
			get
			{
				return curCity;
			}

			set
			{
				curCity = value;
			}
		}

		public Inventory Inven
		{
			get
			{
				return inven;
			}
		}

		public City NextCity
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

		public bool IsFull { get; internal set; }
		public bool IsDetention {
			get
			{
				return isDetention;
			}
			set
			{
				isDetention = value;
			}
		}

		public bool CantCure { get; internal set; }
		public bool CantMove { get; internal set; }

		public City StartCity
		{
			get
			{
				return startCity;
			}
		}

		public bool IsActionSkip
		{
			get
			{
				return isActionSkip;
			}

			set
			{
				isActionSkip = value;
			}
		}

		public BezierPoint CurPoint
		{
			get
			{
				return curPoint;
			}

			set
			{
				curPoint = value;
			}
		}

		public IEnumerator HaulIn()
		{
			City city = CityManager.Instance.GetNearestCity(CurCity);
			
			if (city != null)
			{
				yield return MoveTo(city);
			}
			// if no more left cities 
			else
			{
				GameManager.Instance.uiEventManager.OpenUI();

				yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, this);
				bool testResult = (DiceTester.Instance.Test(Stat.Luck) > 0);       
				yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, this);

				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, "Last chance to Escape!!!");
				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testResult.ToString());

				yield return EventManager.Instance.WaitUntilFinish();
				
				if(testResult == true)
				{
					yield return AbnormalConditionManager.Instance.Find("Detention").DeActivate(this);
				}
				else
				{
					// game over
					yield return GameManager.Instance.ShowStateLabel("Game Over!", 1f);
				}
			}

		}
	}
}