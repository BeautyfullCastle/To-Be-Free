﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class BuffManager : Singleton<BuffManager>
    {
        private List<Buff> buffList;
        private Stat prevStat;

        public delegate void UpdateListHandler(Buff buff);
        static public event UpdateListHandler AddedBuff = delegate { };
        static public event UpdateListHandler DeletedBuff = delegate { };

        public BuffManager()
        {
            buffList = new List<Buff>();
            
        }

        public IEnumerator Add(Buff buff)
        {
            if (buffList == null || buff == null)
            {
                yield break;
            }
            Buff buffInList = buffList.Find(x => x == buff);
            if (buffInList == null)
            {
                buffList.Add(buff);
                AddedBuff(buff);
                Debug.Log(buff.Name + " is added to buff list.");
            }

            yield return null;

            // TODO : have to add this code to AbnormalCondition.
            //if (buffInList.IsStack)
            //{
            //    buff.Stack++;
            //    buff.Amount += buffInList.Amount;
            //    return buffInList;
            //}

        }

        public IEnumerator Delete(Buff buff, Character character)
        {
            if (buffList == null || buffList.Count == 0 || buff == null)
            {
                yield break;
            }

            // delete item what has same buff.
            character.Inven.Delete(buff, character);

            DeletedBuff(buff);

            yield return null;
        }

        public IEnumerator CheckStartTimeAndActivate(eStartTime startTime, Character character)
        {
            List<Buff> buffsToDelete = new List<Buff>();

            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == startTime)
                {
                    yield return buff.ActivateEffect(character);

                    if (buff.Duration == eDuration.ONCE)
                    {
                        buffsToDelete.Add(buff);
                    }
                }
            }

            foreach(Buff buff in buffsToDelete)
            {
                this.Delete(buff, character);
            }
        }

        public IEnumerator ActivateEffectByStartTime(eStartTime startTime, Character character)
        {
            prevStat = character.Stat.DeepCopy();
            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == startTime)
                {
                    yield return buff.ActivateEffect(character);
                }
            }
        }

        public void DeactivateEffectByStartTime(eStartTime startTime, Character character)
        {
            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == startTime)
                    buff.DeactivateEffect(character);
            }
            // restore character's stat
            character.Stat = prevStat.DeepCopy();
        }

        public IEnumerator Rest_Cure_PatienceTest(Character character)
        {
            Buff buff = buffList.Find(x => x.Duration == eDuration.REST_PATIENCE);
            if(buff == null)
            {
                yield break;
            }

            int patienceStat = character.Stat.Patience;

            yield return ActivateEffectByStartTime(eStartTime.TEST, character);
            bool isTestSucceed = DiceTester.Instance.Test(patienceStat, character);
            DeactivateEffectByStartTime(eStartTime.TEST, character);

            if (isTestSucceed)
            {
                DeletedBuff(buff);
                buffList.Remove(buff);
            }
            
        }

        public bool Contains(Buff buff)
        {
            return buffList.Contains(buff);
        }

        public bool Contains(string name)
        {
            return buffList.Contains(buffList.Find(x => x.Name == name));
        }

        public Buff Find(string name)
        {
            return buffList.Find(x => x.Name == name);
        }
    }

    
}