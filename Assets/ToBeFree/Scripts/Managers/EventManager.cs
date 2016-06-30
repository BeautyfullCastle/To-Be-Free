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

    public class EventManager : MonoSingleton<EventManager>
    {
        private Event[] list;
        private EventData[] dataList;
        private string file;

        private EffectAmount[] resultEffectAmountList;
        
        public delegate void UIChangedHandler(eUIEventLabelType type, string text);
        public static event UIChangedHandler UIChanged = delegate { };

        public delegate void SelectUIChangedHandler(Select[] select);
        public static event SelectUIChangedHandler SelectUIChanged = delegate { };

        public delegate void UIOpenHandler();
        public static UIOpenHandler UIOpen = delegate { };
        private bool isFinish;
        private Event selectedEvent;

        public void Awake()
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
                list[data.index] = curEvent;
            }
        }

        public void TreatResult(Result result)
        {
            Character character = GameManager.Instance.Character;
            bool testResult;
            if (result.TestStat == eTestStat.ALL || result.TestStat == eTestStat.NULL)
            {
                testResult = true;
                UIChanged(eUIEventLabelType.DICENUM, testResult.ToString());
            }
            else
            {
                testResult = DiceTester.Instance.Test(character.GetDiceNum(result.TestStat), character);
                UIChanged(eUIEventLabelType.DICENUM, testResult.ToString() + " : " + EnumConvert<eTestStat>.ToString(result.TestStat)); 
            }
            string resultScript = string.Empty;
            string resultEffect = string.Empty;
            if (testResult == true)
            {                
                resultScript = result.Success.Script;                
                for (int i = 0; i < result.Success.EffectAmounts.Length; ++i)
                {
                    EffectAmount effectAmount = result.Success.EffectAmounts[i];
                    if(effectAmount.Effect == null)
                    {
                        continue;
                    }
                    effectAmount.Activate(character);
                    resultEffect += effectAmount.ToString() + "\n";
                }
                resultEffectAmountList = result.Success.EffectAmounts;
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
                    effectAmount.Activate(character);
                    resultEffect += effectAmount.ToString() + "\n";
                }
                resultEffectAmountList = result.Failure.EffectAmounts;
            }
            UIChanged(eUIEventLabelType.RESULT, resultScript);
            UIChanged(eUIEventLabelType.RESULT_EFFECT, resultEffect);
        }
        
        public IEnumerator DoCommand(eEventAction actionType, Character character)
        {
            selectedEvent = Find(actionType, character.CurCity);
            if (selectedEvent == null)
            {
                Debug.LogError("selectedEvent is null");
                yield break;
            }
            UIOpen();
            ActivateEvent(selectedEvent, character);
            yield return StartCoroutine(WaitUntilFinish());
        }

        private IEnumerator WaitUntilFinish()
        {
            isFinish = false;
            while (isFinish == false)
            {
                yield return new WaitForSeconds(.1f);
            }
            Debug.Log("Finished.");
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

        public void ActivateEvent(Event currEvent, Character character)
        {
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
                TreatResult(currEvent.Result);
            }

            //Result result = currEvent.Result;

            //if (currEvent.ActionType == eEventAction.GLOBAL)
            //{
            //    resultEffects = currEvent.Result.Success.EffectAmounts;
            //    return true;
            //}
            //if (currEvent.ActionType == eEventAction.QUEST && currEvent.HasSelect)
            //{
            //    Select select = SelectManager.Instance.List[currEvent.SelectIndexList[0]];
            //    if (select.CheckCondition(character))
            //    {
            //        result = select.Result;
            //        resultEffects = select.Result.Success.EffectAmounts;
            //        return true;
            //    }
            //    else
            //    {
            //        Debug.LogError("Quest's Checkcondition is failed.");
            //        return false;
            //    }
            //}

            //// dice test
            //if ((result != null) && (result.TestStat != eTestStat.NULL))
            //{
                
            //    int diceNum = character.GetDiceNum(result.TestStat);
            //    bool isTestSucceed = DiceTester.Instance.Test(diceNum, character);
            //    //Debug.Log("diceNum : " + diceNum + ", TestItems DiceNum : " + itemsToDeactive.Count);
                

            //    if (isTestSucceed)
            //    {
            //        Debug.Log("Event stat dice test succeeded. ");
            //        resultEffects = currEvent.Result.Success.EffectAmounts;
            //    }
            //    else
            //    {
            //        Debug.Log("Event stat dice test failed. ");
            //        resultEffects = currEvent.Result.Failure.EffectAmounts;
            //        ActivateResultEffects(resultEffects, character);
            //        return false;
            //    }
            //}
            //return true;
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
        
        public EffectAmount[] ResultEffectAmountList
        {
            get
            {
                return resultEffectAmountList;
            }
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
    }
}