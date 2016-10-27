using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
	public class UITimeTable : MonoBehaviour
	{
		public UILabel dayLabel;
		public UILabel hourLabel;
		public Transform hourhand;
		public UISprite lightSprite;

		private float angle;
		
		void Start()
		{
			TimeTable.Instance.NotifyEveryHour += Instance_NotifyEveryHour;
			TimeTable.Instance.NotifyEveryday += OnDayChange;
			angle = hourhand.localRotation.eulerAngles.z;
		}

		private void Instance_NotifyEveryHour()
		{
			int hour = TimeTable.Instance.Hour;
			hourLabel.text = hour.ToString() + ":00";

			if (6 <= hour && hour < 18)
				lightSprite.alpha = 0;
			else
				lightSprite.alpha = 0.5f;

			hour -= 6;
			hourhand.localRotation = Quaternion.AngleAxis(angle + hour*-15f, Vector3.forward);
		}

		private void OnDayChange()
		{
			dayLabel.text = TimeTable.Instance.Day.ToString() + " Day";
		}

	}
}