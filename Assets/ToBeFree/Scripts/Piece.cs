using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Piece
    {
        protected City city;
        protected eSubjectType subjectType;

        public Piece(City city, eSubjectType subjectType)
        {
            this.city = city;
            this.subjectType = subjectType;
        }

        public City City
        {
            get
            {
                return city;
            }
        }

        public eSubjectType SubjectType
        {
            get
            {
                return subjectType;
            }
        }

        public IEnumerator MoveCity(City city)
        {
            yield return GameManager.Instance.MoveDirectingCam(
                GameManager.Instance.FindGameObject(this.City.Name.ToString()).transform.position,
                GameManager.Instance.FindGameObject(city.Name.ToString()).transform.position, 5f);

            this.city = city;            
        }

        public virtual bool CheckDuration()
        {
            return false;
        }
    }

    public class Police : Piece
    {
        public Police(City city, eSubjectType subjectType) : base(city, subjectType)
        {
        }
    }

    public class Information : Piece
    {
        public Information(City city, eSubjectType subjectType) : base(city, subjectType)
        {
        }
    }

    public class Broker : Piece
    {
        public Broker(City city, eSubjectType subjectType) : base(city, subjectType)
        {
        }
    }

    public class QuestPiece : Piece
    {
        private Quest quest;
        private Character character;

        private int pastWeeks;

        public delegate void AddQuestHandler(QuestPiece piece);
        public static event AddQuestHandler AddQuest;

        public QuestPiece(Quest quest, Character character, City city, eSubjectType subjectType) : base(city, subjectType)
        {
            this.quest = quest;
            this.character = character;
            
            //TimeTable.Instance.NotifyEveryWeek += WeekIsGone;

            AddQuest(this);
        }

        public void WeekIsGone()
        {
            pastWeeks++;
        }

        public IEnumerator TreatPastQuests(Character character)
        {
            if(CheckDuration())
            {
                GameManager.Instance.OpenEventUI();

                GameManager.FindObjectOfType<UIEventManager>().OnChanged(eUIEventLabelType.RESULT, CurQuest.FailureEffects.Script);

                string effectScript = string.Empty;
                foreach(EffectAmount effectAmount in CurQuest.FailureEffects.EffectAmounts)
                {
                    effectScript += effectAmount.ToString();
                }
                GameManager.FindObjectOfType<UIEventManager>().OnChanged(eUIEventLabelType.RESULT_EFFECT, effectScript);

                yield return EventManager.Instance.WaitUntilFinish();

                GameManager.FindObjectOfType<UIQuestManager>().DeleteQuest(this.CurQuest);

                yield return null;
            }
        }

        public override bool CheckDuration()
        {
            return pastWeeks >= quest.Duration;
        }

        public Quest CurQuest
        {
            get
            {
                return quest;
            }
        }

        public int PastWeeks
        {
            get
            {
                return pastWeeks;
            }
        }
    }
}