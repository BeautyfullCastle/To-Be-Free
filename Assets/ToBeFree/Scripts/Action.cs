using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public enum eCommand
    {
        MOVE, WORK, REST, SHOP, QUEST, INFO, BROKER, ESCAPE
    }

    public class Action
    {
        protected eStartTime startTime;
        protected eEventAction actionName;

        public delegate void ActionEventHandler(eStartTime startTime, Character character);
        static public event ActionEventHandler ActionEventNotify;

        public virtual IEnumerator Activate(Character character)
        {
            if(ActionEventNotify != null)
                ActionEventNotify(startTime, character);
            
            BuffManager.Instance.CheckStartTimeAndActivate(startTime, character);

            yield return null;
        }
    }

    public class Rest : Action
    {
        public delegate bool CureEventHandler(Character character);
        static public event CureEventHandler CureEventNotify;

        public Rest()
        {
            startTime = eStartTime.REST;
            actionName = eEventAction.NULL;
        }

        public override IEnumerator Activate(Character character)
        {
            Debug.Log("Rest Action Activated.");

            base.Activate(character);

            Debug.Log("Cure for Rest");
            character.Rest();

            yield return null;

            CureEventNotify(character);
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
            base.Activate(character);
            yield return (EventManager.Instance.DoCommand(actionName, character));
            // if effect is money and event is succeeded,
            EffectAmount[] successResulteffects = EventManager.Instance.ResultSuccessEffectAmountList;

            for (int i = 0; i < successResulteffects.Length; ++i)
            {
                if(successResulteffects[i].Effect == null)
                {
                    continue;
                }
                if (successResulteffects[i].Effect.SubjectType == eSubjectType.MONEY)
                {
                    character.Stat.Money += character.CurCity.CalcRandWorkingMoney();
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
            actionName = eEventAction.MOVE;
        }

        public override IEnumerator Activate(Character character)
        {
            Debug.LogWarning("Move action Activated.");
            base.Activate(character);
            yield return (EventManager.Instance.DoCommand(actionName, character));

            character.MoveTo(character.NextCity);
            Debug.LogWarning("character is moved to " + character.CurCity.Name);
        }
    }

    public class QuestAction : Action
    {
        public QuestAction()
        {
            startTime = eStartTime.QUEST;
            actionName = eEventAction.QUEST;
        }

        public override IEnumerator Activate(Character character)
        {
            Debug.LogWarning("Quest action Activated.");
            
            BuffManager.Instance.CheckStartTimeAndActivate(startTime, character);

            List<Piece> quests = PieceManager.Instance.FindAll(eSubjectType.QUEST);
            QuestPiece questPiece = quests.Find(x => x.City == character.CurCity) as QuestPiece;

            Quest quest = questPiece.CurQuest;
            if (quest.CheckCondition(character))
            {
                QuestManager.Instance.ActivateQuest(quest, true, character);
                PieceManager.Instance.Delete(questPiece);
            }

            yield return (EventManager.Instance.WaitUntilFinish());

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
            
            BuffManager.Instance.CheckStartTimeAndActivate(startTime, character);

            List<Piece> policesInThisCity = PieceManager.Instance.FindAll(eSubjectType.POLICE).FindAll(x=>x.City == character.CurCity);
            Debug.LogWarning("policesInThisCity.Count : " + policesInThisCity.Count);
            for (int i = 0; i < policesInThisCity.Count; ++i)
            {
                yield return EventManager.Instance.DoCommand(eEventAction.INSPECT, character);
            }
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
            Debug.LogWarning("Detention action activated.");

            BuffManager.Instance.CheckStartTimeAndActivate(startTime, character);

            bool testResult = DiceTester.Instance.Test(character.Stat.Agility, character);
            if(testResult == true)
            {
                yield return AbnormalConditionManager.Instance.Find("Detention").DeActivate(character);
            }
            else
            {
                yield return character.HaulIn();
            }
        }
    }

    public class EnterToShop : Action
    {
        public EnterToShop()
        {
        }

        public override IEnumerator Activate(Character character)
        {
            Debug.LogWarning("Enter to Shop action activated.");

            NGUIDebug.Log("Enter To Shop action");
            GameManager.Instance.shopUIObj.SetActive(true);
            yield return EventManager.Instance.WaitUntilFinish();
            
        }
    }
}