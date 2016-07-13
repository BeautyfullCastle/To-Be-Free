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
                DeleteUIQuest(uiQuest);
            }
            grid.Reposition();
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
                cityName = EnumConvert<eCity>.ToString(questPiece.City.Name);
            }
            uiQuest.SetLabels(questPiece.CurQuest.UiName, questPiece.PastWeeks.ToString() + "/" + questPiece.CurQuest.Duration.ToString(),
                questPiece.CurQuest.UiConditionScript, cityName);

            uiQuests.Add(uiQuest);
            grid.Reposition();
        }

        public void DeleteQuest(Quest quest)
        {
            UIQuest uiQuest = uiQuests.Find(x => x.QuestPiece.CurQuest == quest);
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
