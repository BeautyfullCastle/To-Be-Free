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
            BuffList.Instance.DoWork(startTime, character);

            Event selectedEvent = EventManager.Instance.DoCommand(actionName, character);

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
            actionName = "Rest";
        }

        public override void Activate(Character character)
        {
            base.Activate(character);

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

            EventManager.Instance.DoCommand("Move", character);
            character.CurCity = CityGraph.Instance.Find("B");
            Debug.Log("character is moved to " + character.CurCity.Name);
        }
    }
}
