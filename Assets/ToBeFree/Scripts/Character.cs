using System;
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

		private int specialEventProbability = 0;

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
			AP++;
		}

		public IEnumerator MoveTo(City city)
		{
			if (CantMove)
			{
				yield break;
			}

			yield return CityManager.Instance.MoveTo(iconCharacter.transform, curCity, city);

			this.CurCity = city;

			Stat.SetViewRange();
		}
		
		public void PrintMovableCity()
		{
			curCity.PrintNeighbors();
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

		public void AddSpecialEventProbability()
		{
			specialEventProbability += 20;
			if (specialEventProbability > 100)
			{
				specialEventProbability = 100;
			}
		}

		public bool CheckSpecialEvent()
		{
			System.Random r = new System.Random();

			bool hasSpecialEvent = specialEventProbability > r.Next(0, 100);
			if (hasSpecialEvent)
			{
				specialEventProbability = 0;
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
	}
}