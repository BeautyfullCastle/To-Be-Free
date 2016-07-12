using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class BuffManager : Singleton<BuffManager>
    {
        private List<Buff> buffList;
        private Stat prevStat;

        public delegate void UpdateListHandler(Buff buff);
        static public event UpdateListHandler AddedBuff;
        static public event UpdateListHandler DeletedBuff;

        public BuffManager()
        {
            buffList = new List<Buff>();

            TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;
        }

        private void Instance_NotifyEveryday()
        {
            foreach(Buff buff in buffList)
            {
                buff.AliveDays++;
            }
        }

        public bool Exist(Buff buff)
        {
            return buffList.Exists(x => x == buff);
        }

        public IEnumerator Add(Buff buff)
        {
            if (buffList == null || buff == null)
            {
                yield break;
            }
            buffList.Add(buff);
            //AddedBuff(buff);
            GameManager.Instance.FindGameObject("BuffManager").GetComponent<UIBuffManager>().AddBuff(buff);
            Debug.Log(buff.Name + " is added to buff list.");

            yield return null;

        }

        public IEnumerator Delete(Buff buff, Character character)
        {
            if (buffList == null || buffList.Count == 0 || buff == null)
            {
                yield break;
            }
            
            DeletedBuff(buff);

            yield return null;
        }
        
        public IEnumerator ActivateEffectByStartTime(eStartTime startTime, Character character)
        {
            yield return GameManager.Instance.ShowStateLabel("Activate Effect : " + startTime.ToString(), 0.5f);
            
            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == startTime)
                {
                    yield return buff.ActivateEffect(character);
                }
            }
        }

        public IEnumerator DeactivateEffectByStartTime(eStartTime startTime, Character character)
        {
            yield return GameManager.Instance.ShowStateLabel("DeActivate Effect : " + startTime.ToString(), 0.5f);

            foreach (Buff buff in buffList)
            {
                if (buff.StartTime == startTime)
                {
                    yield return buff.DeactivateEffect(character);
                }
            }
        }

        public IEnumerator CheckDuration(Character character)
        {
            yield return GameManager.Instance.ShowStateLabel("Check Buff's duration", 0.5f);

            List<Buff> buffsToDelete = new List<Buff>();
            foreach (Buff buff in buffList)
            {
                if (buff.CheckDuration())
                {
                    buffsToDelete.Add(buff);
                }
            }

            foreach (Buff buff in buffsToDelete)
            {
                yield return this.Delete(buff, character);
            }
            yield return null;
        }

        public IEnumerator Rest_Cure_PatienceTest(Character character)
        {
            Buff buff = buffList.Find(x => x.Duration == eDuration.REST_PATIENCE);
            if(buff == null)
            {
                yield break;
            }

            int patienceStat = character.Stat.Patience;

            GameManager.Instance.uiEventManager.OpenUI();

            yield return ActivateEffectByStartTime(eStartTime.TEST, character);
            bool isTestSucceed = DiceTester.Instance.Test(patienceStat, character);
            yield return DeactivateEffectByStartTime(eStartTime.TEST, character);

            GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, "Rest Cure Patience Test");
            GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, isTestSucceed.ToString());

            yield return EventManager.Instance.WaitUntilFinish();

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