﻿using UnityEngine;

namespace ToBeFree
{
	public class UICommandPopup : MonoBehaviour {
		public UILabel nameLabel;
		public UILabel explainLabel;
		public UILabel requiredTimeLabel;
		public UIButton[] requiredTimeButtons;
		public UIButton moveButton;
		public eEventAction actionType;
		
		// Use this for initialization
		void Start()
		{
			foreach(UIButton button in requiredTimeButtons)
			{
				InitButton(button);
			}

			InitButton(moveButton);

			ChangeLanguage();
		}

		private void InitButton(UIButton button)
		{
			if (button == null)
				return;

			button.defaultColor = Color.white;
			EventDelegate.Parameter[] parameters = new EventDelegate.Parameter[] { new EventDelegate.Parameter(this, "actionType"), new EventDelegate.Parameter(button.transform, "name") };
			NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), button, "ClickCommandRequiredTime", parameters);
		}

		private void ChangeLanguage()
		{
			string name = string.Empty;
			string explain = string.Empty;
			if(actionType == eEventAction.INVESTIGATION_CITY)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_CityInquiry);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_City_Inquiry);
			}
			else if (actionType == eEventAction.INVESTIGATION_BROKER)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_BrokerInquiry);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Broker_Inquiry);
			}
			else if (actionType == eEventAction.INVESTIGATION_POLICE)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_PoliceInquiry);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Police_Inquiry);
			}
			else if (actionType == eEventAction.GATHERING)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_GatheringInquiry);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Gathering_Inquiry);
			}
			else if (actionType == eEventAction.REST)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_Rest);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Rest);
			}
			else if (actionType == eEventAction.HIDE)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_HideRest);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Hide_Rest);
			}
			else if (actionType == eEventAction.MOVE)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_Walking);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Walking);
			}
			else if (actionType == eEventAction.MOVE_BUS)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_Bus);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Bus_Move);
			}
			else if(actionType == eEventAction.WORK)
			{
				name = LanguageManager.Instance.Find(eLanguageKey.UI_Work);
				explain = LanguageManager.Instance.Find(eLanguageKey.Popup_Work);
			}

			nameLabel.text = name;
			explainLabel.text = explain;

			requiredTimeLabel.text = LanguageManager.Instance.Find(eLanguageKey.UI_RequiredTime);
		}
	}
}