using UnityEngine;

namespace ToBeFree
{
    public class UIQuest : MonoBehaviour
    {
        public UILabel questName;
        public UILabel duration;
        public UILabel successEffect;
        public UILabel city;

        private QuestPiece questPiece;

        void Awake()
        {
        }

        public void SetLabels(string questName, string duration, string successEffect, string city)
        {
            this.questName.text = questName;
            this.duration.text = duration;
            this.successEffect.text = successEffect;
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
