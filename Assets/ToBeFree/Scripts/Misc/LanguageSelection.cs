using System;
using UnityEngine;

namespace ToBeFree
{
	public class LanguageSelection : MonoBehaviour
	{
		public delegate void selectLanguageHandler(eLanguage language);
		static public event selectLanguageHandler selectLanguage;

		[SerializeField]
		private UIPopupList popupList;
		
		void Start()
		{
			if(popupList == null)
			{
				popupList = this.GetComponent<UIPopupList>();
			}
			
			popupList.value = EnumConvert<eLanguage>.ToString(LanguageManager.Instance.CurrentLanguage);
		}

		public void SelectLanguage(string language)
		{
			LanguageManager.Instance.SelectLanguage(EnumConvert<eLanguage>.ToEnum(language));
			selectLanguage(EnumConvert<eLanguage>.ToEnum(language));
		}

		public void Recall()
		{
			selectLanguage(LanguageManager.Instance.CurrentLanguage);
		}
	}
}