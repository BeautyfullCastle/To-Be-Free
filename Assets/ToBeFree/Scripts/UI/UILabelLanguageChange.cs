using UnityEngine;

namespace ToBeFree
{
	public class UILabelLanguageChange : MonoBehaviour
	{
		public eLanguageKey key;

		private UILabel label;

		void OnEnable()
		{
			label = this.GetComponent<UILabel>();
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
			LanguageSelection_selectLanguage(LanguageManager.Instance.CurrentLanguage);
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			label.text = LanguageManager.Instance.Find(key);
		}
	}
}
