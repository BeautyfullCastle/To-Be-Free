using System;
using UnityEngine;

namespace ToBeFree
{

    public enum eStat
    {
        HP, TOTALHP, MENTAL, TOTALMENTAL, FOOD, MONEY, INFO, STRENGTH, AGILITY, OBSERVATION, BARGAIN, PATIENCE, LUCK, NULL
    }

    public enum eTestStat
    {
        STRENGTH, AGILITY, OBSERVATION, BARGAIN, PATIENCE, LUCK, ALL, NULL
    }

    public class Stat
    {
        private int hp;
        private int totalHP;
        private int mental;
        private int totalMental;

        private int strength;
        private int agility;
        private int observation;
        private int bargain;
        private int patience;
        private int luck;

        private int prevStrength;
        private int prevAgility;
        private int prevObservation;
        private int prevBargain;
        private int prevPatience;
        private int prevLuck;

        private int money;
        private int foodNum;

        private int infoNum;

        public delegate void OnValueChangeHandler(int value, eStat stat);
        static public event OnValueChangeHandler OnValueChange;

        public Stat()
        {
            this.TotalHP = 6;
            this.HP = this.totalHP;
            this.TotalMental = 6;
            this.MENTAL = this.TotalMental;

            this.Strength = 2;
            this.Agility = 2;
            this.Observation = 3;
            this.Bargain = 4;
            this.Patience = 2;
            this.Luck = 2;

            this.Money = 7;
            this.FOOD = 0;
            this.InfoNum = 0;
            
            Inventory.AddedItem += AddItem;
            Inventory.DeletedItem += DeleteItem;
        }

        public Stat(Stat stat)
        {
            this.Strength = stat.Strength;
            this.Agility = stat.Agility;
            this.Observation = stat.Observation;
            this.Bargain = stat.Bargain;
            this.Patience = stat.Patience;
            this.Luck = stat.Luck;
            this.totalHP = stat.totalHP;
            this.totalMental = stat.totalMental;
        }

        public Stat DeepCopy()
        {
            Stat stat = (Stat)this.MemberwiseClone();
            stat.Agility = this.Agility;
            stat.Bargain = this.Bargain;
            stat.Luck = this.Luck;
            stat.Observation = this.Observation;
            stat.Patience = this.Patience;
            stat.Strength = this.Strength;
            stat.totalHP      = this.totalHP;
            stat.totalMental  = this.totalMental;

            return stat;
        }

        private void AddItem(Item item)
        {
            if (item.Buff.StartTime == eStartTime.NIGHT)
            {
                this.FOOD++;
            }
        }

        private void DeleteItem(Item item)
        {
            if (item.Buff.StartTime == eStartTime.NIGHT)
            {
                this.FOOD--;
            }
        }

        public int Strength
        {
            get
            {
                return strength;
            }
            set
            {
                strength = Mathf.Max(value, 1);
                OnValueChange(strength, eStat.STRENGTH);
            }
        }

        public int Agility
        {
            get
            {
                return agility;
            }

            set
            {
                agility = Mathf.Max(value, 1);
                OnValueChange(agility, eStat.AGILITY);
            }
        }

        public int Observation
        {
            get
            {
                return observation;
            }

            set
            {
                observation = Mathf.Max(value, 1);
                OnValueChange(observation, eStat.OBSERVATION);
            }
        }

        public int Bargain
        {
            get
            {
                return bargain;
            }

            set
            {
                bargain = Mathf.Max(value, 1);
                OnValueChange(bargain, eStat.BARGAIN);
            }
        }

        public int Patience
        {
            get
            {
                return patience;
            }

            set
            {
                patience = Mathf.Max(value, 1);
                OnValueChange(patience, eStat.PATIENCE);
            }
        }

        public int Luck
        {
            get
            {
                return luck;
            }

            set
            {
                luck = Mathf.Max(value, 1);
                OnValueChange(luck, eStat.LUCK);
            }
        }

        public int HP
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                if (hp > TotalHP)
                {
                    hp = TotalHP;
                }
                OnValueChange(hp, eStat.HP);
                Debug.Log("HP : " + hp);
            }
        }

        
        public int TotalHP
        {
            get
            {
                return totalHP;
            }
            set
            {
                totalHP = value;
                OnValueChange(totalHP, eStat.TOTALHP);
            }
        }

        

        public int MENTAL
        {
            get
            {
                return mental;
            }
            set
            {
                mental = value;
                if (mental > TotalMental)
                {
                    mental = TotalMental;
                }
                OnValueChange(mental, eStat.MENTAL);
                Debug.Log("Mental : " + mental);
            }
        }

        public int TotalMental
        {
            get
            {
                return totalMental;
            }
            set
            {
                totalMental = value;
                OnValueChange(totalMental, eStat.TOTALMENTAL);
            }
        }
        

        public int FOOD
        {
            get
            {
                return foodNum;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }
                foodNum = value;
                OnValueChange(foodNum, eStat.FOOD);
            }
        }

        public int InfoNum
        {
            get
            {
                return infoNum;
            }

            set
            {
                if (value < 0)
                {
                    return;
                }
                infoNum = value;
                OnValueChange(InfoNum, eStat.INFO);
            }
        }

        public int Money
        {
            get
            {
                return money;
            }

            set
            {
                if (money + value < 0)
                {
                    throw new System.Exception("not enough money");
                }
                money = value;
                OnValueChange(money, eStat.MONEY);
            }
        }

        public void Set(eObjectType objectType, int amount)
        {

            switch (objectType)
            {
                case eObjectType.STRENGTH:
                    prevStrength = Strength;
                    Strength += amount;
                    break;
                case eObjectType.AGILITY:
                    prevAgility = Agility;
                    Agility += amount;
                    break;
                case eObjectType.OBSERVATION:
                    prevObservation = Observation;
                    Observation += amount;
                    break;
                case eObjectType.BARGAIN:
                    prevBargain = Bargain;
                    Bargain += amount;
                    break;
                case eObjectType.PATIENCE:
                    Patience = prevPatience;
                    Patience += amount;
                    break;
                case eObjectType.LUCK:
                    Luck = prevLuck;
                    Luck += amount;
                    break;
                case eObjectType.ALL:
                    prevStrength     = Strength;
                    prevAgility      = Agility;
                    prevObservation  = Observation;
                    prevBargain      = Bargain;
                    prevPatience     = Patience;
                    prevLuck         = Luck;

                    Strength += amount;
                    Agility += amount;
                    Observation += amount;
                    Bargain += amount;
                    Patience += amount;
                    Luck += amount;
                    break;
            }
        }

        public void Restore(eObjectType objectType)
        {
            switch(objectType)
            {
                case eObjectType.STRENGTH:
                    Strength = prevStrength;
                    break;
                case eObjectType.AGILITY:
                    Agility = prevAgility;
                    break;
                case eObjectType.OBSERVATION:
                    Observation = prevObservation;
                    break;
                case eObjectType.BARGAIN:
                    Bargain = prevBargain;
                    break;
                case eObjectType.PATIENCE:
                    Patience = prevPatience;
                    break;
                case eObjectType.LUCK:
                    Luck = prevLuck;
                    break;
                case eObjectType.ALL:
                    Strength = prevStrength;
                    Agility = prevAgility;
                    Observation = prevObservation;
                    Bargain = prevBargain;
                    Patience = prevPatience;
                    Luck = prevLuck;
                    break;
            }
        }
    }
}