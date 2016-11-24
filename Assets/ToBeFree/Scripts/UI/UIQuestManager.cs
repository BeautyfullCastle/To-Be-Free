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
			grid.onCustomSort = Sort;
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
				cityName = questPiece.City.IconCity.nameLabel.text;
			}
			uiQuest.SetLabels(questPiece.CurQuest.UiName, questPiece.CurQuest.PastDays.ToString() + "/" + questPiece.CurQuest.Duration.ToString(),
				questPiece.CurQuest.UiConditionScript, cityName);

			uiQuests.Add(uiQuest);
			grid.Reposition();
		}

		public void DeleteQuest(Quest quest)
		{
			UIQuest uiQuest = uiQuests.Find(x => x.QuestPiece.CurQuest.UiName == quest.UiName);
			if (uiQuest)
				DeleteUIQuest(uiQuest);
		}

		private void DeleteUIQuest(UIQuest uiQuest)
		{
			uiQuests.Remove(uiQuest);

			if(uiQuest.QuestPiece != null)
			{
				PieceManager.Instance.Delete(uiQuest.QuestPiece);
			}

			if (uiQuest.gameObject != null)
			{
				DestroyImmediate(uiQuest.gameObject);
			}
			
		}

		static private int Sort(Transform a, Transform b)
		{
			int aActionType = (int)a.GetComponent<UIQuest>().QuestPiece.CurQuest.ActionType;
			int bActionType = (int)b.GetComponent<UIQuest>().QuestPiece.CurQuest.ActionType;
			return aActionType.CompareTo(bActionType);
		}
	}
}
