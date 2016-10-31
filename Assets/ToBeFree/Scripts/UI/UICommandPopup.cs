using UnityEngine;
using System;

namespace ToBeFree {
	public class UICommandPopup : MonoBehaviour {
		public UILabel nameLabel;
		public UILabel requiredTimeLabel;
		public UIButton[] requiredTimeButtons;
		public eEventAction actionType;

		// Use this for initialization
		void Start()
		{
			foreach(UIButton button in requiredTimeButtons)
			{
				EventDelegate.Parameter[] parameters = new EventDelegate.Parameter[] { new EventDelegate.Parameter(this, "actionType"), new EventDelegate.Parameter(button.transform, "name") };
				NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), button, "ClickCommandRequiredTime", parameters);
			}
		}
	}
}