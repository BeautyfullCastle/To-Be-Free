using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	[Serializable]
	public class PieceSaveData
	{
		public string type;
		public int cityIndex;
		// for Police
		public int power;
		public int movement;
	}

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

			if (city != null)
			{
				city.IconCity.PutPiece(iconPiece);
			}
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

		public Police(City city, eSubjectType subjectType, int power, int movement) : this(city, subjectType)
		{
			max = 5;

			Power = power;
			Movement = movement;

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

			this.iconPiece.PlayExclamation();
			yield return GameManager.Instance.MoveDirectingCam(new List<Transform> { iconPiece.transform }, 1f);
		}

		public IEnumerator MoveToRandomCity()
		{
			// city list can move.
			List<City> cityList = CityManager.Instance.FindCitiesByDistance(city, movement, eWay.NORMALWAY);
			
			// police can't go to the mountain.
			cityList.RemoveAll(x => x.Type == eNodeType.MOUNTAIN);

			this.iconPiece.PlayExclamation();

			TipManager.Instance.Show(eTipTiming.PoliceMove);
			
			yield return MoveTo(cityList[UnityEngine.Random.Range(0, cityList.Count)]);
		}

		public IEnumerator MoveTo(City destCity)
		{
			List<City> path = CityManager.Instance.CalcPath(city, destCity, eEventAction.MOVE);
			if (path == null)
				yield break;
			else if (path.Count == 0)
				yield break;

			foreach (City nextCity in path)
			{
				if (this.iconPiece.gameObject.activeSelf)
				{
					city.IconCity.PutOutPiece(this.iconPiece);
					yield return CityManager.Instance.MoveTo(iconPiece.transform, city, nextCity, 1f);
				}

				city = nextCity;
				Character character = GameManager.Instance.Character;
				if (character == null)
				{
					continue;
				}
					
				character.Stat.SetViewRange();
				city.IconCity.PutPiece(this.iconPiece);

				// 집중단속 중이고 캐릭터가 이미 구금된 상태가 아닐 때만 검문한다.
				if (character.IsDetention == false && CrackDown.Instance.IsCrackDown)
				{
					if (character.CurCity.Index == this.city.Index)
					{
						yield return this.Fight(eEventAction.INSPECT, character);

						if (EventManager.Instance.TestResult == false)
						{
							yield return character.Arrested(this, true);
							break;
						}
					}
				}
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

			TipManager.Instance.Show(eTipTiming.PoliceInspect);

			yield return GameManager.Instance.uiEventManager.OnChanged(selectedEvent.Script);

			int characterSuccessNum = 0;
			int policeSuccessNum = 0;
			yield return DiceTester.Instance.Test(eTestStat.AGILITY, character.Stat.Agility, this.Power, (x, x2) => { characterSuccessNum = x; policeSuccessNum = x2; });

			EventManager.Instance.TestResult = characterSuccessNum >= policeSuccessNum;

			yield return EventManager.Instance.TreatResult(selectedEvent.Result, character);
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
		public QuestPiece(City city, eSubjectType subjectType) : base(city, subjectType)
		{
		}
	}
}