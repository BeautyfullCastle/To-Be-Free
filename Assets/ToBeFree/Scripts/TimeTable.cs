using UnityEngine;

namespace ToBeFree
{
    public class TimeTable : Singleton<TimeTable>
    {
        private int day;
        
        public delegate void TimeEventHandler();

        public event TimeEventHandler NotifyEveryday;

        public event TimeEventHandler NotifyEveryWeek;

        public TimeTable()
        {
            day = 1;
        }

        public void DayIsGone()
        {
            ++day;

            NotifyEveryday();

            if (day % 7 == 0)
            {
                Debug.Log((day / 7) + " weeks are gone.");
                NotifyEveryWeek();
            }

            Debug.Log("Alived day : " + day);
        }

        public int Day
        {
            get
            {
                return day;
            }
        }
    }
}