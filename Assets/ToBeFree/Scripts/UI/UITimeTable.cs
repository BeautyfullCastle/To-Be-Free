using UnityEngine;

namespace ToBeFree
{
	public class UITimeTable : MonoBehaviour
	{
		public UILabel dayLabel;
		public Transform hourhand;

		private float angle;
		private string strDay;
		
		void Start()
		{
			TimeTable.Instance.NotifyEveryHour += Instance_NotifyEveryHour;
			TimeTable.Instance.NotifyEveryday += OnDayChange;
			angle = hourhand.localRotation.eulerAngles.z;
			strDay = "Day";
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			strDay = LanguageManager.Instance.Find(eLanguageKey.UI_Day);
		}

		private void Instance_NotifyEveryHour()
		{
			int hour = TimeTable.Instance.Hour;
			hour -= 6;
			hourhand.localRotation = Quaternion.AngleAxis(angle + hour*-15f, Vector3.forward);
		}

		private void OnDayChange()
		{
			dayLabel.text = TimeTable.Instance.Day.ToString() + " " + strDay;
		}

	}
}