using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eStat
	{
		HP, TOTALHP, SATIETY, MONEY, INFO, STRENGTH, AGILITY, FOCUS, TALENT, NULL,
		TOTALSATIETY
	}

	public enum eTestStat
	{
		STRENGTH, AGILITY, FOCUS, TALENT, ALL, NULL
	}

	[Serializable]
	public class StatSaveData
	{
		public int hp;
		public int totalHP;

		public int satiety;
		public int totalSatiety;
		
		public int strength; // 일
		public int agility; // 공안
		public int concentration; // 휴식
		public int talent; // 조사
		
		public int money;
		
		public int infoNum;
		public int viewRange;
		
		public int tempDiceNum;
		public int diceNumByEffect;

		public StatSaveData(Stat stat)
		{
			hp = stat.HP;
			totalHP = stat.TotalHP;

			satiety = stat.Satiety;
			totalSatiety = stat.TotalSatiety;

			strength = stat.Strength;
			agility = stat.Agility;
			concentration = stat.Concentration;
			talent = stat.Talent;

			money = stat.Money;

			infoNum = stat.InfoNum;
			viewRange = stat.ViewRange;

			tempDiceNum = stat.TempDiceNum;
			diceNumByEffect = stat.DiceNumByEffect;
		}
	}

	public class Stat
	{
		private int hp;
		private int totalHP;

		private int satiety;
		private int totalSatiety;

		private int strength; // 일
		private int agility; // 공안
		private int concentration; // 휴식
		private int talent; // 조사

		private int money;

		private int infoNum;
		private int viewRange;
		private int preViewRange;

		private int tempDiceNum;
		private int diceNumByEffect;
		private StatSaveData stat;

		public delegate void OnValueChangeHandler(int value, eStat stat);
		static public event OnValueChangeHandler OnValueChange;

		public Stat()
		{
			viewRange = 1;
			tempDiceNum = 0;
			TotalSatiety = 5;
			Satiety = TotalSatiety;
		}

		public Stat(int hp, int strength, int agility, int concentration, int talent, int startMoney) : this()
		{
			this.hp         = hp;
			this.totalHP    = hp;

			this.strength   = strength;
			this.agility    = agility;
			this.concentration = concentration;
			this.talent    = talent;

			this.money = startMoney;
		}

		public Stat(StatSaveData data)
		{
			hp = data.hp;
			totalHP = data.totalHP;

			satiety = data.satiety;
			totalSatiety = data.totalSatiety;

			strength = data.strength;
			agility = data.agility;
			concentration = data.concentration;
			talent = data.talent;

			money = data.money;

			infoNum = data.infoNum;
			viewRange = data.viewRange;

			tempDiceNum = data.tempDiceNum;
			diceNumByEffect = data.diceNumByEffect;
		}

		public void RefreshUI()
		{
			OnValueChange(hp, eStat.HP);
			OnValueChange(totalHP, eStat.TOTALHP);

			OnValueChange(satiety, eStat.SATIETY);
			OnValueChange(satiety, eStat.TOTALSATIETY);

			OnValueChange(strength, eStat.STRENGTH);
			OnValueChange(agility, eStat.AGILITY);
			OnValueChange(concentration, eStat.FOCUS);
			OnValueChange(talent, eStat.TALENT);

			OnValueChange(money, eStat.MONEY);
			OnValueChange(infoNum, eStat.INFO);
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
				OnValueChange(concentration, eStat.FOCUS);
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
		
		public int Satiety
		{
			get
			{
				return satiety;
			}
			set
			{
				if (value < 0) 
				{
					satiety = 0;
				}
				else if (value > TotalSatiety)
				{
					satiety = TotalSatiety;
				}
				else
				{
					satiety = value;
				}
				
				OnValueChange(satiety, eStat.SATIETY);
			}
		}

		public int TotalSatiety
		{
			get
			{
				return totalSatiety;
			}
			private set
			{
				if (value < 0)
				{
					return;
				}
				totalSatiety = value;
				OnValueChange(satiety, eStat.TOTALSATIETY);
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

		public int DiceNumByEffect
		{
			get
			{
				return diceNumByEffect;
			}
			set
			{
				diceNumByEffect = value;
			}
		}

		public int PreViewRange
		{
			get
			{
				return preViewRange;
			}

			set
			{
				preViewRange = value;
			}
		}

		public void SetViewRange()
		{
			Character character = GameManager.Instance.Character;
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(character.CurCity, ViewRange, eWay.ENTIREWAY, false);

			// Set polices appreared
			foreach (Piece piece in PieceManager.Instance.FindAll(eSubjectType.POLICE))
			{
				Police police = piece as Police;
				bool isExist = cityList.Exists(x => x == police.City);
				police.IconPiece.gameObject.SetActive(isExist);
				if(isExist)
				{
					TipManager.Instance.Show(eTipTiming.PoliceAppeared);
				}
			}

			// Set icon cities brighthen in view range.
			foreach(IconCity iconCity in GameManager.Instance.iconCities)
			{
				iconCity.GetComponent<UIButton>().defaultColor = Color.gray;
			}

			foreach(City city in cityList)
			{
				city.IconCity.GetComponent<UIButton>().defaultColor = Color.white;
			}
		}

		public void Set(eObjectType objectType, int amount)
		{
			switch (objectType)
			{
				case eObjectType.STRENGTH:
					Strength += amount;
					break;
				case eObjectType.AGILITY:
					Agility += amount;
					break;
				case eObjectType.CONCENTRATION:
					Concentration += amount;
					break;
				case eObjectType.TALENT:
					Talent += amount;
					break;
				case eObjectType.ALL:
					Strength += amount;
					Agility += amount;
					Concentration += amount;
					Talent += amount;
					break;
			}
		}

		public void Restore(eObjectType objectType, int amount)
		{
			switch(objectType)
			{
				case eObjectType.STRENGTH:
					Strength -= amount;
					break;
				case eObjectType.AGILITY:
					Agility -= amount;
					break;
				case eObjectType.CONCENTRATION:
					Concentration -= amount;
					break;
				case eObjectType.TALENT:
					Talent -= amount;
					break;
				case eObjectType.ALL:
					Strength -= amount;
					Agility -= amount;
					Concentration -= amount;
					Talent -= amount;
					break;
			}
		}
	}
}