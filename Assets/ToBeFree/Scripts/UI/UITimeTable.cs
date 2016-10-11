using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
	public class UITimeTable : MonoBehaviour
	{
		public UILabel dayLabel;
		public UILabel hourLabel;

		// Use this for initialization
		void Start()
		{
			TimeTable.Instance.NotifyEveryHour += Instance_NotifyEveryHour;
			TimeTable.Instance.NotifyEveryday += OnDayChange;
		}

		private void Instance_NotifyEveryHour()
		{
			hourLabel.text = TimeTable.Instance.Hour.ToString() + ":00";
		}

		private void OnDayChange()
		{
			dayLabel.text = TimeTable.Instance.Day.ToString() + " Day";
		}

	}
}