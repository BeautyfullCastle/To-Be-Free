using UnityEngine;

namespace ToBeFree
{
	public class UILabelLanguageChange : MonoBehaviour
	{
		public eLanguageKey key;

		private UILabel label;

		void Start()
		{
			label = this.GetComponent<UILabel>();
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			label.text = LanguageManager.Instance.Find(key);
		}
	}
}
