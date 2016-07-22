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

        public EventManager()
        {
            file = Application.streamingAssetsPath + "/Event.json";
            DataList<EventData> cDataList = new DataList<EventData>(file);
            //EventDataList cDataList = new EventDataList(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Event[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach (EventData data in dataList)
            {
                Event curEvent = new Event(EnumConvert<eEventAction>.ToEnum(data.actionType), data.region,
                                            EnumConvert<eTestStat>.ToEnum(data.stat), EnumConvert<eDifficulty>.ToEnum(data.difficulty),
                                            data.script, data.resultIndex, data.selectIndexList);

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

        public bool CalculateTestResult(eTestStat testStat, Character character)
        {
            if (testStat == eTestStat.ALL || testStat == eTestStat.NULL)
            {
                TestResult = true;
                UIChanged(eUIEventLabelType.DICENUM, TestResult.ToString());
            }
            else
            {
                TestResult = DiceTester.Instance.Test(character.GetDiceNum(testStat)) > 0;
                UIChanged(eUIEventLabelType.DICENUM, TestResult.ToString() + " : " + EnumConvert<eTestStat>.ToString(testStat));
            }
            return TestResult;
        }

        public IEnumerator TreatResult(Result result, bool testResult, Character character)
        {
            string resultScript = string.Empty;
            string resultEffect = string.Empty;
            if (testResult == true)
            {
                resultScript = result.Success.Script;
                for (int i = 0; i < result.Success.EffectAmounts.Length; ++i)
                {
                    EffectAmount effectAmount = result.Success.EffectAmounts[i];
                    if (effectAmount.Effect == null)
                    {
                        continue;
                    }
                    yield return effectAmount.Activate(character);
                    resultEffect += effectAmount.ToString() + "\n";
                }
            }
            else
            {
                resultScript = result.Failure.Script;
                for (int i = 0; i < result.Failure.EffectAmounts.Length; ++i)
                {
                    EffectAmount effectAmount = result.Failure.EffectAmounts[i];
                    if (effectAmount.Effect == null)
                    {
                        continue;
                    }
                    yield return effectAmount.Activate(character);
                    resultEffect += effectAmount.ToString() + "\n";
                }
            }
            this.CurrResult = result;
            UIChanged(eUIEventLabelType.RESULT, resultScript);
            UIChanged(eUIEventLabelType.RESULT_EFFECT, resultEffect);
        }
    
        public IEnumerator DoCommand(eEventAction actionType, Character character)
        {
            yield return GameManager.Instance.ShowStateLabel(actionType.ToString() + " command activated.", 0.5f);

            selectedEvent = Find(actionType, character.CurCity);
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
            isFinish = false;
            while (isFinish == false)
            {
                yield return new WaitForSeconds(.1f);
            }
        }

        public void OnClickOK()
        {
            isFinish = true;
        }

        public Event Find(eEventAction actionType, City city)
        {
            // should check here again.
            List<Event> findedEvents = SelectEventsByAction(actionType);

            // <eRegion, List<Event>>
            Dictionary<int, List<Event>> eventListPerRegionDic = InitEventListPerRegionDic(findedEvents, city);
            if (eventListPerRegionDic.Count == 0)
            {
                Debug.LogError("eventListPerRegionDic.Count == 0");
                return null;
            }
            List<Event> regionEvents = SelectRandomEventsByProb(eventListPerRegionDic, actionType, eProbType.REGION);
            if(regionEvents == null)
            {
                regionEvents = findedEvents;
            }
            else if (regionEvents.Count == 0)
            {
                Debug.LogError("regionEvents.Count == 0");
                return null;
            }
            List<Event> statEvents = null;
            if (actionType == eEventAction.GLOBAL || actionType == eEventAction.QUEST)
            {
                statEvents = regionEvents;
            }
            else
            {
                Dictionary<int, List<Event>> eventListPerStatDic = InitEventListPerStatDic(regionEvents);
                if (eventListPerStatDic.Count == 0)
                {
                    Debug.LogError("eventListPerStatDic.Count == 0");
                    return null;
                }
                statEvents = SelectRandomEventsByProb(eventListPerStatDic, actionType, eProbType.STAT);
                if(statEvents == null)
                {
                    statEvents = regionEvents;
                }
            }

            System.Random r = new System.Random();
            int randVal = r.Next(0, statEvents.Count - 1);

            return statEvents[randVal];
        }

        public IEnumerator ActivateEvent(Event currEvent, Character character)
        {
            GameManager.Instance.OpenEventUI();

            Debug.Log(currEvent.ActionType + " " + currEvent.Region + " " + currEvent.TestStat + " is activated.");

            UIChanged(eUIEventLabelType.EVENT, currEvent.Script);
            
            // deal with select part
            if(currEvent.Result == null)
            {
                Select[] selectList = new Select[currEvent.SelectIndexList.Length];
                for (int i = 0; i < currEvent.SelectIndexList.Length; ++i)
                {
                    Select select = SelectManager.Instance.List[currEvent.SelectIndexList[i]];
                    selectList[i] = select;
                }
                SelectUIChanged(selectList);
            }
            // deal with result
            else
            {
                yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, character);
                this.TestResult = CalculateTestResult(currEvent.Result.TestStat, character);
                yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, character);

                yield return TreatResult(currEvent.Result, TestResult, character);
            }

            yield return WaitUntilFinish();
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
                //else if(resultEffects[i].AbnormalCondition != null)
                //{
                //    resultEffects[i].AbnormalCondition.Activate(character, resultEffects[i].Value);
                //}
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
                if (elem.ActionType != actionType || elem.Region == "NULL" || elem.TestStat == eTestStat.NULL)
                {
                    continue;
                }
                findedEvents.Add(elem);
            }
            if (findedEvents.Count == 0)
            {
                Debug.LogError("Events for " + actionType + " are not exist.");
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

        private Dictionary<int, List<Event>> InitEventListPerRegionDic(List<Event> regionEvents, City city)
        {
            Dictionary<int, List<Event>> eventListPerRegionDic = new Dictionary<int, List<Event>>()
            {
                {0, new List<Event>() }, {1, new List<Event>() }, {2, new List<Event>() }
            };

            foreach (Event eventElem in regionEvents)
            {
                if (eventElem == null)
                {
                    Debug.LogError("eventElem can't find.");
                    continue;
                }

                if (eventElem.Region == EnumConvert<eCity>.ToString(city.Name))
                {
                    eventListPerRegionDic[(int)eRegion.CITY].Add(eventElem);
                }
                else if (eventElem.Region == EnumConvert<eArea>.ToString(city.Area))
                {
                    eventListPerRegionDic[(int)eRegion.AREA].Add(eventElem);
                }
                else if (eventElem.Region == "ALL")
                {
                    eventListPerRegionDic[(int)eRegion.ALL].Add(eventElem);
                }
            }
            return eventListPerRegionDic;
        }

        private Dictionary<int, List<Event>> InitEventListPerStatDic(List<Event> statEvents)
        {
            Dictionary<int, List<Event>> eventListPerStatDic = new Dictionary<int, List<Event>>()
            {
                {0, new List<Event>() }, {1, new List<Event>() }, {2, new List<Event>() },
                { 3, new List<Event>() }, {4, new List<Event>() }, {5, new List<Event>() }, {6, new List<Event>() }
            };

            foreach (Event eventElem in statEvents)
            {
                if (eventElem == null)
                {
                    Debug.LogError("eventElem can't find.");
                    continue;
                }
                if(eventElem.TestStat == eTestStat.NULL)
                {
                    continue;
                }
                int iStat = (int)eventElem.TestStat;
                eventListPerStatDic[iStat].Add(eventElem);
            }
            return eventListPerStatDic;
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
            private set
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
    }
}