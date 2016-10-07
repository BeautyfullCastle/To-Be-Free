using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{

	public enum eStat
	{
		HP, TOTALHP, MENTAL, TOTALMENTAL, FOOD, MONEY, INFO, STRENGTH, AGILITY, OBSERVATION, BARGAIN, PATIENCE, LUCK, NULL
	}

	public enum eTestStat
	{
		STRENGTH, AGILITY, OBSERVATION, BARGAIN, PATIENCE, LUCK, ALL, NULL
	}

	public class Stat
	{
		private int hp;
		private int totalHP;
		private int mental;
		private int totalMental;

		private int strength;
		private int agility;
		private int observation;
		private int bargain;
		private int patience;
		private int luck;

		private int prevStrength;
		private int prevAgility;
		private int prevObservation;
		private int prevBargain;
		private int prevPatience;
		private int prevLuck;

		private int money;
		private int foodNum;

		private int infoNum;
		private int viewRange;

		private int tempDiceNum;

		public delegate void OnValueChangeHandler(int value, eStat stat);
		static public event OnValueChangeHandler OnValueChange;

		public Stat()
		{
			viewRange = 1;
			tempDiceNum = 0;
		}

		public Stat(int hP, int mental, int strength, int agility, int observation, int bargain, int patience, int luck, int startMoney) : this()
		{
			this.hp         = hP;
			this.totalHP    = hP;
			this.mental     = mental;
			this.totalMental = mental;

			this.strength   = strength;
			this.agility    = agility;
			this.observation = observation;
			this.bargain    = bargain;
			this.patience   = patience;
			this.luck       = luck;

			this.money = startMoney;
		}

		public void RefreshUI()
		{
			OnValueChange(hp, eStat.HP);
			OnValueChange(totalHP, eStat.TOTALHP);
			OnValueChange(mental, eStat.MENTAL);
			OnValueChange(totalMental, eStat.TOTALMENTAL);

			OnValueChange(strength, eStat.STRENGTH);
			OnValueChange(agility, eStat.AGILITY);
			OnValueChange(observation, eStat.OBSERVATION);
			OnValueChange(bargain, eStat.BARGAIN);
			OnValueChange(patience, eStat.PATIENCE);
			OnValueChange(luck, eStat.LUCK);

			OnValueChange(foodNum, eStat.FOOD);
			OnValueChange(money, eStat.MONEY);
			OnValueChange(infoNum, eStat.INFO);
			
		}

		public void AddFood(Item item)
		{
			if (item.Buff.StartTime == eStartTime.NIGHT)
			{
				this.FOOD++;
			}
		}

		public void DeleteFood(Item item)
		{
			if (item.Buff.StartTime == eStartTime.NIGHT)
			{
				this.FOOD--;
			}
		}

		public int Strength
		{
			get
			{
				return strength;
			}
			set
			{
				strength = Mathf.Max(value, 1);
				OnValueChange(strength, eStat.STRENGTH);
			}
		}

		public int Agility
		{
			get
			{
				return agility;
			}

			set
			{
				agility = Mathf.Max(value, 1);
				OnValueChange(agility, eStat.AGILITY);
			}
		}

		public int Observation
		{
			get
			{
				return observation;
			}

			set
			{
				observation = Mathf.Max(value, 1);
				OnValueChange(observation, eStat.OBSERVATION);
			}
		}

		public int Bargain
		{
			get
			{
				return bargain;
			}

			set
			{
				bargain = Mathf.Max(value, 1);
				OnValueChange(bargain, eStat.BARGAIN);
			}
		}

		public int Patience
		{
			get
			{
				return patience;
			}

			set
			{
				patience = Mathf.Max(value, 1);
				OnValueChange(patience, eStat.PATIENCE);
			}
		}

		public int Luck
		{
			get
			{
				return luck;
			}

			set
			{
				luck = Mathf.Max(value, 1);
				OnValueChange(luck, eStat.LUCK);
			}
		}

		public int HP
		{
			get
			{
				return hp;
			}
			set
			{
				hp = value;
				if (hp > TotalHP)
				{
					hp = TotalHP;
				}
				OnValueChange(hp, eStat.HP);
				Debug.Log("HP : " + hp);
			}
		}

		
		public int TotalHP
		{
			get
			{
				return totalHP;
			}
			set
			{
				totalHP = value;
				OnValueChange(totalHP, eStat.TOTALHP);
			}
		}

		

		public int MENTAL
		{
			get
			{
				return mental;
			}
			set
			{
				mental = value;
				if (mental > TotalMental)
				{
					mental = TotalMental;
				}
				OnValueChange(mental, eStat.MENTAL);
				Debug.Log("Mental : " + mental);
			}
		}

		public int TotalMental
		{
			get
			{
				return totalMental;
			}
			set
			{
				totalMental = value;
				OnValueChange(totalMental, eStat.TOTALMENTAL);
			}
		}
		

		public int FOOD
		{
			get
			{
				return foodNum;
			}
			set
			{
				if (value < 0)
				{
					return;
				}
				foodNum = value;
				OnValueChange(foodNum, eStat.FOOD);
			}
		}

		public int InfoNum
		{
			get
			{
				return infoNum;
			}

			set
			{
				if (value < 0)
				{
					return;
				}
				infoNum = value;
				if(infoNum >= 3)
				{
					City cityOfBroker = CityManager.Instance.FindRandCityByDistance(GameManager.Instance.Character.CurCity, 2, eSubjectType.BROKER);
					Piece broker = new Piece(cityOfBroker, eSubjectType.BROKER);
					GameManager.Instance.StartCoroutine(PieceManager.Instance.Add(broker));
					infoNum = 0;
				}

				OnValueChange(InfoNum, eStat.INFO);

			}
		}

		public int Money
		{
			get
			{
				return money;
			}

			set
			{
				if (money + value < 0)
				{
					throw new System.Exception("not enough money");
				}
				money = value;
				OnValueChange(money, eStat.MONEY);
			}
		}

		public int ViewRange
		{
			get
			{
				return viewRange;
			}
			set
			{
				viewRange = value;
				SetViewRange();
			}
		}

		public int TempDiceNum
		{
			get
			{
				return tempDiceNum;
			}

			set
			{
				tempDiceNum = value;
			}
		}

		public void SetViewRange()
		{
			Character character = GameManager.Instance.Character;
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(character.CurCity, ViewRange);
			foreach (Piece piece in PieceManager.Instance.FindAll(eSubjectType.POLICE))
			{
				Police police = piece as Police;
				police.IconPiece.gameObject.SetActive(cityList.Exists(x => x == police.City));
			}
		}

		public void Set(eObjectType objectType, int amount)
		{

			switch (objectType)
			{
				case eObjectType.STRENGTH:
					prevStrength = Strength;
					Strength += amount;
					break;
				case eObjectType.AGILITY:
					prevAgility = Agility;
					Agility += amount;
					break;
				case eObjectType.OBSERVATION:
					prevObservation = Observation;
					Observation += amount;
					break;
				case eObjectType.BARGAIN:
					prevBargain = Bargain;
					Bargain += amount;
					break;
				case eObjectType.PATIENCE:
					prevPatience = Patience;
					Patience += amount;
					break;
				case eObjectType.LUCK:
					Luck = prevLuck;
					Luck += amount;
					break;
				case eObjectType.ALL:
					prevStrength     = Strength;
					prevAgility      = Agility;
					prevObservation  = Observation;
					prevBargain      = Bargain;
					prevPatience     = Patience;
					prevLuck         = Luck;

					Strength += amount;
					Agility += amount;
					Observation += amount;
					Bargain += amount;
					Patience += amount;
					Luck += amount;
					break;
			}
		}

		public void Restore(eObjectType objectType)
		{
			switch(objectType)
			{
				case eObjectType.STRENGTH:
					Strength = prevStrength;
					break;
				case eObjectType.AGILITY:
					Agility = prevAgility;
					break;
				case eObjectType.OBSERVATION:
					Observation = prevObservation;
					break;
				case eObjectType.BARGAIN:
					Bargain = prevBargain;
					break;
				case eObjectType.PATIENCE:
					Patience = prevPatience;
					break;
				case eObjectType.LUCK:
					Luck = prevLuck;
					break;
				case eObjectType.ALL:
					Strength = prevStrength;
					Agility = prevAgility;
					Observation = prevObservation;
					Bargain = prevBargain;
					Patience = prevPatience;
					Luck = prevLuck;
					break;
			}
		}
	}
}