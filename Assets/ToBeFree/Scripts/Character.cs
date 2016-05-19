using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree
{
    public class Character
    {
        Stat stat;
        string name;
        City curCity;
        int curMoney;
        int curFoodNum;
        int curInfoNum;
        int curHP;
        int curMental;
        
        private Inventory inven;

        // Todo : skill

        public Character(string name, Stat stat, City curCity,
                        int curMoney, int curFoodNum, int curInfoNum,
                        int curHP, int curMental, Inventory inven)
        {
            this.name = name;
            this.stat = stat;
            this.curCity = curCity;
            this.curMoney = curMoney;
            this.curFoodNum = curFoodNum;
            this.curInfoNum = curInfoNum;
            this.curHP = curHP;
            this.curMental = curMental;
            this.inven = inven;
        }

        void Start()
        {
            //int temp = stat.Strength;
        }
        
        public int GetDiceNum(string stat)
        {
            switch (stat)
            {
                case "STR":
                    return this.Stat.Strength;
                case "AGI":
                    return this.Stat.Agility;
                case "OBS":
                    return this.Stat.Observation;
                case "BAR":
                    return this.Stat.Bargain;
                case "PAT":
                    return this.Stat.Patience;
                case "LUC":
                    return this.Stat.Luck;
                default:
                    Debug.LogError("GetDiceNum : Stat name is not correct : " + stat);
                    return -1;
            }
        }

        public void Inspect()
        {
            EventManager.Instance.DoCommand("Inspection", this);
        }

        public bool MoveTo(City city)
        {
            EventManager.Instance.DoCommand("Move", this);
            this.curCity = city;
            Debug.Log("character is moved to " + this.curCity.Name);

            return true;
        }

        public void Work()
        {
            Event selectedEvent = EventManager.Instance.DoCommand("Work", this);
            // if effect is money and event is succeeded,
            ResultEffect[] successResulteffects = selectedEvent.Result.Success.Effects;

            for (int i = 0; i < successResulteffects.Length; ++i)
            {
                if (successResulteffects[i].Effect.BigType == "MONEY")
                {
                    this.curMoney += curCity.CalcRandWorkingMoney();
                    break;
                }
            }
            Debug.Log("character work.");
        }

        public void PrintMovableCity()
        {
            curCity.PrintNeighbors();
        }

        public int HP
        {
            get
            {
                return curHP;
            }
            set
            {
                curHP = value;
                if (curHP > stat.TotalHP)
                {
                    curHP = stat.TotalHP;
                }
            }
        }

        public int MENTAL
        {
            get
            {
                return curMental;
            }
            set
            {
                curMental = value;
                if (curMental > stat.TotalMental)
                {
                    curMental = stat.TotalMental;
                }
            }
        }

        public int FOOD
        {
            get
            {
                return curFoodNum;
            }
            set
            {
                curFoodNum += value;
                if (curFoodNum > stat.TotalFoodNum)
                {
                    curFoodNum = stat.TotalFoodNum;
                }
            }
        }

        public Stat Stat
        {
            get
            {
                return stat;
            }

            set
            {
                stat = value;
            }
        }

        public City CurCity
        {
            get
            {
                return curCity;
            }

            set
            {
                curCity = value;
            }
        }

        public Inventory Inven
        {
            get
            {
                return inven;
            }
        }
    }
}