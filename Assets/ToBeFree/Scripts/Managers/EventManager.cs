using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eProbType
	{
		REGION, STAT
	}

	public class EventManager : Singleton<EventManager>
	{
		private Event[] list;
		private EventData[] dataList;
		private string file;

		private Language.EventData[] engList;
		private Language.EventData[] korList;
		private List<Language.EventData[]> languageList;

		public delegate void UIChangedHandler(eUIEventLabelType type, string text);
		public static event UIChangedHandler UIChanged = delegate { };

		public delegate void SelectUIChangedHandler(Select[] select);
		public static event SelectUIChangedHandler SelectUIChanged = delegate { };

		public delegate void UIOpenHandler();
		public static UIOpenHandler UIOpen = delegate { };
		private bool isFinish;
		private Event selectedEvent;

		private bool testResult;
		private Result currResult;
		private int testSuccessNum;

		public void Init()
		{
			file = Application.streamingAssetsPath + "/Event.json";
			DataList<EventData> cDataList = new DataList<EventData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Event[dataList.Length];

			engList = new DataList<Language.EventData>(Application.streamingAssetsPath + "/Language/English/Event.json").dataList;
			korList = new DataList<Language.EventData>(Application.streamingAssetsPath + "/Language/Korean/Event.json").dataList;
			languageList = new List<Language.EventData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			foreach (EventData data in dataList)
			{
				Event curEvent = new Event(EnumConvert<eEventAction>.ToEnum(data.actionType), 
					EnumConvert<eDifficulty>.ToEnum(data.difficulty), data.script, data.resultIndex, data.selectIndexList);

				if (list[data.index] != null)
				{
					Debug.LogError("EventManager : data.index is duplicated.");
				}

				if (list[data.index] != null)
				{
					throw new Exception("Event data.index " + data.index + " is duplicated.");
				}

				list[data.index] = curEvent;
			}
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.EventData data in languageList[(int)language])
			{
				try
				{
					list[data.index].Script = data.script;
				} catch(Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
				
			}
		}

		public IEnumerator CalculateTestResult(eTestStat testStat, Character character)
		{
			if (testStat == eTestStat.ALL || testStat == eTestStat.NULL)
			{
				TestResult = true;
				UIChanged(eUIEventLabelType.DICENUM, TestResult.ToString());
			}
			else
			{
				UIChanged(eUIEventLabelType.RESULT, "Let's Roll the dice for the test.");
				yield return EventManager.Instance.WaitUntilFinish();

				GameManager.Instance.OpenEventUI();
				UIChanged(eUIEventLabelType.EVENT, "Dice Test Result");
				yield return DiceTester.Instance.Test(character.GetDiceNum(testStat), (x) => testSuccessNum = x);
				TestResult = testSuccessNum > 0;
				UIChanged(eUIEventLabelType.DICENUM, "Succeeded Dice Num : " + testSuccessNum.ToString() + " : " + EnumConvert<eTestStat>.ToString(testStat));
			}
			yield return null;
		}

		public IEnumerator TreatResult(Result result, Character character)
		{
			string resultScript = string.Empty;
			string resultEffect = string.Empty;
			ResultScriptAndEffects resultScriptAndEffects = null;
			if (testResult == true)
			{
				resultScriptAndEffects = result.Success;
			}
			else
			{
				resultScriptAndEffects = result.Failure;
			}

			resultScript = resultScriptAndEffects.Script;
			for (int i = 0; i < resultScriptAndEffects.EffectAmounts.Length; ++i)
			{
				EffectAmount effectAmount = resultScriptAndEffects.EffectAmounts[i];
				if (effectAmount.Effect == null)
				{
					continue;
				}
				resultEffect += effectAmount.ToString() + "\n";
			}

			this.CurrResult = result;
			UIChanged(eUIEventLabelType.RESULT, resultScript);
			UIChanged(eUIEventLabelType.RESULT_EFFECT, resultEffect);

			yield return WaitUntilFinish();

			for (int i = 0; i < resultScriptAndEffects.EffectAmounts.Length; ++i)
			{
				EffectAmount effectAmount = resultScriptAndEffects.EffectAmounts[i];
				if (effectAmount.Effect == null)
				{
					continue;
				}
				yield return effectAmount.Activate(character);
			}
		}
	
		public IEnumerator DoCommand(eEventAction actionType, Character character)
		{
			yield return GameManager.Instance.ShowStateLabel(actionType.ToString() + " command activated.", 0.5f);

			selectedEvent = Find(actionType);
			if (selectedEvent == null)
			{
				Debug.LogError("selectedEvent is null");
				yield break;
			}

			yield return ActivateEvent(selectedEvent, character);
		   
			Debug.Log("DoCommand Finished.");
		}

		public IEnumerator WaitUntilFinish()
		{
			GameManager.Instance.uiEventManager.okButton.isEnabled = true;
			isFinish = false;
			while (isFinish == false)
			{
				yield return new WaitForSeconds(.1f);
			}
		}

		public void OnClickOK()
		{
			if (IsFinish == false)
			{
				GameManager.Instance.uiEventManager.okButton.isEnabled = false;
				isFinish = true;
			}
		}

		public Event Find(eEventAction actionType)
		{
			/*
			 * 이벤트는 랜덤으로 발생
			하루에 이벤트발생 가능성에 확률을 추가한다.
			발생조건 있음(일을 할때 라던지)
			이벤트는 하루에 최대 하나만 나옴
			*/
			
			List<Event> findedEvents = SelectEventsByAction(actionType);
			if(findedEvents == null)
			{
				return null;
			}

			System.Random r = new System.Random();
			int randVal = r.Next(0, findedEvents.Count - 1);
			
			return findedEvents[randVal];
		}

		public IEnumerator ActivateEvent(Event currEvent, Character character)
		{
			if(currEvent == null)
			{
				Debug.LogError("ActivateEvent : currEvent is null!");
				yield break;
			}

			GameManager.Instance.OpenEventUI();

			Debug.Log(currEvent.ActionType + " is activated.");

			UIChanged(eUIEventLabelType.EVENT, currEvent.Script);
			
			// deal with select part
			if(currEvent.HasSelect)
			{
				Select[] selectList = new Select[currEvent.SelectIndexList.Length];
				for (int i = 0; i < currEvent.SelectIndexList.Length; ++i)
				{
					Select select = SelectManager.Instance.List[currEvent.SelectIndexList[i]];
					selectList[i] = select;
				}
				SelectUIChanged(selectList);
				yield return SelectManager.Instance.WaitForSelect();
			}
			// deal with result
			else
			{
				yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
				yield return CalculateTestResult(currEvent.Result.TestStat, character);
				yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

				yield return TreatResult(currEvent.Result, character);
			}
		}

		public void ActivateResultEffects(EffectAmount[] resultEffects, Character character)
		{
			if (resultEffects == null)
			{
				Debug.LogError("resultEffects null");
				return;
			}

			for (int i = 0; i < resultEffects.Length; ++i)
			{
				if (resultEffects[i].Effect != null)
				{
					resultEffects[i].Effect.Activate(character, resultEffects[i].Amount);
				}
			}
		}

		private List<Event> SelectEventsByAction(eEventAction actionType)
		{
			if(actionType == eEventAction.NULL)
			{
				return null;
			}

			List<Event> findedEvents = new List<Event>();
			foreach (Event elem in list)
			{
				if( (elem.ActionType & actionType) == actionType)
				{
					findedEvents.Add(elem);
				}
			}
			if (findedEvents.Count == 0)
			{
				Debug.LogError("Events for " + EnumConvert<eEventAction>.ToString(actionType) + " are not exist.");
				return null;
			}
			return findedEvents;
		}

		private List<Event> SelectRandomEventsByProb(Dictionary<int, List<Event>> eventListDic, eEventAction actionType, eProbType probType)
		{
			Probability prob = null;
			if (probType == eProbType.STAT)
			{
				StatProbability statProb = StatProbabilityManager.Instance.FindProbByAction(actionType);
				if(statProb == null)
				{
					return null;
				}
				prob = (Probability)statProb.DeepCopy();
			}
			else if(probType == eProbType.REGION)
			{
				RegionProbability regionProb = RegionProbabilityManager.Instance.Prob;
				if(regionProb == null)
				{
					return null;
				}
				prob = (Probability)RegionProbabilityManager.Instance.Prob.DeepCopy();
			}
			prob.ResetProbValues(eventListDic);
			return new List<Event>(SelectRandomEvents(prob, eventListDic));
		}
		

		private List<Event> SelectRandomEvents(Probability prob, Dictionary<int, List<Event>> dic)
		{
			if (prob == null)
			{
				Debug.LogError("prob is null.");
				return null;
			}

			int totalProbVal = prob.CheckAddedAllProbValues();
			if (totalProbVal == 0)
			{
				Debug.LogError("Total prob value is 0");
				return null;
			}
			System.Random r = new System.Random();
			int randVal = r.Next(1, totalProbVal);

			List<Event> eventList;
			int val = 0;
			foreach (int key in dic.Keys)
			{
				val += prob.DataList[key];
				if (randVal < val)
				{
					eventList = dic[key];
					return eventList;
				}
			}
			Debug.LogError("Can't find event list. rand Val : + " + randVal + " , total val : " + val);
			return null;
		}

		public Event[] List
		{
			get
			{
				return list;
			}
		}

		public Event SelectedEvent
		{
			get
			{
				return selectedEvent;
			}
		}

		public bool IsFinish
		{
			get
			{
				return isFinish;
			}

			set
			{
				isFinish = value;
			}
		}

		public bool TestResult
		{
			get
			{
				return testResult;
			}
			set
			{
				testResult = value;
			}
		}

		public Result CurrResult
		{
			get
			{
				return currResult;
			}

			private set
			{
				currResult = value;
			}
		}

		public int TestSuccessNum
		{
			get
			{
				return testSuccessNum;
			}
		}
	}
}
 