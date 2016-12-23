﻿using System;
using UnityEngine;

namespace ToBeFree
{
	public class UIQuest : MonoBehaviour
	{
		public UILabel questName;
		public UILabel pastDays;
		public UILabel condition;
		public UILabel city;

		private QuestPiece questPiece;
		
		public void SetLabels(string questName, string pastDays, string condition, string city)
		{
			this.questName.text = questName;
			this.pastDays.text = pastDays;
			this.condition.text = condition;
			
			this.city.text = city;

			TimeTable.Instance.NotifyEveryday += DayIsGone;
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
		}

		public void Refresh()
		{
			Quest quest = QuestManager.Instance.GetByIndex(questPiece.CurQuest.Index);
			if (quest == null)
				return;

			this.questName.text = quest.UiName;
			this.condition.text = quest.UiConditionScript;
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			if(questPiece.City == null)
			{
				this.city.text = string.Empty;
				return;
			}
			
			this.city.text = questPiece.City.IconCity.nameLabel.text;
		}

		private void OnDisable()
		{
			TimeTable.Instance.NotifyEveryday -= DayIsGone;
			LanguageSelection.selectLanguage -= LanguageSelection_selectLanguage;
		}

		private void DayIsGone()
		{
			if(questPiece.CurQuest.Duration == 1000)
			{
				return;
			}
			questPiece.CurQuest.PastDays++;
			pastDays.text = questPiece.CurQuest.PastDays.ToString() + "/" + questPiece.CurQuest.Duration.ToString();
		}

		public QuestPiece QuestPiece
		{
			get
			{
				return questPiece;
			}

			set
			{
				questPiece = value;
			}
		}
	}
}
