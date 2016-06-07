﻿using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Action
    {
        protected eStartTime startTime;
        protected string actionName;

        public delegate void ActionEventHandler(eStartTime startTime, Character character);
        static public event ActionEventHandler ActionEventNotify;

        public virtual void Activate(Character character)
        {
            if(ActionEventNotify != null)
                ActionEventNotify(startTime, character);
            
            BuffList.Instance.CheckStartTimeAndActivate(startTime, character);

            if (!string.IsNullOrEmpty(actionName))
            {
                Event selectedEvent = EventManager.Instance.DoCommand(actionName, character);
            }
            
        }
    }

    public class Rest : Action
    {
        public delegate bool CureEventHandler(Character character);
        static public event CureEventHandler CureEventNotify;

        public Rest()
        {
            startTime = eStartTime.REST;
            actionName = string.Empty;
        }

        public override void Activate(Character character)
        {
            Debug.Log("Rest Action Activated.");

            base.Activate(character);

            Debug.Log("Cure for Rest");
            character.HP++;
            character.MENTAL++;

            CureEventNotify(character);
        }
    }

    public class Work : Action
    {
        public Work()
        {
            startTime = eStartTime.WORK;
            actionName = "Work";
        }

        public override void Activate(Character character)
        {
            Debug.LogWarning("Work action activated.");
            base.Activate(character);

            // if effect is money and event is succeeded,
            ResultEffect[] successResulteffects = EventManager.Instance.ResultEffects;

            for (int i = 0; i < successResulteffects.Length; ++i)
            {
                if (successResulteffects[i].Effect.BigType == "MONEY")
                {
                    character.CurMoney += character.CurCity.CalcRandWorkingMoney();
                    break;
                }
            }

            
        }
    }

    public class Move : Action
    {
        public Move()
        {
            startTime = eStartTime.MOVE;
            actionName = "Move";
        }

        public override void Activate(Character character)
        {
            Debug.LogWarning("Move action Activated.");
            base.Activate(character);

            character.CurCity = CityGraph.Instance.Find("B");
            Debug.LogWarning("character is moved to " + character.CurCity.Name);
        }
    }

    public class QuestAction : Action
    {
        public QuestAction()
        {
            startTime = eStartTime.QUEST;
            actionName = string.Empty;
        }

        public override void Activate(Character character)
        {
            Debug.LogWarning("Quest action Activated.");
            
            BuffList.Instance.CheckStartTimeAndActivate(startTime, character);

            List<Quest> quests = PieceManager.Instance.QuestList.FindAll(x => x.City == character.CurCity);
            if (quests.Count >= 2)
            {
                // can't spawn more than 2 quests in one city.
            }
            else if (quests == null || quests.Count == 0)
            {
                Debug.LogError("No Quest in this city.");
            }
            else
            {
                Quest quest = quests[0];
                if (EventManager.Instance.ActivateEvent(quest.CurEvent, character))
                {
                    PieceManager.Instance.QuestList.Remove(quest);
                }
            }

            Debug.Log("character quest activated.");
        }
    }

    public class Inspect : Action
    {
        public Inspect()
        {
            startTime = eStartTime.INSPECT;
            actionName = string.Empty;
        }

        public override void Activate(Character character)
        {
            Debug.LogWarning("Inpect action activated.");
            
            BuffList.Instance.CheckStartTimeAndActivate(startTime, character);

            foreach (Police police in PieceManager.Instance.PoliceList)
            {
                if (police.City == character.CurCity)
                {
                    character.Inspect();
                }
            }
            
        }
    }
}