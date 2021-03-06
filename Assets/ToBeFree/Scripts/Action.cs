﻿using System.Collections;
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
			yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Start_Rest));

			yield return base.Activate(character);

			character.AP += requiredTime;

			if (character.CheckSpecialEvent())
			{
				//if (actionName == eEventAction.REST)
					actionName = eEventAction.REST_SPECIAL;
				//else if (actionName == eEventAction.HIDE)
				//	actionName = eEventAction.HIDE_SPECIAL;

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

				yield return EventManager.Instance.DoCommand(actionName, character);

				yield return TimeTable.Instance.SpendRemainTime();
			}
			else
			{
				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

				Event selectedEvent = EventManager.Instance.Find(actionName);
				if (selectedEvent == null)
				{
					Debug.LogError("selectedEvent is null");
					yield break;
				}

				yield return GameManager.Instance.uiEventManager.OnChanged(selectedEvent.Script);

				if (actionName == eEventAction.HIDE)
				{
					character.Stat.TempDiceNum -= 2;
				}

				// deal with result
				yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
				if(selectedEvent.Result != null)
				{
					yield return EventManager.Instance.CalculateTestResult(selectedEvent.Result.TestStat, character);
				}
				yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

				yield return EventManager.Instance.TreatResult(selectedEvent.Result, character, true, false);

				//int testSuccessNum = EventManager.Instance.TestSuccessNum + requiredTime - 1;
				

				yield return BuffManager.Instance.Rest_Cure_PatienceTest(character, EventManager.Instance.TestSuccessNum);
			}
			
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
			
			yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Start_Working));
			
			yield return base.Activate(character);

			character.AP += requiredTime;

			if (character.CheckSpecialEvent())
			{
				int randActionType = UnityEngine.Random.Range(0, 2);
				if (randActionType == 0) {
					actionName = eEventAction.WORK_START;

					yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

					yield return EventManager.Instance.DoCommand(actionName, character);

					yield return TimeTable.Instance.SpendRemainTime();
				}
				else {
					actionName = eEventAction.WORK_END;

					yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

					yield return EventManager.Instance.DoCommand(actionName, character);
				}
			}
			else
			{
				actionName = eEventAction.WORK;

				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

				Event selectedEvent = EventManager.Instance.Find(actionName);
				if (selectedEvent == null)
				{
					Debug.LogError("selectedEvent is null");
					yield break;
				}

				yield return GameManager.Instance.uiEventManager.OnChanged(selectedEvent.Script);

				if (selectedEvent.Result != null)
				{
					yield return EventManager.Instance.CalculateTestResult(selectedEvent.Result.TestStat, character);
				}

				yield return EventManager.Instance.TreatResult(selectedEvent.Result, character, true, false);

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
							int basicMoney = successResulteffects[i].Amount;
							//int randWorkingMoneyOfCity = character.CurCity.CalcRandWorkingMoney();
							//int earnedMoney = randWorkingMoneyOfCity;
							int TestSuccesMoney = EventManager.Instance.TestSuccessNum;
							yield return GameManager.Instance.uiEventManager.OnChanged(
								//LanguageManager.Instance.Find(eLanguageKey.Event_WoringMoneyPerCity) + " " + randWorkingMoneyOfCity + "\n" + 
								LanguageManager.Instance.Find(eLanguageKey.Event_SucceedDiceNumber) + ": " + TestSuccesMoney + "\n" +
								LanguageManager.Instance.Find(eLanguageKey.Event_TotalMoney) + ": " + (TestSuccesMoney + basicMoney), false, true);

							character.Stat.Money += TestSuccesMoney;
							break;
						}
					}
				}
				else
				{
					yield return EventManager.Instance.WaitUntilFinish();
				}
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);
			
			
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

			yield return GameManager.Instance.uiEventManager.OnChanged(event_start_script);

			yield return base.Activate(character);

			// 1. 집중 단속 시 공안 단속
			if (CrackDown.Instance.IsCrackDown)
			{
				yield return inspectAction.Activate(character);

				// 공안 단속 후 구금되면 더 이상 이동하지 않음.
				if (character.IsDetention)
				{
					yield break;
				}
			}

			// 2. 스페셜 이벤트 확인
			if (character.CheckSpecialEvent())
			{
				bool dontMove = false;

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

				if (dontMove)
				{
					yield break;
				}
			}

			// 3. 이동 및 중간 경로 검문

			List<City> path = CityManager.Instance.CalcPath(character.CurCity, character.NextCity, actionName);

			if (path == null)
			{
				yield break;
			}
			else if (path.Count == 0)
			{
				yield break;
			}

			// 일반 이동
			float moveTimePerCity = TimeTable.Instance.MoveTimePerAction;

			// 버스 이동
			if (actionName == eEventAction.MOVE_BUS)
			{
				character.Stat.Money -= 4;
				// 도시별 이동 시간 = 기본 이동 시간 / 이동하는 도시 개수
				moveTimePerCity = TimeTable.Instance.MoveTimePerAction / path.Count;
				// 얼마나 이동하든 ap는 1만 사용
				character.AP++;
			}

			foreach (City city in path)
			{
				if (actionName == eEventAction.MOVE)
				{
					character.AP++;
					moveTimePerCity = TimeTable.Instance.MoveTimePerAction;
					// 산이 껴 있으면 ap와 moveTime을 두 배로 사용.
					if (character.CurCity.Type == eNodeType.MOUNTAIN || city.Type == eNodeType.MOUNTAIN)
					{
						character.AP++;
						moveTimePerCity += TimeTable.Instance.MoveTimePerAction;
					}
				}
				yield return character.MoveTo(city, moveTimePerCity);

				// 집중 단속 기간이면 공안 단속 들어감
				if (CrackDown.Instance.IsCrackDown)
				{
					yield return inspectAction.Activate(character);

					// 공안 단속 후 구금되면 더 이상 이동하지 않음.
					if (character.IsDetention)
					{
						yield break;
					}
				}
			}
			
			yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_End_Move));
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

			character.AP += requiredTime;

			yield return GameManager.Instance.uiQuestManager.Activate();

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
			TipManager.Instance.Show(eTipTiming.Inspect);
			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			List<Piece> policesInThisCity = PieceManager.Instance.FindAll(eSubjectType.POLICE).FindAll(x=>x.City == character.CurCity);
			Debug.LogWarning("policesInThisCity.Count : " + policesInThisCity.Count);
			for (int i = 0; i < policesInThisCity.Count; ++i)
			{
				if (character.IsDetention)
					break;

				actionName = eEventAction.INSPECT;

				Police police = policesInThisCity[i] as Police;
				yield return police.Fight(actionName, character);

				if (EventManager.Instance.TestResult == false)
				{
					yield return character.Arrested(police);
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

			bool isLastCity = (character.CurCity.Name == "DANDONG" || character.CurCity.Name == "TUMEN");

			/* 밤단계 때 
			 * 단둥 또는 투먼시에 도착하지 않았으면 공안체크로 탈출 시도 이벤트
			 * 단둥 또는 투먼시에 도착하면 밤단계에 수용소이벤트 출력
			 * 수용소 이벤트는 그냥 호출
			 * - 수용소이벤트 실패 시 북송 게임오버
			 * */
			if (GameManager.Instance.State == GameManager.GameState.Night)
			{
				if (isLastCity)
				{
					TipManager.Instance.Show(eTipTiming.Camp);
					AudioManager.Instance.ChangeBGM("Camp");
					actionName = eEventAction.CAMP;

					yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Start_Camp));
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
			// ActState : 체포상태에서 체포된 공안의 이동력만큼 이동
			else if (GameManager.Instance.State == GameManager.GameState.Detention)
			{
				// 마지막 도시에 도착하면 밤에 수용소 이벤트를 하고 실패시 그 다음날 아침에 북송 엔딩
				if (isLastCity)
				{
					yield return GameManager.Instance.endingManager.StartEnding(eEnding.REPATRIATE);
				}
				else
				{
					yield return character.HaulIn();
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
			yield return BuffManager.Instance.ActivateEffectByStartTime(startTime, character);

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

			TipManager.Instance.Show(eTipTiming.Shop);

			GameManager.Instance.shopUIObj.SetActive(true);

			yield return EventManager.Instance.WaitUntilFinish();
		}
	}

	public class Investigation : Action
	{
		public Investigation()
		{
			startTime = eStartTime.INVESTIGATION;
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
			yield return GameManager.Instance.uiEventManager.OnChanged(event_start_script);

			yield return base.Activate(character);

			character.AP += requiredTime;

			// 도시크기 별 주사위 추가 - 중도시 주사위1, 대도시는 주사위2
			//if(character.CurCity.Type == eNodeType.MIDDLECITY)
			//{
			//	character.Stat.TempDiceNum += 1;
			//}
			//else if (character.CurCity.Type == eNodeType.BIGCITY)
			//{
			//	character.Stat.TempDiceNum += 2;
			//}

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
				yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

				Event selectedEvent = EventManager.Instance.Find(actionName);
				if (selectedEvent == null)
				{
					Debug.LogError("selectedEvent is null");
					yield break;
				}
				
				yield return GameManager.Instance.uiEventManager.OnChanged(selectedEvent.Script);
				
				// deal with result
				yield return EventManager.Instance.CalculateTestResult(selectedEvent.Result.TestStat, character);

				yield return EventManager.Instance.TreatResult(selectedEvent.Result, character, true, false);

				int testSuccessNum = EventManager.Instance.TestSuccessNum;
				character.Stat.TempDiceNum = 0;

				ArrayList list = new ArrayList(3);
				ArrayList finalList = new ArrayList();

				// 브로커 조사 : 정보 퀘스트, 정보
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
				//공안 조사 : decrease short-term gauge
				else if (ActionName == eEventAction.INVESTIGATION_POLICE)
				{
					// 하루 시야 증가, 클릭한 도시의 공안 수, 집중단속확률 중 1, 2, 3 (중복 없음)
					//AbnormalCondition addViewRange = AbnormalConditionManager.Instance.Find("Add View Range");
					//EffectAmount revealPosition = new EffectAmount(EffectManager.Instance.Find(eSubjectType.POLICE, eVerbType.REVEAL, eObjectType.NUMBER), 1);
					//EffectAmount getProbability = new EffectAmount(EffectManager.Instance.Find(eSubjectType.POLICE, eVerbType.REVEAL, eObjectType.CRACKDOWN_GAUGE), 1);
					int amount = testSuccessNum;
					// 최대 3개 감소
					if (amount > 3)
					{
						amount = 3;
					}
					EffectAmount decreaseGauge = new EffectAmount(EffectManager.Instance.Find(eSubjectType.POLICE, eVerbType.DEL, eObjectType.SHORT_TERM_GAUGE), amount);

					finalList.Add(decreaseGauge);
				}
				//식량아이템 얻기 : 차는 양이 하나 또는 둘 또는 셋
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
						resultEffectScript += abnormalCondition.Name + "\n";
					}
					else if(item is Item)
					{
						Item addingItem = item as Item;
						resultEffectScript += addingItem.Name + "\n";
					}
				}
				yield return GameManager.Instance.uiEventManager.OnChanged("\n" + resultEffectScript, false, true);

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
						character.Inven.AddItem(addingItem);
					}
				}
			}
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			
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
			//NGUIDebug.Log("BrokerAction action");

			yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Start_Broker));

			yield return base.Activate(character);

			character.AP += requiredTime;

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.RAND);

			yield return EventManager.Instance.DoCommand(actionName, character);

			yield return TimeTable.Instance.SpendRemainTime();

			if (EventManager.Instance.TestResult)
			{
				PieceManager.Instance.Delete(PieceManager.Instance.Find(eSubjectType.BROKER, character.CurCity));
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			
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
			//NGUIDebug.Log("AbilityAction action");

			yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Start_Ability));

			yield return base.Activate(character);

			character.AP += requiredTime;

			yield return TimeTable.Instance.SpendTime(requiredTime, eSpendTime.END);

			yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[character.EventIndex], character);

			yield return BuffManager.Instance.DeactivateEffectByStartTime(startTime, character);

			
		}
	}
}