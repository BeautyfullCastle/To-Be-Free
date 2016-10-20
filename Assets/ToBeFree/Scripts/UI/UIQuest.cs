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
