using System;
using UnityEngine;

namespace ToBeFree
{
    public class LanguageSelection : MonoBehaviour
    {
        public delegate void selectLanguageHandler(eLanguage language);
        static public event selectLanguageHandler selectLanguageForManager;
        static public event selectLanguageHandler selectLanguageForUI;

        public void SelectLanguage(string language)
        {
            selectLanguageForManager(EnumConvert<eLanguage>.ToEnum(language));
            selectLanguageForUI(EnumConvert<eLanguage>.ToEnum(language));
        }
    }
}