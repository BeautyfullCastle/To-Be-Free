using System.Collections.Generic;

namespace ToBeFree
{
    public class BuffList : Singleton<BuffList>
    {
        private List<Buff> buffList;

        public BuffList()
        {
            buffList = new List<Buff>();
        }

        public Buff Add(Buff buff)
        {
            if (buffList == null || buff == null)
            {
                return null;
            }
            Buff buffInList = buffList.Find(x => x == buff);
            if (buffInList != null)
            {
                if(buffInList.IsStack)
                {
                    buff.Amount += buffInList.Amount;
                    return buffInList;
                }
            }
            buffList.Add(buff);

            return buff;
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
                    CheckDurationAndDelete(buff, character.Inven);

                    buffList.Remove(buff);
                }
            }
        }

        private void CheckDurationAndDelete(Buff buff, Inventory inven)
        {
            if (buff.Duration == eDuration.ONCE)
            {
                buffList.Remove(buff);
            }
        }

        public bool Contains(Buff item)
        {
            return buffList.Contains(item);
        }
    }

    
}