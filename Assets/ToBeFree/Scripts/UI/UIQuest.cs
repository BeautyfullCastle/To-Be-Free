using UnityEngine;

namespace ToBeFree
{
    public class UIQuest : MonoBehaviour
    {
        public UILabel questName;
        public UILabel duration;
        public UILabel condition;
        public UILabel city;

        private QuestPiece questPiece;
        

        public void SetLabels(string questName, string duration, string condition, string city)
        {
            this.questName.text = questName;
            this.duration.text = duration;
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
