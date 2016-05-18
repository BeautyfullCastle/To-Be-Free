using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ToBeFree
{
    public class TimeTable : Singleton<TimeTable>
    {
        private int day;

        public delegate void TimeEventHandler();
        public event TimeEventHandler NotifyEveryday;

        public void DayIsGone()
        {
            ++day;
            NotifyEveryday();
            Debug.Log("Alived day : " + day);
        }
    }
}
