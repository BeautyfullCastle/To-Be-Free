﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	[Serializable]
	public class CharacterSaveData
	{
		public int index;
		public StatSaveData stat;
		public List<ItemSaveData> inventory;
		public int maxSlot;
		public float specialEventProbability = 0f;
		public int caughtPolicePieceIndex;
		public int curCityIndex;
		public bool isDetention;
		public bool isActionSkip;
		public int arrestedDate;
	}

	public class Character
	{
		private readonly int index;
		private Stat stat;
		private string name;
		private string script;
		private string startCityName;
		private int eventIndex;
		private string skillScript;
		private int abnormalIndex;

		private IconCharacter iconCharacter;

		private City curCity;
		private City nextCity;

		private BezierPoint curPoint;

		private Inventory inven;

		private bool isDetention;
		private bool isActionSkip;
		
		private int ap;
		private readonly int totalAP = 3;

		private bool[] canAction = new bool[10];

		private float specialEventProbability = 0f;

		private Police caughtPolice;
		private int arrestedDate;

		// Todo : skill
		public Character(int index, string name, string script, Stat stat, string startCityName, Inventory inven, int eventIndex, string skillScript, int abnormalIndex)
		{
			this.index = index;
			this.name = name;
			this.script = script;
			this.stat = stat;
			this.startCityName = startCityName;
				
			this.inven = inven;
			this.eventIndex = eventIndex;
			this.skillScript = skillScript;
			this.abnormalIndex = abnormalIndex;

			CantCure = false;
			CantMove = false;
			IsFull = false;
			IsDetention = false;
			isActionSkip = false;
			arrestedDate = -1;

			SetCanAction(true);
		}

		public IEnumerator Init()
		{
			this.iconCharacter = GameObject.FindObjectOfType<IconCharacter>();
			this.CurCity = CityManager.Instance.Find(startCityName);

			GameManager.Instance.uiCharacter.Refresh();
			this.Stat.InitUI();
			this.Stat.SetViewRange();

			GameManager.Instance.uiInventory.Init(this.Inven, UIItem.eBelong.INVEN);

			GameManager.Instance.worldCam.GetComponent<CameraZoom>().Init();

			// activate character's passive abnormal condition.
			AbnormalCondition ab = AbnormalConditionManager.Instance.GetByIndex(this.AbnormalIndex);
			if (ab != null)
			{
				yield return ab.Activate(this);
			}
			GameManager.Instance.uiBuffManager.Refresh();

			yield return this.MoveTo(this.CurCity, 0, true);
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
				case eTestStat.FOCUS:
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

			return diceNum + this.Stat.TempDiceNum + this.Stat.DiceNumByEffect;
		}

		public IEnumerator Activate(eVerbType verbType, eObjectType objectType, int amount)
		{
			if (verbType == eVerbType.ADD)
			{
				if (objectType == eObjectType.HP)
				{
					Debug.Log("Cure HP");
					Stat.HP += amount;
				}
				else if (objectType == eObjectType.INFO)
				{
					Stat.InfoNum++;
					if (Stat.InfoNum >= 5)
					{
						// Add broker.
						City brokerCity = CityManager.Instance.FindRand(eSubjectType.BROKER);
						Broker broker = new Broker(brokerCity, eSubjectType.BROKER);
						PieceManager.Instance.Add(broker);
						// Delete first main quest and Load main quest : "Go to the broker"
						Quest firstMainQuest = QuestManager.Instance.GetByIndex(15);
						if(firstMainQuest != null)
						{
							GameManager.Instance.uiQuestManager.DeleteQuest(firstMainQuest);
						}

						Quest mainQuest = QuestManager.Instance.GetByIndex(16);
						if(mainQuest != null)
						{
							TipManager.Instance.Show(eTipTiming.Quest);
							yield return (QuestManager.Instance.Load(mainQuest, broker));
						}
						
						Stat.InfoNum = 0;
					}
				}
				else if (objectType == eObjectType.FOOD)
				{
					Stat.Satiety += amount;
				}
				else if (objectType == eObjectType.INVEN)
				{
					for (int i = 0; i < amount; ++i)
					{
						Inven.AddSlot();
					}
				}
				else if (objectType == eObjectType.VIEWRANGE)
				{
					Stat.PreViewRange = Stat.ViewRange;
					Stat.ViewRange += amount;
				}
				else if (objectType == eObjectType.DICE)
				{
					Stat.DiceNumByEffect += amount;
				}
			}
			else if (verbType == eVerbType.DEL)
			{
				if (objectType == eObjectType.INFO)
				{
					Stat.InfoNum--;
				}
			}
			else if (verbType == eVerbType.MOVE)
			{
				if (objectType == eObjectType.CLOSE)
				{
					yield return MoveTo(CityManager.Instance.FindRandCityByDistance(CurCity, amount, eSubjectType.CHARACTER, eWay.ENTIREWAY), TimeTable.Instance.MoveTimePerAction, false, false);
				}
				// can't move after move event( in mongolia )
				else if (objectType == eObjectType.CANCEL)
				{
					CantMove = true;
				}
			}
			else if (verbType == eVerbType.IN)
			{
				if (objectType == eObjectType.DETENTION)
				{
					IsDetention = true;
				}
			}
			else if(verbType == eVerbType.ARRESTED)
			{
				if(objectType == eObjectType.STAKEOUT)
				{
					Police police = new Police(this.curCity, eSubjectType.POLICE, 1, 1);
					PieceManager.Instance.Add(police);
					AbnormalCondition detention = AbnormalConditionManager.Instance.Find("Detention");
					if (detention != null)
					{
						yield return detention.Activate(this);
					}

					yield return this.Arrested(police);
				}
			}
		}

		public IEnumerator Deactivate(eVerbType verbType, eObjectType objectType)
		{
			if (verbType == eVerbType.MOVE)
			{
				if (objectType == eObjectType.CANCEL)
				{
					CantMove = false;
				}
			}
			else if (verbType == eVerbType.ADD)
			{
				if (objectType == eObjectType.VIEWRANGE)
				{
					Stat.ViewRange = Stat.PreViewRange;
				}
				else if (objectType == eObjectType.DICE)
				{
					Stat.DiceNumByEffect = 0;
				}
			}

			yield return null;
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

		public IEnumerator MoveTo(City city, float moveTimePerCity = 0, bool direct = false, bool flowTime = true)
		{
			if (direct)
			{
				iconCharacter.MoveCity(city.IconCity);
				yield break;
			}

			if (CantMove)
			{
				yield break;
			}

			yield return CityManager.Instance.MoveTo(iconCharacter.transform, curCity, city, moveTimePerCity, flowTime);
			this.iconCharacter.transform.position = city.IconCity.characterOffset.position;
			this.CurCity = city;

			eTipTiming tipTiming = eTipTiming.NULL;
			if(city.Type == eNodeType.BIGCITY)
			{
				tipTiming = eTipTiming.BigCity;
			}
			else if(city.Type == eNodeType.TOWN)
			{
				tipTiming = eTipTiming.Street;
			}
			else if(city.Type == eNodeType.MOUNTAIN)
			{
				tipTiming = eTipTiming.Mountain;
			}

			if (tipTiming != eTipTiming.NULL)
			{
				TipManager.Instance.Show(tipTiming);
			}

			Stat.SetViewRange();
		}
		
		public IEnumerator HaulIn()
		{
			for(int i=0; i<caughtPolice.Movement; ++i)
			{
				City city = CityManager.Instance.GetNearestCity(CurCity);

				if (city == null)
				{
					break;
				}
				yield return (MoveTo(city, 1f, false, false));// TimeTable.Instance.MoveTimePerAction * 3 / caughtPolice.Movement, false));
				yield return (this.caughtPolice.MoveTo(city));
			}

			yield return TimeTable.Instance.SpendTime(TotalAP, eSpendTime.END);
		}

		public void AddSpecialEventProbability()
		{
			specialEventProbability += GameManager.Instance.IncreasingSpecialEventProbability;
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
				TipManager.Instance.Show(eTipTiming.Event);
			}
			return hasSpecialEvent;
		}

		public void ResetAPandAction()
		{
			ap = 0;
			SetCanAction(true);
		}

		public IEnumerator Arrested(Police police, bool atPoliceTurn = false)
		{
			this.caughtPolice = police;

			CityManager.Instance.FindNearestPathToCamp();

			this.IsDetention = true;

			this.arrestedDate = TimeTable.Instance.Day;

			if(atPoliceTurn)
			{
				yield return TimeTable.Instance.SpendTime(TotalAP, eSpendTime.END);
			}
			else
			{
				yield return SkipTime();
			}

			if (GameManager.Instance.State == GameManager.GameState.StartWeek)
			{
				GameManager.Instance.ChangeState(GameManager.GameState.Night);
			}
		}

		public IEnumerator SkipTime()
		{
			int remainAP = this.RemainAP;
			this.AP = this.TotalAP;

			yield return TimeTable.Instance.SpendRemainTime();

			yield return TimeTable.Instance.SpendTime(remainAP, eSpendTime.END);
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
				IconCity icon = curCity.IconCity;
				if (icon != null)
				{
					CurPoint = icon.GetComponent<BezierPoint>();
				}
				
			}
		}

		public Inventory Inven
		{
			get
			{
				return inven;
			}
			set
			{
				inven = value;
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
			set
			{
				name = value;

			}
		}

		public int EventIndex
		{
			get
			{
				return eventIndex;
			}

			set
			{
				eventIndex = value;
			}
		}

		public string SkillScript
		{
			get
			{
				return skillScript;
			}

			set
			{
				skillScript = value;
			}
		}

		public int AbnormalIndex
		{
			get
			{
				return abnormalIndex;
			}

			set
			{
				abnormalIndex = value;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public float SpecialEventProbability
		{
			get
			{
				return specialEventProbability;
			}

			set
			{
				specialEventProbability = value;
			}
		}

		public string EngName
		{
			get
			{
				Language.CharacterData characterData = CharacterManager.Instance.GetLanguageData(eLanguage.ENGLISH, this);
				if(characterData == null)
				{
					return string.Empty;
				}

				return characterData.name;
			}
		}

		public int ArrestedDate
		{
			get
			{
				return arrestedDate;
			}
			set
			{
				arrestedDate = value;
			}
		}

		public string Script
		{
			get
			{
				return script;
			}
		}
	}
}