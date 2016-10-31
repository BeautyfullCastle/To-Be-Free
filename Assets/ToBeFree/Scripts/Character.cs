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

		private IconCharacter iconCharacter;

		private City curCity;
		private City nextCity;

		private BezierPoint curPoint;

		private Inventory inven;

		private bool isDetention;
		private bool isActionSkip;

		private float moveTime;
		private int ap;
		readonly private int totalAP = 3;

		private bool[] canAction = new bool[10];

		private float specialEventProbability = 0f;

		private Police caughtPolice;

		public delegate void MoveCityHandler(string cityName);
		public static event MoveCityHandler MoveCity = delegate { };
		
		// Todo : skill
		public Character(string name, string script, Stat stat, string startCityName, Inventory inven)
		{
			this.name = name;			
			this.script = script;
			this.stat = stat;
			this.iconCharacter = GameObject.FindObjectOfType<IconCharacter>();
			this.CurCity = GameObject.Find(startCityName).GetComponent<IconCity>().City;
			this.inven = inven;

			CantCure = false;
			CantMove = false;
			IsFull = false;
			IsDetention = false;
			isActionSkip = false;

			SetCanAction(true);

			GameObject.Find("Character Name").GetComponent<UILabel>().text = this.name;
		}

		private void SetCanAction(bool canAction)
		{
			for (int i = 0; i < this.canAction.Length; ++i)
			{
				this.canAction[i] = canAction;
			}
		}
		
		public int GetDiceNum(eTestStat stat)
		{
			int diceNum = 0;
			switch (stat)
			{
				case eTestStat.STRENGTH:
					diceNum = this.Stat.Strength;
					break;
				case eTestStat.AGILITY:
					diceNum = this.Stat.Agility;
					break;
				case eTestStat.CONCENTRATION:
					diceNum = this.Stat.Concentration;
					break;
				case eTestStat.TALENT:
					diceNum = this.Stat.Talent;
					break;
				case eTestStat.ALL:
				case eTestStat.NULL:
					return -99;
				default:
					Debug.LogError("GetDiceNum : Stat name is not correct : " + stat);
					return -1;
			}

			return diceNum + this.Stat.TempDiceNum;
		}

		public void Rest()
		{
			if(CantCure)
			{
				return;
			}
			Stat.HP++;
			AP++;
		}

		public IEnumerator MoveTo(City city)
		{
			if (CantMove)
			{
				yield break;
			}

			if(isDetention == false)
			{
				AP++;
				if(curCity.Type == eNodeType.MOUNTAIN || city.Type == eNodeType.MOUNTAIN)
				{
					AP++;
				}
			}
			yield return CityManager.Instance.MoveTo(iconCharacter.transform, curCity, city);

			this.CurCity = city;

			Stat.SetViewRange();
		}
		
		public IEnumerator HaulIn()
		{
			for(int i=0; i<caughtPolice.Movement; ++i)
			{
				City city = CityManager.Instance.GetNearestCity(CurCity);

				if (city != null)
				{
					yield return MoveTo(city);
					yield return this.caughtPolice.MoveCity(city);
				}
			}
			
		}

		public void AddSpecialEventProbability()
		{
			specialEventProbability += 4.7f;
			if (specialEventProbability > 100f)
			{
				specialEventProbability = 100f;
			}
		}

		public bool CheckSpecialEvent()
		{
			bool hasSpecialEvent = specialEventProbability > UnityEngine.Random.Range(0f, 100f);
			if (hasSpecialEvent)
			{
				specialEventProbability = 0f;
			}
			return hasSpecialEvent;
		}

		public void Reset()
		{
			ap = 0;
			SetCanAction(true);
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
				CurPoint = Array.Find<IconCity>(GameManager.Instance.iconCities, x => x.City == curCity).GetComponent<BezierPoint>();
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
		public bool IsDetention
		{
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

		public int AP
		{
			get
			{
				return ap;
			}

			set
			{
				ap = value;
			}
		}

		public int RemainAP
		{
			get
			{
				return totalAP - ap;
			}
		}

		public bool[] CanAction
		{
			get
			{
				return canAction;
			}

			set
			{
				canAction = value;
			}
		}

		public Police CaughtPolice
		{
			get
			{
				return caughtPolice;
			}

			set
			{
				caughtPolice = value;
			}
		}

		public int TotalAP
		{
			get
			{
				return totalAP;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}
	}
}