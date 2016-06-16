using UnityEngine;

namespace ToBeFree
{
    public class Character
    {
        private Stat stat;
        private string name;
        private City curCity;

        private Inventory inven;
        
        // Todo : skill

        public Character(string name, Stat stat, City curCity,
                        int curMoney, int curFoodNum, int curInfoNum,
                        int curHP, int curMental, Inventory inven)
        {
            this.name = name;
            this.stat = stat;
            this.curCity = curCity;
            this.inven = inven;
        }

        private void Start()
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
            EventManager.Instance.DoCommand("INSPECT", this);
        }

        public bool MoveTo(City city)
        {
            if (city == null)
                return false;

            this.curCity = city;
            Debug.Log("character is moved to " + this.curCity.Name);

            return true;
        }

        public void Work()
        {
            Event selectedEvent = EventManager.Instance.DoCommand("WORK", this);
            // if effect is money and event is succeeded,
            ResultEffect[] successResulteffects = selectedEvent.Result.Success.Effects;

            for (int i = 0; i < successResulteffects.Length; ++i)
            {
                if (successResulteffects[i].Effect.SubjectType == eSubjectType.MONEY)
                {
                    this.Stat.Money += curCity.CalcRandWorkingMoney();
                    break;
                }
            }
            Debug.Log("character work.");
        }

        public void PrintMovableCity()
        {
            curCity.PrintNeighbors();
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
                CityGraph.Instance.CalculateDistance(this.curCity);
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