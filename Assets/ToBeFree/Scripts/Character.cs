using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Character
    {
        private Stat stat;
        private string name;
        private City curCity;
        private City nextCity;

        private Inventory inven;

        public delegate void MoveCityHandler(string cityName);
        public static event MoveCityHandler MoveCity = delegate { };
        
        // Todo : skill

        public Character(string name, Stat stat, City curCity,
                        int curMoney, int curFoodNum, int curInfoNum,
                        int curHP, int curMental, Inventory inven)
        {
            this.name = name;
            this.stat = stat;
            this.curCity = curCity;
            this.inven = inven;

            CantCure = false;
            CantMove = false;
            IsFull = false;
            IsDetention = false;
        }
        
        public int GetDiceNum(eTestStat stat)
        {
            switch (stat)
            {
                case eTestStat.STRENGTH:
                    return this.Stat.Strength;
                case eTestStat.AGILITY:
                    return this.Stat.Agility;
                case eTestStat.OBSERVATION:
                    return this.Stat.Observation;
                case eTestStat.BARGAIN:
                    return this.Stat.Bargain;
                case eTestStat.PATIENCE:
                    return this.Stat.Patience;
                case eTestStat.LUCK:
                    return this.Stat.Luck;
                case eTestStat.ALL:
                case eTestStat.NULL:
                    return -99;
                default:
                    Debug.LogError("GetDiceNum : Stat name is not correct : " + stat);
                    return -1;
            }
        }

        public void Rest()
        {
            if(CantCure)
            {
                return;
            }
            Stat.HP++;
            Stat.MENTAL++;
        }

        public void Inspect()
        {
            EventManager.Instance.DoCommand(eEventAction.INSPECT, this);
        }

        public IEnumerator MoveTo(City city)
        {
            if (city == null)
            {
                yield break;
            }
            if(CantMove)
            {
                yield break;
            }
            
            
            yield return GameManager.Instance.MoveDirectingCam(
                GameManager.Instance.FindGameObject(this.curCity.Name.ToString()).transform.position,
                GameManager.Instance.FindGameObject(city.Name.ToString()).transform.position,
                5f);

            this.curCity = city;
            MoveCity(EnumConvert<eCity>.ToString(city.Name));

            Debug.Log("character is moved to " + this.curCity.Name);

            yield return null;
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
                CityManager.Instance.CalculateDistance(this.curCity);
            }
        }

        public Inventory Inven
        {
            get
            {
                return inven;
            }
        }

        public City NextCity
        {
            get
            {
                return nextCity;
            }

            set
            {
                nextCity = value;
            }
        }

        public bool IsFull { get; internal set; }
        public bool IsDetention { get; internal set; }
        public bool CantCure { get; internal set; }
        public bool CantMove { get; internal set; }

        public IEnumerator HaulIn()
        {
            City city = CityManager.Instance.GetNearestCity(CurCity);
            yield return MoveTo(city);
        }
    }
}