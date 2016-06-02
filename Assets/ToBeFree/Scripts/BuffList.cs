using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class BuffList : Singleton<BuffList>
    {
        private List<Buff> buffList;

        public BuffList()
        {
            buffList = new List<Buff>();
            Rest.CureEventNotify += Rest_Cure_PatienceTest;

            DiceTester.Instance.StartTestNotify += ActivateTestEffect;
            DiceTester.Instance.EndTestNotify += DeactivateTestEffect;
        }

        public Buff Add(Buff buff)
        {
            if (buffList == null || buff == null)
            {
                return null;
            }
            Buff buffInList = buffList.Find(x => x == buff);
            if (buffInList == null)
            {
                buffList.Add(buff);
                Debug.Log(buff.Name + " is added to buff list.");
                return buff;
            }

            if (buffInList.IsStack)
            {
                buff.Amount += buffInList.Amount;
                return buffInList;
            }
            else
            {
                return null;
            }

        }

        public bool Delete(Buff buff)
        {
            if (buffList == null || buffList.Count == 0 || buff == null)
            {
                return false;
            }

            return buffList.Remove(buff);
        }

        public void CheckStartTimeAndActivate(eStartTime startTime, Character character)
        {
            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == startTime)
                {
                    buff.ActivateEffect(character);

                    if (buff.Duration == eDuration.ONCE)
                    {
                        buffList.Remove(buff);
                    }
                    
                }
            }
        }

        public void ActivateTestEffect(eStartTime startTime, Character character)
        {
            foreach (Buff buff in buffList)
            {
                if(buff.StartTime == startTime)
                    buff.ActivateEffect(character);
            }
        }

        public void DeactivateTestEffect(eStartTime startTime, Character character)
        {
            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == eStartTime.TEST)
                    buff.DeactivateEffect(character);
            }
        }

        private bool Rest_Cure_PatienceTest(Character character)
        {
            int patienceStat = character.Stat.Patience;
            bool isTestSucceed = DiceTester.Instance.Test(patienceStat, character);

            Buff buff = buffList.Find(x => x.Duration == eDuration.PAT_TEST_REST);
            
            if (isTestSucceed && buff != null)
            {
                buffList.Remove(buff);
            }

            return isTestSucceed;
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