﻿using System;
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
			List<City> path = CityManager.Instance.CalcPath(city, destCity);

			foreach (City nextCity in path)
			{
				city = nextCity;
				PieceManager.Instance.SetVisible();
				if(this.iconPiece.gameObject.activeSelf)
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
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(city, movement);
			
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

		private int pastWeeks;

		public delegate void AddQuestHandler(QuestPiece piece);
		public static event AddQuestHandler AddQuest;

		public QuestPiece(Quest quest, Character character, City city, eSubjectType subjectType) : base(city, subjectType)
		{
			this.quest = quest;
			this.character = character;
			
			//TimeTable.Instance.NotifyEveryWeek += WeekIsGone;

			AddQuest(this);
		}

		public void WeekIsGone()
		{
			pastWeeks++;
		}

		public IEnumerator TreatPastQuests(Character character)
		{
			if(CheckDuration())
			{
				GameManager.Instance.OpenEventUI();

				GameManager.FindObjectOfType<UIEventManager>().OnChanged(eUIEventLabelType.RESULT, CurQuest.FailureEffects.Script);

				string effectScript = string.Empty;
				foreach(EffectAmount effectAmount in CurQuest.FailureEffects.EffectAmounts)
				{
					effectScript += effectAmount.ToString();
				}
				GameManager.FindObjectOfType<UIEventManager>().OnChanged(eUIEventLabelType.RESULT_EFFECT, effectScript);

				yield return EventManager.Instance.WaitUntilFinish();

				GameManager.FindObjectOfType<UIQuestManager>().DeleteQuest(this.CurQuest);

				yield return null;
			}
		}

		public override bool CheckDuration()
		{
			return pastWeeks >= quest.Duration;
		}

		public Quest CurQuest
		{
			get
			{
				return quest;
			}
		}

		public int PastWeeks
		{
			get
			{
				return pastWeeks;
			}
		}
	}
}