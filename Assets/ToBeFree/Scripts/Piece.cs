using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class Piece
	{
		protected City city;
		protected IconPiece iconPiece;
		protected eSubjectType subjectType;

		public Piece(City city, eSubjectType subjectType)
		{
			this.city = city;
			this.subjectType = subjectType;
			GameObject pieceObj = GameObject.Instantiate(GameManager.Instance.IconPieceObj, GameManager.Instance.IconPieceObj.transform.parent) as GameObject;
			iconPiece = pieceObj.GetComponent<IconPiece>();
			iconPiece.Init(subjectType);
			city.IconCity.PutPiece(iconPiece);
		}

		public City City
		{
			get
			{
				return city;
			}
		}

		public eSubjectType SubjectType
		{
			get
			{
				return subjectType;
			}
		}

		public IconPiece IconPiece
		{
			get
			{
				return iconPiece;
			}
		}

		public IEnumerator MoveCity(City destCity)
		{
			List<City> path = CityManager.Instance.CalcPath(city, destCity, eEventAction.MOVE);

			foreach (City nextCity in path)
			{
				city = nextCity;
				GameManager.Instance.Character.Stat.SetViewRange();
				if (this.iconPiece.gameObject.activeSelf)
				{
					yield return CityManager.Instance.MoveTo(iconPiece.transform, city, nextCity);
				}
			}
			city.IconCity.PutPiece(this.iconPiece);
		}

		public virtual bool CheckDuration()
		{
			return false;
		}
	}

	public class Police : Piece
	{
		private int power;
		private int movement;
		readonly private int max;

		public Police(City city, eSubjectType subjectType) : base(city, subjectType)
		{
			max = 5;

			Power = UnityEngine.Random.Range(1, 4);
			Movement = UnityEngine.Random.Range(1, 4);

			iconPiece.Init(subjectType, Power, Movement);
		}

		public IEnumerator AddStat(bool IsCrackDown)
		{
			if (IsCrackDown)
			{
				Power++;
				Movement++;
			}
			else
			{
				int randIndex = UnityEngine.Random.Range(0, 2);
				if (randIndex == 0)
				{
					Power++;
				}
				else if (randIndex == 1)
				{
					Movement++;
				}
			}
			yield return GameManager.Instance.MoveDirectingCam(new List<Transform> { iconPiece.transform }, 1f);
		}

		public IEnumerator Move()
		{
			// city list can move.
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(city, movement, eWay.NORMALWAY);
			
			// police can't go to the mountain.
			cityList.RemoveAll(x => x.Type == eNodeType.MOUNTAIN);
			
			yield return MoveCity(cityList[UnityEngine.Random.Range(0, cityList.Count)]);
		}

		public bool IsMaxStat()
		{
			return (Power >= max | Movement >= max);
		}

		public int Power
		{
			get
			{
				return power;
			}
			private set
			{
				power = value;
				if (power > max)
				{
					power = max;
				}
				iconPiece.Power = power;
			}
		}

		public int Movement
		{
			get
			{
				return movement;
			}
			private set
			{
				movement = value;
				if (movement > max)
				{
					movement = max;
				}
				iconPiece.Movement = movement;
			}
		}

		public IEnumerator Fight(eEventAction actionName, Character character)
		{
			Event selectedEvent = EventManager.Instance.Find(actionName);
			if (selectedEvent == null)
			{
				Debug.LogError("selectedEvent is null");
				yield break;
			}

			GameManager.Instance.uiEventManager.OpenUI();

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);

			int characterSuccessNum = DiceTester.Instance.Test(character.Stat.Agility);
			int policeSuccessNum = DiceTester.Instance.Test(this.Power);
			EventManager.Instance.TestResult = characterSuccessNum >= policeSuccessNum;
			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, selectedEvent.Script);

			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM,
				EventManager.Instance.TestResult.ToString() + ", " + characterSuccessNum.ToString() + " : " + policeSuccessNum.ToString());

			yield return EventManager.Instance.TreatResult(selectedEvent.Result, character);
		}
	}

	public class Information : Piece
	{
		public Information(City city, eSubjectType subjectType) : base(city, subjectType)
		{
		}
	}

	public class Broker : Piece
	{
		public Broker(City city, eSubjectType subjectType) : base(city, subjectType)
		{
		}
	}

	public class QuestPiece : Piece
	{
		private Quest quest;
		private Character character;

		private int pastDays;

		public delegate void AddQuestHandler(QuestPiece piece);
		public static event AddQuestHandler AddQuest;

		public QuestPiece(Quest quest, Character character, City city, eSubjectType subjectType) : base(city, subjectType)
		{
			this.quest = quest;
			this.character = character;
			pastDays = 0;

			AddQuest(this);

			TimeTable.Instance.NotifyEveryday += DayIsGone;
		}

		public void DayIsGone()
		{
			pastDays++;
			quest.PastDays++;
		}

		public IEnumerator TreatPastQuests(Character character)
		{
			if(CheckDuration())
			{
				GameManager.Instance.OpenEventUI();

				GameManager.FindObjectOfType<UIEventManager>().OnChanged(eUIEventLabelType.RESULT, CurQuest.FailureEffects.Script);
				
				if(CurQuest.FailureEffects.EffectAmounts != null)
				{
					string effectScript = string.Empty;
					foreach (EffectAmount effectAmount in CurQuest.FailureEffects.EffectAmounts)
					{
						if (effectAmount == null)
							continue;

						effectScript += effectAmount.ToString();
					}
					GameManager.FindObjectOfType<UIEventManager>().OnChanged(eUIEventLabelType.RESULT_EFFECT, effectScript);
				}

				yield return EventManager.Instance.WaitUntilFinish();

				GameManager.FindObjectOfType<UIQuestManager>().DeleteQuest(this.CurQuest);
				PieceManager.Instance.Delete(this);
			}
		}

		public override bool CheckDuration()
		{
			return pastDays >= quest.Duration;
		}

		public Quest CurQuest
		{
			get
			{
				return quest;
			}
		}

		public int PastDays
		{
			get
			{
				return pastDays;
			}
		}
	}
}