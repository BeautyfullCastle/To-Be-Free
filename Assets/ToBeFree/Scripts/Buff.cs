using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Buff
    {
        private string name;
        private Effect effect;
        private bool isRestore;
        private int amount;

        private eStartTime startTime;
        private eDuration duration;

        private bool isStack;

        public Buff()
        {
            if(startTime == eStartTime.TEST)
            {
                EventManager.Instance.StartTestNotify += ActivateEffect;
                EventManager.Instance.EndTestNotify += DeactivateEffect;
            }
        }

        public Buff(string name, Effect effect, bool isRestore, int amount,
            eStartTime startTime, eDuration duration, bool isStack)
        {
            this.name = name;
            this.effect = effect;
            this.isRestore = isRestore;
            this.amount = amount;
            this.startTime = startTime;
            this.duration = duration;
            this.isStack = isStack;
        }

        public void ActivateEffect(Character character)
        {
            Debug.Log("Item effect activate");

            if (effect == null)
                return;

            effect.Activate(character, amount);
        }

        public void DeactivateEffect(Character character)
        {
            if (effect == null)
                return;

            effect.Activate(character, -amount);

            Debug.Log("Item.Use");
        }

        public eStartTime StartTime
        {
            get
            {
                return startTime;
            }
        }

        public eDuration Duration
        {
            get
            {
                return duration;
            }
        }

        public Effect Effect
        {
            get
            {
                return effect;
            }

            set
            {
                effect = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public bool IsStack
        {
            get
            {
                return isStack;
            }

            set
            {
                isStack = value;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }

            set
            {
                amount = value;
            }
        }
    }
}
