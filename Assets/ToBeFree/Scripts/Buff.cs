using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public enum eBuff
    {
        NAME, AMOUNT, STACK
    }

    public class Buff
    {
        private string name;
        private Effect effect;
        private bool isRestore;
        private int amount;

        private int stack;
        private bool isStack;

        private eStartTime startTime;
        private eDuration duration;

        
        public Buff(string name, Effect effect, bool isRestore, int amount,
            eStartTime startTime, eDuration duration, bool isStack=false)
        {
            this.name = name;
            this.effect = new Effect(effect);
            this.isRestore = isRestore;
            this.amount = amount;
            this.startTime = startTime;
            this.duration = duration;
            this.isStack = isStack;
            this.stack = 1;
        }

        public Buff(Buff buff) : this(buff.name, buff.effect, buff.isRestore, buff.amount,
            buff.startTime, buff.duration, buff.isStack)
        {
        }

        public void ActivateEffect(Character character)
        {
            Debug.Log("buff " + name + "'s effect activate");

            if (effect == null)
                return;

            effect.Activate(character, amount);
        }

        public void DeactivateEffect(Character character)
        {
            Debug.Log("buff " + name + "'s effect deactivate");

            if (effect == null)
                return;

            effect.Deactivate(character);
            
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

        public bool IsRestore
        {
            get
            {
                return isRestore;
            }

            set
            {
                isRestore = value;
            }
        }

        public int Stack
        {
            get
            {
                return stack;
            }

            set
            {
                stack = value;
            }
        }
    }
}
