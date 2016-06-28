using UnityEngine;

namespace ToBeFree
{
    public class Piece
    {
        protected City city;

        public Piece(City city)
        {
            this.city = city;
        }

        public City City
        {
            get
            {
                return city;
            }
            set
            {
                city = value;
            }
        }
    }

    public class Police : Piece
    {
        public Police(City city) : base(city)
        {
        }
    }

    public class Information : Piece
    {
        public Information(City city) : base(city)
        {
        }
    }

    public class Broker : Piece
    {
        public Broker(City city) : base(city)
        {
        }
    }

    public class Quest : Piece
    {
        private int duration;
        private int leftDays;
        private Event curEvent;
        private Character character;

        public Quest(City city) : base(city)
        {
            this.duration = 14;
            this.leftDays = this.duration;
            TimeTable.Instance.NotifyEveryday += CheckTimeToDisapper;
        }

        public Quest(Event curEvent, Character character, City city) : this(city)
        {
            this.curEvent = curEvent;
            this.character = character;
        }

        public void CheckTimeToDisapper()
        {
            this.leftDays--;
            if (this.leftDays == 0)
            {
                Disapper();
            }
        }

        private void Disapper()
        {
            Debug.Log("Quest Disappered.");
            EventManager.Instance.ActivateResultEffects(this.curEvent.Result.Failure.EffectAmounts, this.character);
        }

        public Event CurEvent
        {
            get
            {
                return curEvent;
            }
            set
            {
                curEvent = value;
            }
        }
    }
}