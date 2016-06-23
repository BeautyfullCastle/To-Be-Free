using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class EventManager : Singleton<EventManager>
    {
        private EffectAmount[] resultEffects;

        private readonly Event[] list;
        private readonly EventData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Event.json";

        public EventManager()
        {
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

                list[data.index] = curEvent;
            }
        }

        public Event DoCommand(eEventAction actionType, Character character)
        {
            Event selectedEvent = Find(actionType, character.CurCity);
            if (selectedEvent == null)
            {
                Debug.LogError("selectedEvent is null");
                return null;
            }
            ActivateEvent(selectedEvent, character);

            return selectedEvent;
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
            List<Event> regionEvents = SelectRandomEventsByProb(eventListPerRegionDic, actionType, "Region");
            if (regionEvents.Count == 0)
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
                statEvents = SelectRandomEventsByProb(eventListPerStatDic, actionType, "Stat");
            }

            System.Random r = new System.Random();
            int randVal = r.Next(0, statEvents.Count - 1);

            return statEvents[randVal];
        }

        public bool ActivateEvent(Event currEvent, Character character)
        {
            Debug.Log(currEvent.ActionType + " " + currEvent.Region + " " + currEvent.TestStat + " is activated.");

            Result result = currEvent.Result;

            if (currEvent.ActionType == eEventAction.GLOBAL)
            {
                resultEffects = currEvent.Result.Success.Effects;
            }
            if (currEvent.ActionType == eEventAction.QUEST && currEvent.HasSelect)
            {
                Select select = SelectManager.Instance.List[currEvent.SelectIndexList[0]];
                if (select.CheckCondition(character))
                {
                    result = select.Result;
                    resultEffects = select.Result.Success.Effects;
                }
                else
                {
                    Debug.LogError("Quest's Checkcondition is failed.");
                    return false;
                }
            }

            // dice test
            if (result.TestStat != eTestStat.NULL)
            {
                
                int diceNum = character.GetDiceNum(result.TestStat);
                bool isTestSucceed = DiceTester.Instance.Test(diceNum, character);
                //Debug.Log("diceNum : " + diceNum + ", TestItems DiceNum : " + itemsToDeactive.Count);
                

                if (isTestSucceed)
                {
                    Debug.Log("Event stat dice test succeeded. ");
                    resultEffects = currEvent.Result.Success.Effects;
                }
                else
                {
                    Debug.Log("Event stat dice test failed. ");
                    resultEffects = currEvent.Result.Failure.Effects;
                    ActivateResultEffects(resultEffects, character);
                    return false;
                }
            }
            return true;
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
            List<Event> findedEvents = new List<Event>();
            foreach (Event elem in list)
            {
                if (elem.ActionType != actionType)
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

        private List<Event> SelectRandomEventsByProb(Dictionary<int, List<Event>> eventListDic, eEventAction actionType, string probType)
        {
            Probability prob = ProbabilityManager.Instance.FindProbByAction(actionType, probType).DeepCopy();
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
                { 3, new List<Event>() }, {4, new List<Event>() }, {5, new List<Event>() }
            };

            foreach (Event eventElem in statEvents)
            {
                if (eventElem == null)
                {
                    Debug.LogError("eventElem can't find.");
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
        
        public EffectAmount[] ResultEffects
        {
            get
            {
                return resultEffects;
            }

            set
            {
                resultEffects = value;
            }
        }

        public Event[] List
        {
            get
            {
                return list;
            }
        }
    }
}