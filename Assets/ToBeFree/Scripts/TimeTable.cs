using System;
using System.Collections.Generic;
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
                NotifyEveryWeek();
            
            Debug.Log("Alived day : " + day);
        }
    }
}
