using UnityEngine;

namespace ToBeFree
{
	public class LanguageSelection : MonoBehaviour
	{
		public delegate void selectLanguageHandler(eLanguage language);
		static public event selectLanguageHandler selectLanguage;
		
		public void SelectLanguage(string language)
		{
			LanguageManager.Instance.LanguageSelection_selectLanguageForUI(EnumConvert<eLanguage>.ToEnum(language));
			selectLanguage(EnumConvert<eLanguage>.ToEnum(language));
		}
	}
}