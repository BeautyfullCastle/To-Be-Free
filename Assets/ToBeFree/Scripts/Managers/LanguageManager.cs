using System;
using System.Collections.Generic;
using Language;
using UnityEngine;

namespace ToBeFree
{
	public enum eLanguage
	{
		ENGLISH, KOREAN
	}

	public enum eLanguageKey
	{
		UI_Move, UI_Work, UI_Inquiry, UI_Rest, UI_Shop, UI_Broker, UI_Quest, UI_Abilty, UI_CityInquiry, UI_BrokerInquiry, UI_PoliceInquiry,
		UI_GatheringInquiry, UI_HideRest, UI_Walking, UI_Bus, UI_HP, UI_Satiety, UI_Money, UI_Info, UI_Strength, UI_Agility, UI_Talent,
		UI_Focus, UI_Day, UI_Setting, UI_Resolution, UI_FullScreen, UI_MusicVolume, UI_EffectVolume, UI_Mute, UI_Language,
		UI_Police_Turn, UI_Your_Turn, Event_Police_Add, Event_Police_AddStat, Event_Police_Move, Event_Police_CrackDown, UI_Road,
		UI_SmallTown, UI_MiddleCity, UI_BigCity, UI_Mountain, UI_RequiredTime,
		Event_Start_Working, Event_Start_CityInvestigation, Event_Start_PoliceInvestigation, Event_Start_BrokerInfoInvestigation,
		Event_Start_Gathering, Event_Start_Rest, Event_Start_Broker, Event_Start_Walking, Event_Start_Bus, Event_Start_Ability,		
		Over_Move, Over_Work, Over_Inquiry, Over_Rest, Over_Shop, Over_Abilty, Over_Quest, Over_Broker,
		Popup_Walking, Popup_Bus_Move, Popup_City_Inquiry, Popup_Broker_Inquiry, Popup_Police_Inquiry, Popup_Gathering_Inquiry,
		Popup_Hide_Rest, Popup_Rest, Popup_Work,
		Event_Police_RevealNumber,
		Event_PoliceNumber,
		Event_WoringMoneyPerCity,
		Event_SucceedDiceNumber,
		Event_TotalMoney
	}

	public class LanguageManager : Singleton<LanguageManager>
	{
		private string fileName;
		private eLanguage currentLanguage;
		private Language.LanguageData[] engList;
		private Language.LanguageData[] korList;
		private List<Language.LanguageData[]> languageList;


		public LanguageManager()
		{
		}

		public void Init()
		{
			fileName = "Language.json";
			
			engList = new DataList<Language.LanguageData>(Application.streamingAssetsPath + "/Language/English/" + fileName).dataList;
			korList = new DataList<Language.LanguageData>(Application.streamingAssetsPath + "/Language/Korean/" + fileName).dataList;
			languageList = new List<Language.LanguageData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			GameManager.Instance.languageSelection.SelectLanguage("KOREAN");
		}

		public void LanguageSelection_selectLanguageForUI(eLanguage language)
		{
			currentLanguage = language;
		}

		public string Find(eLanguageKey key)
		{
			LanguageData data = Array.Find<LanguageData>(languageList[(int)currentLanguage], x => x.key == key.ToString());
			return data.script;
		}

	}
}