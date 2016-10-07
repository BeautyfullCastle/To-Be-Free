using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{

	public enum eStat
	{
		HP, TOTALHP, FOOD, MONEY, INFO, STRENGTH, AGILITY, CONCENTRATION, TALENT, NULL
	}

	public enum eTestStat
	{
		STRENGTH, AGILITY, CONCENTRATION, TALENT, ALL, NULL
	}

	public class Stat
	{
		private int hp;
		private int totalHP;

		private int strength; // 일
		private int agility; // 공안
		private int concentration; // 휴식
		private int talent; // 조사

		private int prevStrength;
		private int prevAgility;
		private int prevConcentration;
		private int prevTalent;

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

		public Stat(int hP, int strength, int agility, int concentration, int talent, int startMoney) : this()
		{
			this.hp         = hP;
			this.totalHP    = hP;

			this.strength   = strength;
			this.agility    = agility;
			this.concentration = concentration;
			this.talent    = talent;

			this.money = startMoney;
		}

		public void RefreshUI()
		{
			OnValueChange(hp, eStat.HP);
			OnValueChange(totalHP, eStat.TOTALHP);

			OnValueChange(strength, eStat.STRENGTH);
			OnValueChange(agility, eStat.AGILITY);
			OnValueChange(concentration, eStat.CONCENTRATION);
			OnValueChange(talent, eStat.TALENT);

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

		public int Concentration
		{
			get
			{
				return concentration;
			}

			set
			{
				concentration = Mathf.Max(value, 1);
				OnValueChange(concentration, eStat.CONCENTRATION);
			}
		}

		public int Talent
		{
			get
			{
				return talent;
			}

			set
			{
				talent = Mathf.Max(value, 1);
				OnValueChange(talent, eStat.TALENT);
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
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(character.CurCity, ViewRange, false);
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
				case eObjectType.CONCENTRATION:
					prevConcentration = Concentration;
					Concentration += amount;
					break;
				case eObjectType.TALENT:
					prevTalent = Talent;
					Talent += amount;
					break;
				case eObjectType.ALL:
					prevStrength     = Strength;
					prevAgility      = Agility;
					prevConcentration  = Concentration;
					prevTalent      = Talent;

					Strength += amount;
					Agility += amount;
					Concentration += amount;
					Talent += amount;
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
				case eObjectType.CONCENTRATION:
					Concentration = prevConcentration;
					break;
				case eObjectType.TALENT:
					Talent = prevTalent;
					break;
				case eObjectType.ALL:
					Strength = prevStrength;
					Agility = prevAgility;
					Concentration = prevConcentration;
					Talent = prevTalent;
					break;
			}
		}
	}
}