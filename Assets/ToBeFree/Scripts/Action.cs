﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eCommand
	{
		MOVE, WORK, REST, SHOP, BROKER, QUEST, SPECIAL, INVESTIGATION, ABILITY,
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

			// 추가 행동만큼 주사위 +
			character.Stat.TempDiceNum = 0;
			character.Stat.TempDiceNum += requiredTime - 1;

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

			GameManager.Instance.OpenEventUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
				LanguageManager.Instance.Find(eLanguageKey.Event_Start_Rest));
			yield return EventManager.Instance.WaitUntilFinish();

			yield return base.Activate(character);
			
			if (character.CheckSpecialEvent())
			{
				if (actionName == eEventAction.REST)
					actionName = eEventAction.REST_SPECIAL;
				else if (actionName == eEventAction.HIDE)
					actionName = eEventAction.HIDE_SPECIAL;

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

				yield return EventManager.Instance.DoCommand(actionName, character);

				yield return TimeTable.Instance.SpendRemainTime();
			}
			else
			{
				yield return GameManager.Instance.ShowStateLabel(actionName.ToString() + " command activated.", 0.5f);

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

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
				yield return EventManager.Instance.CalculateTestResult(selectedEvent.Result.TestStat, character);
				yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);
				
				int testSuccessNum = EventManager.Instance.TestSuccessNum + requiredTime - 1;

				if(actionName == eEventAction.HIDE)
				{
					testSuccessNum -= 2;
				}

				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, testSuccessNum.ToString());

				yield return BuffManager.Instance.Rest_Cure_PatienceTest(character, testSuccessNum);

				Debug.Log("DoCommand Finished.");
			}
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime;
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

			GameManager.Instance.OpenEventUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
				LanguageManager.Instance.Find(eLanguageKey.Event_Start_Working));
			yield return EventManager.Instance.WaitUntilFinish();

			yield return base.Activate(character);
			
			if (character.CheckSpecialEvent())
			{
				int randActionType = UnityEngine.Random.Range(0, 1);
				if (randActionType == 0) {
					actionName = eEventAction.WORK_START;
				}
				else {
					actionName = eEventAction.WORK_END;
				}

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

				yield return EventManager.Instance.DoCommand(actionName, character);

				yield return TimeTable.Instance.SpendRemainTime();
			}
			else
			{
				actionName = eEventAction.WORK;

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

				yield return EventManager.Instance.DoCommand(actionName, character);
			}
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
						character.Stat.Money += successResulteffects[i].Amount;
						character.Stat.Money += character.CurCity.CalcRandWorkingMoney();
						character.Stat.Money += EventManager.Instance.TestSuccessNum;
						GameManager.Instance.OpenEventUI();
						GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, 
							"기본급 : " + successResulteffects[i].Amount
							+ "\n도시 추가금 : " + character.CurCity.CalcRandWorkingMoney()
							+ "\n주사위 성공 개수 추가금 : " + EventManager.Instance.TestSuccessNum);

						yield return EventManager.Instance.WaitUntilFinish();
						break;
					}
				}
			}

			character.AP += requiredTime;
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

			string event_start_script = string.Empty;
			if (actionName == eEventAction.MOVE_BUS)
			{
				event_start_script = LanguageManager.Instance.Find(eLanguageKey.Event_Start_Bus);
			}
			else if (actionName == eEventAction.MOVE)
			{
				event_start_script = LanguageManager.Instance.Find(eLanguageKey.Event_Start_Walking);
			}
			GameManager.Instance.OpenEventUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, event_start_script);
			yield return EventManager.Instance.WaitUntilFinish();

			yield return base.Activate(character);

			bool dontMove = false;

			if (character.CheckSpecialEvent())
			{
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
				List<City> path = CityManager.Instance.CalcPath(character.CurCity, character.NextCity, actionName);
				
				if(actionName == eEventAction.MOVE_BUS)
				{
					character.Stat.Money -= 4;
				}

				foreach (City city in path)
				{
					// 집중 단속 기간이면 공안 단속 들어감
					if(CrackDown.Instance.IsCrackDown)
					{
						yield return inspectAction.Activate(character);
					}

					if (character.IsDetention == false)
					{
						yield return character.MoveTo(city);
					}
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

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);
			
			List<Piece> quests = PieceManager.Instance.FindAll(eSubjectType.QUEST);
			QuestPiece questPiece = quests.Find(x => x.City == character.CurCity) as QuestPiece;

			Quest quest = questPiece.CurQuest;
			EventManager.Instance.TestResult = quest.CheckCondition(character);
			if (EventManager.Instance.TestResult)
			{
				yield return QuestManager.Instance.ActivateQuest(quest, character);
			}

			// have to check TestResult again cause of Dice Test of activated quest.
			if (EventManager.Instance.TestResult == true)
			{
				//PieceManager.Instance.Delete(questPiece);
				GameManager.FindObjectOfType<UIQuestManager>().DeleteQuest(quest);
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime;

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
			TipManager.Instance.Show(eTipTiming.Inspect);
			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			List<Piece> policesInThisCity = PieceManager.Instance.FindAll(eSubjectType.POLICE).FindAll(x=>x.City == character.CurCity);
			Debug.LogWarning("policesInThisCity.Count : " + policesInThisCity.Count);
			for (int i = 0; i < policesInThisCity.Count; ++i)
			{
				if (character.IsDetention)
					break;

				if (character.CheckSpecialEvent())
				{
					actionName = eEventAction.INSPECT_SPECIAL;
					yield return EventManager.Instance.DoCommand(actionName, character);
				}
				else
				{
					actionName = eEventAction.INSPECT;

					yield return GameManager.Instance.ShowStateLabel(actionName.ToString() + " command activated.", 0.5f);

					Police police = policesInThisCity[i] as Police;
					yield return police.Fight(actionName, character);

					if (EventManager.Instance.TestResult == false)
					{
						character.CaughtPolice = police;
						List<City> pathToTumen = CityManager.Instance.CalcPath(character.CurCity, CityManager.Instance.Find("TUMEN"), eEventAction.MOVE);
						List<City> pathToDandong = CityManager.Instance.CalcPath(character.CurCity, CityManager.Instance.Find("DANDONG"), eEventAction.MOVE);

						CityManager.Instance.FindNearestPath(pathToTumen, pathToDandong);
						int remainAP = character.RemainAP;
						character.AP = character.TotalAP;
						character.IsDetention = true;
						yield return TimeTable.Instance.SpendTime(remainAP, eSpendTime.END);
					}
				}
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
			yield return base.Activate(character);

			TipManager.Instance.Show(eTipTiming.Detention);
			// ActState : 체포상태에서 체포된 공안의 이동력만큼 이동
			if (GameManager.Instance.State == GameManager.GameState.Detention)
			{
				yield return character.HaulIn();
			}
			/* 밤단계 때 
			 * 단둥 또는 투먼시에 도착하지 않았으면 공안체크로 탈출 시도 이벤트
			 * 단둥 또는 투먼시에 도착하면 밤단계에 수용소이벤트 출력
			 * 수용소 이벤트는 그냥 호출
			 * - 수용소이벤트 실패 시 북송 게임오버
			 * */
			else if (GameManager.Instance.State == GameManager.GameState.Night)
			{
				bool isLastCity = (character.CurCity.Name == "DANDONG" || character.CurCity.Name == "TUMEN");

				if (isLastCity)
				{
					TipManager.Instance.Show(eTipTiming.Camp);
					actionName = eEventAction.CAMP;
					yield return EventManager.Instance.DoCommand(actionName, character);
				}
				else if (character.CheckSpecialEvent())
				{
					actionName = eEventAction.DETENTION_SPECIAL;
					yield return EventManager.Instance.DoCommand(actionName, character);
				}
				else
				{
					actionName = eEventAction.DETENTION;
					yield return character.CaughtPolice.Fight(actionName, character);
				}
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

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

			NGUIDebug.Log("Enter To Shop action");
			GameManager.Instance.shopUIObj.SetActive(true);
			//GameManager.Instance.shopUIObj.GetComponent<UIShop>().DiscountNum = testSucceedDiceNum;
			yield return EventManager.Instance.WaitUntilFinish();
			
		}
	}

	public class Investigation : Action
	{
		public Investigation()
		{
			startTime = eStartTime.INVESTIGATION;
			//actionName = eEventAction.INFO;
		}

		public override IEnumerator Activate(Character character)
		{
			string event_start_script = string.Empty;
			if (actionName == eEventAction.INVESTIGATION_BROKER)
			{
				event_start_script = LanguageManager.Instance.Find(eLanguageKey.Event_Start_BrokerInfoInvestigation);
			}
			else if (actionName == eEventAction.INVESTIGATION_CITY)
			{
				event_start_script = LanguageManager.Instance.Find(eLanguageKey.Event_Start_CityInvestigation);
			}
			else if (actionName == eEventAction.INVESTIGATION_POLICE)
			{
				event_start_script = LanguageManager.Instance.Find(eLanguageKey.Event_Start_PoliceInvestigation);
			}
			else if (actionName == eEventAction.GATHERING)
			{
				event_start_script = LanguageManager.Instance.Find(eLanguageKey.Event_Start_Gathering);
			}
			GameManager.Instance.OpenEventUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, event_start_script);
			yield return EventManager.Instance.WaitUntilFinish();


			yield return base.Activate(character);
			
			// 도시크기 별 주사위 추가 - 중도시 주사위1, 대도시는 주사위2
			if(character.CurCity.Type == eNodeType.MIDDLECITY)
			{
				character.Stat.TempDiceNum += 1;
			}
			else if (character.CurCity.Type == eNodeType.BIGCITY)
			{
				character.Stat.TempDiceNum += 2;
			}
			
			// 스페셜 이벤트 처리
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

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

				yield return EventManager.Instance.DoCommand(actionName, character);

				yield return TimeTable.Instance.SpendRemainTime();
			}
			// 일반 조사
			else
			{
				yield return GameManager.Instance.ShowStateLabel(actionName.ToString() + " command activated.", 0.5f);

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

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
				yield return EventManager.Instance.CalculateTestResult(selectedEvent.Result.TestStat, character);
				yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

				int testSuccessNum = EventManager.Instance.TestSuccessNum;
				character.Stat.TempDiceNum = 0;

				ArrayList list = new ArrayList(3);
				ArrayList finalList = new ArrayList();

				if (ActionName == eEventAction.INVESTIGATION_BROKER)
				{
					Quest quest = QuestManager.Instance.FindRand(eQuestActionType.QUEST_BROKERINFO);
					int questIndex = QuestManager.Instance.IndexOf(quest);
					EffectAmount brokerInfoQuest = new EffectAmount(EffectManager.Instance.Find(eSubjectType.QUEST, eVerbType.LOAD), questIndex);
					EffectAmount brokerInfo = new EffectAmount(EffectManager.Instance.Find(eSubjectType.CHARACTER, eVerbType.ADD, eObjectType.INFO), 1);

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
				// 도시 조사 : 돈, 퀘스트, 아이템 중 1, 2, 3 (중복 없음)
				else if (ActionName == eEventAction.INVESTIGATION_CITY)
				{
					int money = 2;
					Effect moneyEffect = EffectManager.Instance.Find(eSubjectType.MONEY, eVerbType.ADD, eObjectType.SPECIFIC);
					EffectAmount moneyEffectAmount = new EffectAmount(moneyEffect, money);

					Quest quest = QuestManager.Instance.FindRand(eQuestActionType.QUEST);
					int questIndex = QuestManager.Instance.IndexOf(quest);
					Effect questEffect = EffectManager.Instance.Find(eSubjectType.QUEST, eVerbType.LOAD);
					EffectAmount questEffectAmount = new EffectAmount(questEffect, questIndex);

					Effect itemEffect = EffectManager.Instance.Find(eSubjectType.ITEM, eVerbType.ADD, eObjectType.ALL);					
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
				//공안 조사 : 하루 시야 증가, 클릭한 도시의 공안 수, 집중단속확률 중 1, 2, 3 (중복 없음)
				else if (ActionName == eEventAction.INVESTIGATION_POLICE)
				{
					AbnormalCondition addViewRange = AbnormalConditionManager.Instance.Find("Add View Range");
					EffectAmount revealPosition = new EffectAmount(EffectManager.Instance.Find(eSubjectType.POLICE, eVerbType.REVEAL, eObjectType.NUMBER), 1);
					EffectAmount getProbability = new EffectAmount(EffectManager.Instance.Find(eSubjectType.POLICE, eVerbType.REVEAL, eObjectType.CRACKDOWN_PROBABILITY), 1);
					
					list.Add(addViewRange);
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
				//식량아이템 얻기 차는량이 하나 또는 둘 또는 셋
				else if (ActionName ==  eEventAction.GATHERING)
				{
					Item[] foods = ItemManager.Instance.FindAll(ItemTag.FOOD);

					if (testSuccessNum >= 1)
					{
						list.Add(foods[0]);
						if (testSuccessNum >= 2)
						{
							list.Add(foods[1]);
							if (testSuccessNum >= 3)
							{
								list.Add(foods[2]);
							}
						}
						int rand = UnityEngine.Random.Range(0, list.Count);
						finalList.Add(list[rand]);
					}
				}

				string resultEffectScript = string.Empty;
				string resultScript = ActionName.ToString() + " Result";
				foreach (var item in finalList)
				{
					if(item is EffectAmount)
					{
						EffectAmount effectAmount = item as EffectAmount;
						if (effectAmount.Effect == null)
						{
							continue;
						}
						resultEffectScript += effectAmount.ToString() + "\n";
					}
					else if(item is AbnormalCondition)
					{
						AbnormalCondition abnormalCondition = item as AbnormalCondition;
						resultEffectScript += "Buff : " + abnormalCondition.Name + "\n";
					}
					else if(item is Item)
					{
						Item addingItem = item as Item;
						resultEffectScript += "Item : " + addingItem.Name + "\n";
					}
				}
				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.RESULT, resultScript);
				GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.RESULT_EFFECT, resultEffectScript);

				yield return EventManager.Instance.WaitUntilFinish();

				foreach (var item in finalList)
				{
					if (item is EffectAmount)
					{
						EffectAmount effectAmount = item as EffectAmount;
						if (effectAmount.Effect == null)
						{
							continue;
						}
						yield return effectAmount.Activate(character);
					}
					else if (item is AbnormalCondition)
					{
						AbnormalCondition abnormalCondition = item as AbnormalCondition;
						yield return abnormalCondition.Activate(character);
					}
					else if (item is Item)
					{
						Item addingItem = item as Item;
						character.Inven.AddItem(addingItem, character);
					}
				}
			}
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime;
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
			NGUIDebug.Log("BrokerAction action");

			GameManager.Instance.OpenEventUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
				LanguageManager.Instance.Find(eLanguageKey.Event_Start_Broker));
			yield return EventManager.Instance.WaitUntilFinish();

			yield return base.Activate(character);

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

			yield return EventManager.Instance.DoCommand(actionName, character);

			yield return TimeTable.Instance.SpendRemainTime();

			if (EventManager.Instance.TestResult)
			{
				PieceManager.Instance.Delete(PieceManager.Instance.Find(eSubjectType.BROKER, character.CurCity));
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime;
		}
	}

	public class AbilityAction : Action
	{
		public AbilityAction()
		{
			startTime = eStartTime.ABILITY;
			actionName = eEventAction.ABILITY;
		}

		public override IEnumerator Activate(Character character)
		{
			NGUIDebug.Log("AbilityAction action");

			GameManager.Instance.OpenEventUI();
			GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
				LanguageManager.Instance.Find(eLanguageKey.Event_Start_Ability));
			yield return EventManager.Instance.WaitUntilFinish();

			yield return base.Activate(character);

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

			yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[character.EventIndex], character);

			yield return TimeTable.Instance.SpendRemainTime();

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			character.AP += requiredTime;
		}
	}
}