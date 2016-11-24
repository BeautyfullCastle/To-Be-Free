using System;
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
		public float specialEventProbability = 0f;
		public int caughtPolicePieceIndex;
		public int curCityIndex;
		public bool isDetention;
		public bool isActionSkip;
	}

	public class Character
	{
		private readonly int index;
		private Stat stat;
		private string name;
		private string script;
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
		readonly private int totalAP = 3;

		private bool[] canAction = new bool[10];

		private float specialEventProbability = 0f;

		private Police caughtPolice;

		public delegate void MoveCityHandler(string cityName);
		public static event MoveCityHandler MoveCity = delegate { };
		
		// Todo : skill
		public Character(int index, string name, string script, Stat stat, string startCityName, Inventory inven, int eventIndex, string skillScript, int abnormalIndex)
		{
			this.index = index;
			this.name = name;
			this.script = script;
			this.stat = stat;
			this.iconCharacter = GameObject.FindObjectOfType<IconCharacter>();
			if(GameObject.Find(startCityName))
			{
				this.CurCity = GameObject.Find(startCityName).GetComponent<IconCity>().City;
			}
				
			this.inven = inven;
			this.eventIndex = eventIndex;
			this.skillScript = skillScript;
			this.abnormalIndex = abnormalIndex;

			CantCure = false;
			CantMove = false;
			IsFull = false;
			IsDetention = false;
			isActionSkip = false;

			SetCanAction(true);
		}

		public void Save(CharacterSaveData data)
		{
			data.index = this.index;
			data.stat = new StatSaveData(this.stat);
			data.inventory = new List<ItemSaveData>(inven.list.Count);
			for(int i=0; i < inven.list.Count; ++i)
			{
				data.inventory[i] = new ItemSaveData();
				data.inventory[i].index = inven.list[i].Index;
				data.inventory[i].buffAliveDays = inven.list[i].Buff.AliveDays;
			}
			data.specialEventProbability = this.specialEventProbability;
			data.caughtPolicePieceIndex = PieceManager.Instance.List.IndexOf(this.caughtPolice);
			data.curCityIndex = this.curCity.Index;
			data.isDetention = this.isDetention;
			data.isActionSkip = this.isActionSkip;

			SaveLoadManager.Instance.data.character = data;
		}

		public void Load(CharacterSaveData data)
		{
			this.stat = new Stat(data.stat);
			this.inven = new Inventory(data.inventory);
			
			this.specialEventProbability = data.specialEventProbability;
			if(data.caughtPolicePieceIndex == -1)
			{
				this.caughtPolice = null;
			}
			else
			{
				this.caughtPolice = PieceManager.Instance.List[data.caughtPolicePieceIndex] as Police;
			}
			this.curCity = CityManager.Instance.EveryCity[data.curCityIndex];
			this.isDetention = data.isDetention;
			this.isActionSkip = data.isActionSkip;
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

			return diceNum + this.Stat.TempDiceNum + this.Stat.DiceNumByEffect;
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

		public IEnumerator MoveTo(City city, bool direct = false)
		{
			if (direct)
			{
				MoveCity(city.Name);
				yield break;
			}

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

			if(city.Type == eNodeType.BIGCITY)
			{
				TipManager.Instance.Show(eTipTiming.BigCity);
			}
			else if(city.Type == eNodeType.TOWN)
			{
				TipManager.Instance.Show(eTipTiming.Street);
			}
			else if(city.Type == eNodeType.MOUNTAIN)
			{
				TipManager.Instance.Show(eTipTiming.Mountain);
			}

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

		public IEnumerator Init()
		{
			GameObject.Find("Character Name").GetComponent<UILabel>().text = this.Name;
			this.Stat.RefreshUI();
			this.Stat.SetViewRange();

			GameObject.FindObjectOfType<UIInventory>().Change(this.Inven);
			
			UICenterOnChild scrollviewCenter = GameObject.FindObjectOfType<UICenterOnChild>();
			scrollviewCenter.CenterOn(this.CurCity.IconCity.transform);
			scrollviewCenter.enabled = false;
			
			// activate character's passive abnormal condition.
			yield return AbnormalConditionManager.Instance.List[this.AbnormalIndex].Activate(this);

			yield return this.MoveTo(this.CurCity, true);
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
	}
}