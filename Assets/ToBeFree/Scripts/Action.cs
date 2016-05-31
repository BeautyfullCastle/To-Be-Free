using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Action // : Singleton<Action>
    {
        protected eStartTime startTime;
        protected string actionName;

        public virtual void Activate(Character character)
        {
            List<Item> itemsToDeactive = character.Inven.CheckItemStartTime(startTime, character);

            // activate abnormal condition's effect in buff list.
            BuffList.Instance.Do(startTime, character);

            if (!string.IsNullOrEmpty(actionName))
            {
                Event selectedEvent = EventManager.Instance.DoCommand(actionName, character);
            }

            foreach(Item item in itemsToDeactive)
            {
                item.DeactiveEffect(character);
            }
        }
    }

    public class Rest : Action
    {
        public Rest()
        {
            startTime = eStartTime.REST;
            actionName = string.Empty;
        }

        public override void Activate(Character character)
        {
            Debug.Log("Rest Action Start");

            base.Activate(character);

            Debug.Log("Cure for Rest");
            character.HP++;
            character.MENTAL++;
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
            
            Debug.Log("character work.");
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
            base.Activate(character);
            
            character.CurCity = CityGraph.Instance.Find("A");
            Debug.Log("character is moved to " + character.CurCity.Name);
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
            List<Item> itemsToDeactive = character.Inven.CheckItemStartTime(startTime, character);

            // activate abnormal condition's effect in buff list.
            BuffList.Instance.Do(startTime, character);
            
            
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

            foreach (Item item in itemsToDeactive)
            {
                item.DeactiveEffect(character);
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
            List<Item> itemsToDeactive = character.Inven.CheckItemStartTime(startTime, character);

            // activate abnormal condition's effect in buff list.
            BuffList.Instance.Do(startTime, character);

            foreach (Police police in PieceManager.Instance.PoliceList)
            {
                if (police.City == character.CurCity)
                {
                    character.Inspect();
                }
            }

            foreach (Item item in itemsToDeactive)
            {
                item.DeactiveEffect(character);
            }

            Debug.Log("character quest activated.");
        }
    }
}
