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

		public eEventAction ActionName
		{
			get
			{
				return actionName;
			}
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
			
			if (character.CheckSpecialEvent())
			{
				actionName = eEventAction.REST_SPECIAL;
			}
			else
			{
				actionName = eEventAction.REST;
			}

			yield return GameManager.Instance.ShowStateLabel(actionName.ToString() + " command activated.", 0.5f);

			Event selectedEvent = EventManager.Instance.Find(actionName);
			if (selectedEvent == null)
			{
				Debug.LogError("selectedEvent is null");
				yield break;
			}

			GameManager.Instance.OpenEventUI();

			Debug.Log(selectedEvent.ActionType + " is activated.");

			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, selectedEvent.Script);
			
			// deal with result
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
			EventManager.Instance.CalculateTestResult(selectedEvent.Result.TestStat, character);
			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

			int testSuccessNum = EventManager.Instance.TestSuccessNum;

			if(actionName == eEventAction.REST)
			{
				// TODO : 숨기 휴식 testSuccessNum -2
				yield return BuffManager.Instance.Rest_Cure_PatienceTest(character, testSuccessNum);
			}
			else
			{
				yield return EventManager.Instance.TreatResult(selectedEvent.Result, character);
			}

			Debug.Log("DoCommand Finished.");
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP++;
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
			
			if (character.CheckSpecialEvent())
			{
				actionName = eEventAction.WORK_START | eEventAction.WORK_END;
			}
			else
			{
				actionName = eEventAction.WORK;
			}

			yield return EventManager.Instance.DoCommand(actionName, character);
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

			character.AP++;
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

			if (character.CheckSpecialEvent())
			{
				// TODO : have to add bus action
				actionName = eEventAction.MOVE;
				yield return EventManager.Instance.DoCommand(actionName, character);
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
				int pathAP = 0;
				foreach(BezierPoint point in path)
				{
					if(point.GetComponent<IconCity>().type == eNodeType.MOUNTAIN)
					{
						pathAP += 2;
					}
					else
					{
						pathAP++;
					}
				}
				if(pathAP > character.RemainAP)
				{
					Debug.LogError("path is too long.");
					yield break;
				}

				character.AP += pathAP;
				foreach (BezierPoint point in path)
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
			//actionName = eEventAction.QUEST;
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
				if (character.CheckSpecialEvent())
				{
					actionName = eEventAction.INSPECT_SPECIAL;
				}
				else
				{
					actionName = eEventAction.INSPECT;
				}
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

			//GameManager.Instance.uiEventManager.OpenUI();

			//yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
			//int testSucceedDiceNum = DiceTester.Instance.Test(character.Stat.Bargain);
			//yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

			//GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, "협상력 테스트를 통한 할인 금액");
			//GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testSucceedDiceNum.ToString());

			//yield return EventManager.Instance.WaitUntilFinish();


			NGUIDebug.Log("Enter To Shop action");
			GameManager.Instance.shopUIObj.SetActive(true);
			//GameManager.Instance.shopUIObj.GetComponent<UIShop>().DiscountNum = testSucceedDiceNum;
			yield return EventManager.Instance.WaitUntilFinish();
			
		}
	}

	public class InfoAction : Action
	{
		public InfoAction()
		{
			startTime = eStartTime.INFO;
			//actionName = eEventAction.INFO;
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