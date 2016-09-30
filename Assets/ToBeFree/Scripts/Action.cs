using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eCommand
	{
		MOVE, WORK, REST, SHOP, BROKER, ESCAPE, SPECIAL, INVESTIGATION,
		NULL
	}

	public class Action
	{
		protected eStartTime startTime;
		protected eEventAction actionName;
		protected int requiredTime;

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
			set
			{
				actionName = value;
			}
		}

		public int RequiredTime
		{
			get
			{
				return requiredTime;
			}

			set
			{
				requiredTime = value;
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
				if (actionName == eEventAction.REST)
					actionName = eEventAction.REST_SPECIAL;
				else if (actionName == eEventAction.HIDE) 
					actionName = eEventAction.HIDE_SPECIAL;

				yield return EventManager.Instance.DoCommand(actionName, character);
			}
			else
			{
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

				int testSuccessNum = EventManager.Instance.TestSuccessNum + requiredTime;

				if(actionName == eEventAction.HIDE)
				{
					testSuccessNum -= 2;
				}

				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testSuccessNum.ToString());

				yield return BuffManager.Instance.Rest_Cure_PatienceTest(character, testSuccessNum);

				Debug.Log("DoCommand Finished.");
			}
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime + 1;
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

			character.AP += requiredTime + 1;
		}
	}

	public class Move : Action
	{
		private Inspect inspectAction;

		public Move()
		{
			startTime = eStartTime.MOVE;
			actionName = eEventAction.MOVE;
			inspectAction = new Inspect();
		}

		public override IEnumerator Activate(Character character)
		{
			Debug.LogWarning("Move action Activated.");
			yield return base.Activate(character);

			bool dontMove = false;

			if (character.CheckSpecialEvent())
			{
				// TODO : have to add bus action
				actionName = eEventAction.MOVE;
				yield return EventManager.Instance.DoCommand(actionName, character);
				
				EffectAmount[] effects = null;
				if (EventManager.Instance.TestResult)
				{
					effects = EventManager.Instance.CurrResult.Success.EffectAmounts;
				}
				else
				{
					effects = EventManager.Instance.CurrResult.Failure.EffectAmounts;
				}

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

				yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
			}

			if (dontMove == false)
			{
				List<City> path = CityManager.Instance.CalcPath(character.CurCity, character.NextCity);
				int pathAP = 0;
				foreach (City city in path)
				{
					if (city.Type == eNodeType.MOUNTAIN)
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
				foreach (City city in path)
				{
					// TODO : 집중 단속 기간 추가
					if(actionName != eEventAction.HIDE)
					{
						yield return inspectAction.Activate(character);
					}
					yield return character.MoveTo(city);
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

			base.Activate(character);

			if (character.CheckSpecialEvent())
			{
				if(actionName == eEventAction.INVESTIGATION_BROKER)
				{
					actionName = eEventAction.INVESTIGATION_BROKER_SPECIAL;
				}
				else if(actionName == eEventAction.INVESTIGATION_CITY)
				{
					actionName = eEventAction.INVESTIGATION_CITY_SPECIAL;
				}
				else if(actionName == eEventAction.INVESTIGATION_POLICE)
				{
					actionName = eEventAction.INVESTIGATION_POLICE_SPECIAL;
				}
				else if(actionName == eEventAction.GATHERING)
				{
					actionName = eEventAction.GATHERING_SPECIAL;
				}

				yield return EventManager.Instance.DoCommand(actionName, character);
			}
			// 일반 조사
			else
			{
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

				int testSuccessNum = EventManager.Instance.TestSuccessNum + requiredTime;
				
				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testSuccessNum.ToString());

				List<EffectAmount> list = new List<EffectAmount>(3);
				List<EffectAmount> finalList = new List<EffectAmount>();

				if (ActionName == eEventAction.INVESTIGATION_BROKER)
				{
					Quest quest = QuestManager.Instance.FindRand(eQuestActionType.QUEST_BROKERINFO);
					int questIndex = QuestManager.Instance.IndexOf(quest);
					EffectAmount brokerInfoQuest = new EffectAmount(new Effect(eSubjectType.QUEST, eVerbType.LOAD), questIndex);

					EffectAmount brokerInfo = new EffectAmount(new Effect(eSubjectType.CHARACTER, eVerbType.ADD, eObjectType.INFO), 1);

					//하나성공: 브로커정보 퀘스트 받음
					if (testSuccessNum == 1)
					{
						finalList.Add(brokerInfoQuest);
					}
					//둘성공: 브로커정보 퀘스트 또는 브로커정보를 얻음
					if (testSuccessNum == 2)
					{
						list.Add(brokerInfoQuest);
						list.Add(brokerInfo);

						finalList.Add(list[UnityEngine.Random.Range(0, list.Count)]);
					}
					//셋성공: 브로커정보를 얻음
					else if (testSuccessNum >= 3)
					{
						finalList.Add(brokerInfo);
					}
				}
				else if (ActionName == eEventAction.INVESTIGATION_CITY)
				{
					//돈, 퀘스트, 아이템 중 1, 2, 3 (중복 없음)
					int money = 1;
					Effect moneyEffect = new Effect(eSubjectType.MONEY, eVerbType.ADD, eObjectType.SPECIFIC);
					EffectAmount moneyEffectAmount = new EffectAmount(moneyEffect, money);

					Effect questEffect = new Effect(eSubjectType.QUEST, eVerbType.LOAD);
					EffectAmount questEffectAmount = new EffectAmount(questEffect, UnityEngine.Random.Range(0, QuestManager.Instance.List.Length));

					Effect itemEffect = new Effect(eSubjectType.ITEM, eVerbType.ADD, eObjectType.ALL);					
					EffectAmount itemEffectAmount = new EffectAmount(itemEffect, -99);
					
					list.Add(moneyEffectAmount);
					list.Add(questEffectAmount);
					list.Add(itemEffectAmount);
					
					int rand = UnityEngine.Random.Range(0, 3);

					if (testSuccessNum == 1)
					{
						finalList.Add(list[rand]);
					}
					else if (testSuccessNum == 2)
					{
						list.RemoveAt(rand);
						finalList.AddRange(list);
					}
					else if (testSuccessNum >= 3)
					{
						finalList.AddRange(list);
					}
				}
				else if(ActionName == eEventAction.INVESTIGATION_POLICE)
				{
					//공안의 스탯, 공안의 위치, 집중단속확률 중 1, 2, 3 (중복 없음)
					EffectAmount addStat = new EffectAmount(new Effect(eSubjectType.POLICE, eVerbType.ADD, eObjectType.STAT), 1);
					EffectAmount revealPosition = new EffectAmount(new Effect(eSubjectType.POLICE, eVerbType.REVEAL, eObjectType.POSITION), 1);
					EffectAmount getProbability = new EffectAmount(new Effect(eSubjectType.POLICE, eVerbType.REVEAL, eObjectType.CRACKDOWN_PROBABILITY), 1);

					list.Add(addStat);
					list.Add(revealPosition);
					list.Add(getProbability);

					int rand = UnityEngine.Random.Range(0, 3);

					if (testSuccessNum == 1)
					{
						finalList.Add(list[rand]);
					}
					else if (testSuccessNum == 2)
					{
						list.RemoveAt(rand);
						finalList.AddRange(list);
					}
					else if (testSuccessNum >= 3)
					{
						finalList.AddRange(list);
					}
				}

				EffectAmount[] effectAmounts = selectedEvent.Result.Success.EffectAmounts;
				effectAmounts = finalList.ToArray();
				
				string resultEffectScript = string.Empty;
				string resultScript = ActionName.ToString() + " Result";
				foreach (EffectAmount effectAmount in effectAmounts)
				{
					if (effectAmount.Effect == null)
					{
						continue;
					}
					resultEffectScript += effectAmount.ToString() + "\n";
				}
				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.RESULT, resultScript);
				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.RESULT_EFFECT, resultEffectScript);

				yield return EventManager.Instance.WaitUntilFinish();

				foreach (EffectAmount effectAmount in effectAmounts)
				{
					if (effectAmount.Effect == null)
					{
						continue;
					}
					yield return effectAmount.Activate(character);
				}

				Debug.Log("DoCommand Finished.");
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime + 1;
			
			//// 이전 브로커 정보 처리했던 부분
			//Piece piece = PieceManager.Instance.GetPieceOfCity(eSubjectType.INFO, character.CurCity);
			//if(piece != null)
			//{
			//	yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);
			//	yield return EventManager.Instance.DoCommand(actionName, character);
			//	yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
			//	PieceManager.Instance.Delete(piece);
			//}

			//// spawn broker if character's info piece's number is 3.
			//// and delete 3 info pieces.
			//if(character.Stat.InfoNum >= 3)
			//{
			//	City cityOfBroker = CityManager.Instance.FindRandCityByDistance(character.CurCity, 2, eSubjectType.BROKER);
			//	Piece broker = new Piece(cityOfBroker, eSubjectType.BROKER);
			//	PieceManager.Instance.Add(broker);
			//	character.Stat.InfoNum -= 3;
			//	yield return GameManager.Instance.MoveDirectingCam(new List<Transform>() {
			//		GameManager.Instance.FindGameObject(cityOfBroker.Name.ToString()).transform }, 2f);
			//}
			//yield return null;

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