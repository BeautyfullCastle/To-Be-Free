using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eCommand
	{
		MOVE, WORK, REST, SHOP, QUEST, INFO, BROKER, ESCAPE, SPECIAL
	}

	public class Action
	{
		protected eStartTime startTime;
		protected eEventAction actionName;

		public delegate void ActionEventHandler(eStartTime startTime, Character character);
		static public event ActionEventHandler ActionEventNotify;

		public virtual IEnumerator Activate(Character character)
		{
			if(ActionEventNotify != null)
				ActionEventNotify(startTime, character);
			
			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			yield return null;
		}
	}

	public class Rest : Action
	{

		public Rest()
		{
			startTime = eStartTime.REST;
			actionName = eEventAction.NULL;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.Log("Rest Action Activated.");

			yield return base.Activate(character);

			Debug.Log("Cure for Rest");
			character.Rest();

			yield return BuffManager.Instance.Rest_Cure_PatienceTest(character);

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
		}
	}

	public class Work : Action
	{
		public Work()
		{
			startTime = eStartTime.WORK;
			actionName = eEventAction.WORK;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Work action activated.");
			yield return base.Activate(character);
			yield return (EventManager.Instance.DoCommand(actionName, character));
			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
			
			// if effect is money and event is succeeded,
			if (EventManager.Instance.TestResult)
			{
				EffectAmount[] successResulteffects = EventManager.Instance.CurrResult.Success.EffectAmounts;

				for (int i = 0; i < successResulteffects.Length; ++i)
				{
					if (successResulteffects[i].Effect == null)
					{
						continue;
					}
					if (successResulteffects[i].Effect.SubjectType == eSubjectType.MONEY)
					{
						character.Stat.Money += character.CurCity.CalcRandWorkingMoney();
						break;
					}
				}
			}
		}
	}

	public class Move : Action
	{
		public Move()
		{
			startTime = eStartTime.MOVE;
			actionName = eEventAction.MOVE;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Move action Activated.");
			yield return base.Activate(character);

			string region = string.Empty;
			//if (character.CurCity.Area == eArea.MONGOLIA || character.NextCity.Area == eArea.MONGOLIA)
			//{
			//    region = eArea.MONGOLIA.ToString();
			//}
			//else if (character.NextCity.Area == eArea.SOUTHEAST_ASIA)
			//{
			//    region = eArea.SOUTHEAST_ASIA.ToString();
			//}

			if (region != string.Empty)
			{
				yield return GameManager.Instance.ShowStateLabel(actionName.ToString() + " command activated.", 0.5f);
				Event[] events = Array.FindAll(EventManager.Instance.List, x => x.Region == region);
				System.Random r = new System.Random();
				int index = r.Next(0, events.Length - 1);
				Event selectedEvent = events[index];
				if (selectedEvent != null)
				{
					yield return EventManager.Instance.ActivateEvent(selectedEvent, character);
				}
			}
			else
			{
				yield return (EventManager.Instance.DoCommand(actionName, character));
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			EffectAmount[] effects = null;
			if(EventManager.Instance.TestResult)
			{
				effects = EventManager.Instance.CurrResult.Success.EffectAmounts;
			}
			else
			{
				effects = EventManager.Instance.CurrResult.Failure.EffectAmounts;
			}
			bool dontMove = false;
			if (effects != null)
			{
				for (int i = 0; i < effects.Length; ++i)
				{
					if (effects[i].Effect == null)
					{
						continue;
					}
					if (effects[i].Effect.SubjectType == eSubjectType.CHARACTER
						&& effects[i].Effect.VerbType == eVerbType.MOVE)
					{
						dontMove = true;
						break;
					}
				}
			}

			if (dontMove == false)
			{
				List<BezierPoint> path = CityManager.Instance.CalcPath();
				foreach(BezierPoint point in path)
				{
					yield return character.MoveTo(point);
				}
			}
			//Debug.LogWarning("character is moved to " + character.CurCity.Name);
			
		}
	}

	public class QuestAction : Action
	{
		public QuestAction()
		{
			startTime = eStartTime.QUEST;
			actionName = eEventAction.QUEST;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Quest action Activated.");

			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			List<Piece> quests = PieceManager.Instance.FindAll(eSubjectType.QUEST);
			QuestPiece questPiece = quests.Find(x => x.City == character.CurCity) as QuestPiece;

			Quest quest = questPiece.CurQuest;
			EventManager.Instance.TestResult = quest.CheckCondition(character);
			if (EventManager.Instance.TestResult)
			{
				yield return QuestManager.Instance.ActivateQuest(quest, true, character);
			}

			// have to check TestResult again cause of Dice Test of activated quest.
			if (EventManager.Instance.TestResult == true)
			{
				PieceManager.Instance.Delete(questPiece);
				GameManager.FindObjectOfType<UIQuestManager>().DeleteQuest(quest);
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			Debug.Log("character quest activated.");
		}
	}

	public class Inspect : Action
	{
		public Inspect()
		{
			startTime = eStartTime.INSPECT;
			actionName = eEventAction.INSPECT;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Inpect action activated.");
			
			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			List<Piece> policesInThisCity = PieceManager.Instance.FindAll(eSubjectType.POLICE).FindAll(x=>x.City == character.CurCity);
			Debug.LogWarning("policesInThisCity.Count : " + policesInThisCity.Count);
			for (int i = 0; i < policesInThisCity.Count; ++i)
			{
				yield return EventManager.Instance.DoCommand(actionName, character);
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
		}
	}

	public class DetentionAction : Action
	{
		public DetentionAction()
		{
			startTime = eStartTime.DETENTION;
			actionName = eEventAction.DETENTION;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Detention action activated.");
			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			GameManager.Instance.uiEventManager.OpenUI();

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
			bool testResult = (DiceTester.Instance.Test(character.Stat.Agility) > 0);
			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, "Detention Test");
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testResult.ToString());

			yield return EventManager.Instance.WaitUntilFinish();

			if(testResult == true)
			{
				yield return AbnormalConditionManager.Instance.Find("Detention").DeActivate(character);
			}
			else
			{
				yield return character.HaulIn();
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
		}
	}

	public class EnterToShop : Action
	{
		public EnterToShop()
		{
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Enter to Shop action activated.");

			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			GameManager.Instance.uiEventManager.OpenUI();

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
			int testSucceedDiceNum = DiceTester.Instance.Test(character.Stat.Bargain);
			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, "협상력 테스트를 통한 할인 금액");
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testSucceedDiceNum.ToString());

			yield return EventManager.Instance.WaitUntilFinish();


			NGUIDebug.Log("Enter To Shop action");
			GameManager.Instance.shopUIObj.SetActive(true);
			GameManager.Instance.shopUIObj.GetComponent<UIShop>().DiscountNum = testSucceedDiceNum;
			yield return EventManager.Instance.WaitUntilFinish();
			
		}
	}

	public class InfoAction : Action
	{
		public InfoAction()
		{
			startTime = eStartTime.INFO;
			actionName = eEventAction.INFO;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Info action activated.");
			NGUIDebug.Log("Info action");

			Piece piece = PieceManager.Instance.GetPieceOfCity(eSubjectType.INFO, character.CurCity);
			if(piece != null)
			{
				yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);
				yield return EventManager.Instance.DoCommand(actionName, character);
				yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
				PieceManager.Instance.Delete(piece);
			}

			// spawn broker if character's info piece's number is 3.
			// and delete 3 info pieces.
			if(character.Stat.InfoNum >= 3)
			{
				City cityOfBroker = CityManager.Instance.FindRandCityByDistance(character.CurCity, 2, eSubjectType.BROKER);
				Piece broker = new Piece(cityOfBroker, eSubjectType.BROKER);
				PieceManager.Instance.Add(broker);
				character.Stat.InfoNum -= 3;
				yield return GameManager.Instance.MoveDirectingCam(new List<Transform>() {
					GameManager.Instance.FindGameObject(cityOfBroker.Name.ToString()).transform }, 2f);
			}
			yield return null;

		}
	}

	public class BrokerAction : Action
	{
		public BrokerAction()
		{
			startTime = eStartTime.BROKER;
			actionName = eEventAction.BROKER;
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("BrokerAction activated.");
			NGUIDebug.Log("BrokerAction action");

			yield return base.Activate(character);
			yield return EventManager.Instance.DoCommand(actionName, character);
			if (EventManager.Instance.TestResult)
			{
				PieceManager.Instance.Delete(PieceManager.Instance.Find(eSubjectType.BROKER, character.CurCity));
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

		}
	}
}