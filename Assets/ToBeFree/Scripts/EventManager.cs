using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class EventManager : Singleton<EventManager>
    {
        private List<Event> everyEvents;
        private ResultEffect[] resultEffects;

        public EventManager()
        {
            everyEvents = new List<Event>();
        }

        public Event DoCommand(string actionType, Character character)
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

        public Event Find(string actionType, City city)
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
            if (actionType == "Global" || actionType == "Quest")
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
            Debug.Log(currEvent.ActionType + " " + currEvent.Region + " " + currEvent.Stat + " is activated.");

            Result result = currEvent.Result;

            if (currEvent.ActionType == "Global")
            {
                resultEffects = currEvent.Result.Success.Effects;
            }
            if (currEvent.ActionType == "Quest" && currEvent.BSelect)
            {
                if (currEvent.SelectList[0].CheckCondition(character))
                {
                    result = currEvent.SelectList[0].Result;
                    resultEffects = currEvent.SelectList[0].Result.Success.Effects;
                }
                else
                {
                    Debug.LogError("Quest's Checkcondition is failed.");
                    return false;
                }
            }

            // dice test
            if (!string.IsNullOrEmpty(result.TestStat))
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

        public void ActivateResultEffects(ResultEffect[] resultEffects, Character character)
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
                    resultEffects[i].Effect.Activate(character, resultEffects[i].Value);
                }
                else if(resultEffects[i].AbnormalCondition != null)
                {
                    resultEffects[i].AbnormalCondition.Activate(character, resultEffects[i].Value);
                }
            }
        }

        private List<Event> SelectEventsByAction(string actionType)
        {
            List<Event> findedEvents = new List<Event>();
            foreach (Event elem in everyEvents)
            {
                if (!elem.ActionType.Contains(actionType))
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

        private List<Event> SelectRandomEventsByProb(Dictionary<int, List<Event>> eventListDic, string actionType, string probType)
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

                if (eventElem.Region == city.Name)
                {
                    eventListPerRegionDic[(int)eRegion.CITY].Add(eventElem);
                }
                else if (eventElem.Region == city.Area)
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
                int iStat = ProbabilityManager.Instance.ConvertToIndex(eventElem.Stat);
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

        public List<Event> EveryEvents
        {
            get
            {
                return everyEvents;
            }

            set
            {
                everyEvents = value;
            }
        }

        public ResultEffect[] ResultEffects
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
        
    }
}