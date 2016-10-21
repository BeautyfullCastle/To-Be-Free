using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class UIQuestManager : MonoBehaviour
    {
        public GameObject QuestPref;
        public UIGrid grid;

        private List<UIQuest> uiQuests = new List<UIQuest>();

        public void Awake()
        {
            QuestPiece.AddQuest += AddQuest;
        }

        public void AddQuest(QuestPiece questPiece)
        {
            Debug.Log("UIQuestManager : AddQuest");
            GameObject questObj = NGUITools.AddChild(grid.gameObject, QuestPref);
            UIQuest uiQuest = questObj.GetComponent<UIQuest>();
            uiQuest.QuestPiece = questPiece;
            string cityName = string.Empty;
            if(questPiece.City != null)
            {
                cityName = questPiece.City.Name;
            }
            uiQuest.SetLabels(questPiece.CurQuest.UiName, questPiece.PastDays.ToString() + "/" + questPiece.CurQuest.Duration.ToString(),
                questPiece.CurQuest.UiConditionScript, cityName);

            uiQuests.Add(uiQuest);
            grid.Reposition();
        }

        public void DeleteQuest(Quest quest)
        {
            UIQuest uiQuest = uiQuests.Find(x => x.QuestPiece.CurQuest == quest);
            if (uiQuest)
                DeleteUIQuest(uiQuest);
        }

        private void DeleteUIQuest(UIQuest uiQuest)
        {
            if (uiQuest.gameObject != null)
            {
                DestroyImmediate(uiQuest.gameObject);
            }
            uiQuests.Remove(uiQuest);
        }
    }
}
