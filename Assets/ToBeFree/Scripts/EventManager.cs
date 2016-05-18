using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree {
    public class EventManager : Singleton<EventManager> {
        private List<Event> everyEvents;
        
        public EventManager()
        {
            everyEvents = new List<Event>();
        }

        public Event Find(string actionType, Stat stat, City city) {
            // should check here again.
            List<Event> findedEvents = SelectEventsByAction(actionType);
            // <eRegion, List<Event>>
            Dictionary<int, List<Event>> eventListPerRegionDic = InitEventListPerRegionDic(findedEvents, city);
            List<Event> regionEvents = SelectRandomEventsByProb(eventListPerRegionDic, actionType, "Region");

            Dictionary<int, List<Event>> eventListPerStatDic = InitEventListPerStatDic(regionEvents);
            List<Event> statEvents = SelectRandomEventsByProb(eventListPerStatDic, actionType, "Stat");
            
            System.Random r = new System.Random();
            int randVal = r.Next(0, statEvents.Count-1);
            
            return statEvents[randVal];
        }
        
        public bool ActivateEvent(Event currEvent, Character character) {
            Debug.Log(currEvent.ActionType + " " + currEvent.Region + " " + currEvent.Stat + " is activated.");

            int diceNum = 0;
            diceNum = character.GetDiceNum(currEvent.Result.TestStat);
            
            int minSuccessNum = 4;
            int successDiceNum = 0;
            System.Random r = new System.Random();
            for (int i=0; i<diceNum; ++i)
            {
                if(r.Next(1, 6) >= minSuccessNum)
                {
                    successDiceNum++;
                }
            }
            ResultEffect[] resultEffects = null;
            if (successDiceNum > 0)
            {
                Debug.Log("Event succeeded. " + successDiceNum);
                resultEffects = currEvent.Result.Success.Effects;
                
            }
            else
            {
                Debug.Log("Event failed. " + successDiceNum);
                resultEffects = currEvent.Result.Failure.Effects;
            }
            if(resultEffects == null)
            {
                Debug.LogError("resultEffects null");
                return false;
            }

            for (int i = 0; i < resultEffects.Length; ++i)
            {
                resultEffects[i].Effect.Activate(character, resultEffects[i].Value); // working well. fold and add to fail result.
            }

            return true;
        }

        public void DoCommand(string actionType, Character character)
        {
            Event selectedEvent = Find(actionType, character.Stat, character.CurCity);
            ActivateEvent(selectedEvent, character);
        }

        private List<Event> SelectEventsByAction(string actionType) {
            List<Event> findedEvents = new List<Event>();        
            foreach (Event elem in everyEvents)
            {
                if(elem.ActionType != actionType) {
                    continue;
                }
                findedEvents.Add(elem);
            }
            return findedEvents;
        }
        
        private List<Event> SelectRandomEventsByProb(Dictionary<int, List<Event>> eventListDic, string actionType, string probType)
        {

            Probability prob = ProbabilityManager.Instance.FindProbByAction(actionType, probType);
            prob.ResetProbValues(eventListDic);
            return new List<Event>(SelectRandomEvents(prob, eventListDic));
        }

        private Dictionary<int, List<Event>> InitEventListPerRegionDic(List<Event> regionEvents, City city) {
            
            Dictionary<int, List<Event>> eventListPerRegionDic = new Dictionary<int, List<Event>>()
            {
                {0, new List<Event>() }, {1, new List<Event>() }, {2, new List<Event>() }
            };

            foreach (Event eventElem in regionEvents)
            {
                if(eventElem == null) {
                    Debug.LogError("eventElem can't find.");
                    continue;
                }
                
                if(eventElem.Region == city.Name)
                {
                    eventListPerRegionDic[(int)eRegion.CITY].Add(eventElem);
                }
                else if(eventElem.Region == city.Area)
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

        private List<Event> SelectRandomEvents(Probability prob, Dictionary<int, List<Event>> dic) {
            if(prob == null) {
                Debug.LogError("prob is null.");
                return null;
            }
            
            int totalProbVal = prob.CheckAddedAllProbValues();
            System.Random r = new System.Random();
            int randVal = r.Next(1, totalProbVal);
            
            List<Event> eventList;
            int val = 0;
            foreach(int key in dic.Keys) {
                val += prob.DataList[key];
                if(randVal < val) {
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
    }    
}