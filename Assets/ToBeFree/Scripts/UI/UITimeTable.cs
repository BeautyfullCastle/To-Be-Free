using UnityEngine;

namespace ToBeFree
{
	public class UITimeTable : MonoBehaviour
	{
		public UILabel dayLabel;
		public Transform hourhand;

		private float angle;
		private string strDay;
		
		void Awake()
		{
			TimeTable.Instance.NotifyEveryHour += Instance_NotifyEveryHour;
			TimeTable.Instance.NotifyEveryday += OnDayChange;
			angle = hourhand.localRotation.eulerAngles.z;
			strDay = "Day";
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
		}

		void OnDisable()
		{
			ChangeDay(1);
			ChangeHour(6);
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			strDay = LanguageManager.Instance.Find(eLanguageKey.UI_Day);
		}

		private void Instance_NotifyEveryHour()
		{
			ChangeHour(TimeTable.Instance.Hour);
		}

		private void ChangeHour(int hour)
		{
			hour -= 6;
			hourhand.localRotation = Quaternion.AngleAxis(angle + hour * -15f, Vector3.forward);
		}

		private void OnDayChange()
		{
			ChangeDay(TimeTable.Instance.Day);
		}

		private void ChangeDay(int day)
		{
			dayLabel.text = day.ToString() + " " + strDay;
		}

	}
}