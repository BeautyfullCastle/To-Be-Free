using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class UIQuestManager : MonoBehaviour
	{
		public GameObject QuestPref;
		public UIGrid grid;

		private List<UIQuest> uiQuestList = new List<UIQuest>();

		public void Awake()
		{
			QuestPiece.AddQuest += AddQuest;
			grid.onCustomSort = Sort;
		}

		public void OnDisable()
		{
			Clear();
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

			uiQuestList.Add(uiQuest);
			grid.Reposition();
		}

		public void DeleteQuest(Quest quest)
		{
			UIQuest uiQuest = uiQuestList.Find(x => x.QuestPiece.CurQuest.UiName == quest.UiName);
			if (uiQuest == null)
			{
				Debug.LogError(quest.UiName + " quest is not exist in this list.");
				return;
			}
			this.DeleteUIQuest(uiQuest);
		}

		private void Clear()
		{
			if (uiQuestList.Count == 0)
				return;

			foreach(UIQuest uiQuest in uiQuestList)
			{
				if (uiQuest.gameObject != null)
				{
					DestroyImmediate(uiQuest.gameObject);
				}
			}
			uiQuestList.Clear();
			grid.Reposition();
		}

		private void DeleteUIQuest(UIQuest uiQuest)
		{
			uiQuestList.Remove(uiQuest);

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
