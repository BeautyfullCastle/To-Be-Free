using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ToBeFree
{
    public class Piece { }

    public class Police : Piece { }

    public class Information : Piece { }

    public class Broker : Piece { }

    public class Quest : Piece
    {
        private int duration;
        private int leftDays;
        private Event curEvent;
        private Character character;
        private City city;

        public Quest()
        {
            this.duration = 14;
            this.leftDays = this.duration;
            TimeTable.Instance.NotifyEveryday += CheckTimeToDisapper;
        }

        public Quest(Event curEvent, Character character) : this()
        {
            this.curEvent = curEvent;
            this.character = character;
        }

        public void CheckTimeToDisapper()
        {
            this.leftDays--;
            if(this.leftDays == 0)
            {
                Disapper();
            }
        }

        private void Disapper()
        {
            Debug.Log("Quest Disappered.");
            EventManager.Instance.ActivateResultEffects(this.curEvent.Result.Failure.Effects, this.character);
            city.PieceList.Remove( (Piece)this );
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

}
