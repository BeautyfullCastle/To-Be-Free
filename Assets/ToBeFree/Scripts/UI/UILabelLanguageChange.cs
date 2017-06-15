using UnityEngine;

namespace ToBeFree
{
	public class UILabelLanguageChange : MonoBehaviour
	{
		public eLanguageKey key;

		private UILabel label;

		void Awake()
		{
			if (label != null)
				return;

			label = this.GetComponent<UILabel>();
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
			LanguageSelection_selectLanguage(LanguageManager.Instance.CurrentLanguage);
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			label.text = LanguageManager.Instance.Find(key);
		}

		public void Refresh(eLanguageKey key)
		{
			if (this.label == null)
			{
				Awake();
			}

			this.key = key;
			label.text = LanguageManager.Instance.Find(key);
		}
	}
}
