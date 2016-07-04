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
            TimeTable.Instance.NotifyEveryWeek += Instance_NotifyEveryWeek;
        }

        private void Instance_NotifyEveryWeek()
        {
            Debug.Log("UIQuestManager : notify every week");
            List<UIQuest> questsToRemove = new List<UIQuest>();
            foreach(UIQuest uiQuest in uiQuests)
            {
                uiQuest.QuestPiece.WeekIsGone();
                uiQuest.duration.text = uiQuest.QuestPiece.PastWeeks.ToString() + "/" + uiQuest.QuestPiece.CurQuest.Duration.ToString();
                if (uiQuest.QuestPiece.CheckDuration())
                {
                    questsToRemove.Add(uiQuest);
                }
            }

            foreach (UIQuest uiQuest in questsToRemove)
            {
                uiQuests.Remove(uiQuest);
                uiQuest.QuestPiece.Disapper();
                DestroyImmediate(uiQuest.gameObject);                
            }
            grid.Reposition();
        }

        public void AddQuest(QuestPiece questPiece)
        {
            Debug.Log("UIQuestManager : AddQuest");
            GameObject questObj = NGUITools.AddChild(grid.gameObject, QuestPref);
            UIQuest uiQuest = questObj.GetComponent<UIQuest>();
            uiQuest.QuestPiece = questPiece;
            uiQuest.SetLabels(questPiece.CurQuest.UiName, questPiece.PastWeeks.ToString() + "/" + questPiece.CurQuest.Duration.ToString(),
                questPiece.CurQuest.UiConditionScript, EnumConvert<eCity>.ToString(questPiece.City.Name));

            uiQuests.Add(uiQuest);
            grid.Reposition();
        }
    }
}
