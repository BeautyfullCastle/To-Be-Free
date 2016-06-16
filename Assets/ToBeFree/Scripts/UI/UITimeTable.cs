using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
    public class UITimeTable : MonoBehaviour
    {
        public UILabel label;

        // Use this for initialization
        void Start()
        {
            TimeTable.Instance.NotifyEveryday += OnDayChange;
        }

        private void OnDayChange()
        {
            label.text = TimeTable.Instance.Day.ToString() + " Day";
        }

    }
}