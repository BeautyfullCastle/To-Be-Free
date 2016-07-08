using System;
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

        public void MoveCity(City city)
        {
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

        public void Disapper()
        {
            Debug.Log("Quest Disappered.");
            QuestManager.Instance.ActivateResultEffects(this.quest.Event_.Result.Failure.EffectAmounts, this.character);
        }

        public void WeekIsGone()
        {
            pastWeeks++;
        }

        public void TreatPastQuests(Character character)
        {
            
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