using UnityEngine;

namespace ToBeFree
{
    public class TimeTable : Singleton<TimeTable>
    {
        private int day;
        private readonly int week;

        public delegate void TimeEventHandler();

        public event TimeEventHandler NotifyEveryday;

        public event TimeEventHandler NotifyEveryWeek;

        public TimeTable()
        {
            day = 1;
            week = 4;
        }

        public void DayIsGone()
        {
            ++day;

            NotifyEveryday();

            if (day % week == 0)
            {
                Debug.Log((day / week) + " weeks are gone.");
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