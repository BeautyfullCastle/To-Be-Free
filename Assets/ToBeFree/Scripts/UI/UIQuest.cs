using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class UIQuest : MonoBehaviour
	{
		public UILabel questNameLabel;
		public UILabel pastDaysLabel;
		public UILabel conditionLabel;
		public UILabel cityLabel;

		private Quest quest;
		private QuestPiece piece;
		private int pastDays;
		
		public void Init(Quest quest, QuestPiece piece)
		{
			this.quest = quest;
			this.piece = piece;
			this.pastDays = 0;
			
			TimeTable.Instance.NotifyEveryday += DayIsGone;

			this.Refresh();
		}

		public void Refresh()
		{
			this.questNameLabel.text = quest.UiName;
			this.conditionLabel.text = quest.UiConditionScript;
			
			if (quest.Duration == 1000)
			{
				this.pastDaysLabel.text = "-";
			}
			else
			{
				this.pastDaysLabel.text = pastDays.ToString() + "/" + quest.Duration.ToString();
			}

			if (this.Piece == null)
			{
				this.cityLabel.text = "-";
			}
			else
			{
				this.cityLabel.text = CityManager.Instance.GetName(this.piece.City);
			}
		}
		
		private void OnDisable()
		{
			TimeTable.Instance.NotifyEveryday -= DayIsGone;
		}

		private void DayIsGone()
		{
			if(quest.Duration == 1000)
			{
				return;
			}
			this.PastDays++;
		}

		public IEnumerator TreatPastQuest()
		{
			if (PastDays >= quest.Duration)
			{
				yield return GameManager.Instance.uiEventManager.OnChanged(quest.FailureEffects.Script, true, false);

				string effectScript = string.Empty;
				if (quest.FailureEffects.EffectAmounts != null)
				{
					foreach (EffectAmount effectAmount in quest.FailureEffects.EffectAmounts)
					{
						if (effectAmount == null)
							continue;

						effectScript += effectAmount.ToString();
					}
				}

				yield return GameManager.Instance.uiEventManager.OnChanged(effectScript, false, true);

				GameManager.Instance.uiQuestManager.DeleteQuest(this.quest);
			}
		}

		public bool CheckCondition()
		{
			return quest.CheckCondition(GameManager.Instance.Character, pastDays);
		}

		public IEnumerator Activate()
		{
			Character character = GameManager.Instance.Character;
			if (character == null)
				yield break;

			EventManager.Instance.TestResult = quest.CheckCondition(character, pastDays);
			if (EventManager.Instance.TestResult)
			{
				yield return QuestManager.Instance.ActivateQuest(quest, character);
			}

			// have to check TestResult again cause of Dice Test of activated quest.
			if (EventManager.Instance.TestResult == true)
			{
				GameManager.Instance.uiQuestManager.DeleteUIQuest(character.CurCity);
			}
		}

		public QuestPiece Piece
		{
			get
			{
				return piece;
			}
		}

		public int PastDays
		{
			get
			{
				return pastDays;
			}
			set
			{
				pastDays = value;
				this.Refresh();
			}
		}

		public Quest Quest
		{
			get
			{
				return quest;
			}
		}
	}
}
