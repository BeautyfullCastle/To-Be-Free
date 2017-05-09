using System;
using System.Collections;
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
			grid.onCustomSort = Sort;
		}

		public void OnDisable()
		{
			Clear();
		}

		public void Save(List<QuestSaveData> questList)
		{
			for (int i = 0; i < uiQuestList.Count; ++i)
			{
				Quest quest = uiQuestList[i].Quest;
				QuestPiece questPiece = uiQuestList[i].Piece;

				int cityIndex = -1;
				if(questPiece != null)
				{
					cityIndex = questPiece.City.Index;
				}
				QuestSaveData data = new QuestSaveData(quest.Index, uiQuestList[i].PastDays, cityIndex);
				questList.Add(data);
			}
		}

		public void Load(List<QuestSaveData> questList)
		{
			for (int i = 0; i < questList.Count; ++i)
			{
				Quest quest = QuestManager.Instance.GetByIndex(questList[i].index);
				QuestPiece piece = PieceManager.Instance.Find(eSubjectType.QUEST, CityManager.Instance.GetbyIndex(questList[i].cityIndex)) as QuestPiece;

				MakeUIQuest(quest, piece);
			}
		}

		public void AddQuest(Quest quest)
		{
			QuestPiece piece = null;
			City city = null;
			if (quest.Region != eRegion.NULL)
			{
				if (quest.Region == eRegion.CITY)
				{
					city = CityManager.Instance.Find(quest.CityName);
				}
				else if (quest.Region == eRegion.RANDOM)
				{
					city = CityManager.Instance.FindRandCityByDistance(GameManager.Instance.Character.CurCity, 3, eSubjectType.QUEST, eWay.NORMALWAY);
				}
				else if (quest.Region == eRegion.CURRENT)
				{
					city = GameManager.Instance.Character.CurCity;
				}

				while (true)
				{
					Piece questInCity = PieceManager.Instance.Find(eSubjectType.QUEST, city);
					if (questInCity == null)
					{
						break;
					}
					city = CityManager.Instance.FindRandCityByDistance(city, 1, eSubjectType.QUEST, eWay.NORMALWAY);
				}

				piece = new QuestPiece(city, eSubjectType.QUEST);
				PieceManager.Instance.List.Add(piece);
			}

			MakeUIQuest(quest, piece);
		}

		private void MakeUIQuest(Quest quest, QuestPiece piece)
		{
			GameObject questObj = NGUITools.AddChild(grid.gameObject, QuestPref);
			UIQuest uiQuest = questObj.GetComponent<UIQuest>();

			uiQuest.Init(quest, piece);
			uiQuestList.Add(uiQuest);
			grid.Reposition();
		}

		public UIQuest Find(QuestPiece piece)
		{
			List<UIQuest> tempList = new List<UIQuest>(uiQuestList);
			tempList.RemoveAll(x => x.Piece == null);
			return tempList.Find(x => x.Piece.City.Index == piece.City.Index);
		}

		public UIQuest Find(City city)
		{
			List<UIQuest> tempList = new List<UIQuest>(uiQuestList);
			tempList.RemoveAll(x => x.Piece == null);
			foreach (UIQuest uiQ in tempList)
				Debug.Log(uiQ.Piece.City.Name + " : " + uiQ.Piece.City.Index);

			Debug.Log("CurCity : " + city.Name + " : " + city.Index);

			return tempList.Find(x => x.Piece.City.Index == city.Index);
		}

		public void DeleteQuest(Quest quest)
		{
			UIQuest uiQuest = uiQuestList.Find(x => x.Quest.Index == quest.Index);
			if (uiQuest == null)
			{
				Debug.LogError(quest.UiName + " quest is not exist in this list.");
				return;
			}
			this.DeleteUIQuest(uiQuest);
		}

		public void Refresh()
		{
			foreach(UIQuest uiQuest in uiQuestList)
			{
				uiQuest.Refresh();
			}
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

		public void DeleteUIQuest(City curCity)
		{
			UIQuest uiQuest = this.Find(curCity);
			if (uiQuest == null)
				return;

			this.DeleteUIQuest(uiQuest);
		}

		private void DeleteUIQuest(UIQuest uiQuest)
		{
			uiQuestList.Remove(uiQuest);

			if(uiQuest.Piece != null)
			{
				PieceManager.Instance.Delete(uiQuest.Piece);
			}

			if (uiQuest.gameObject != null)
			{
				DestroyImmediate(uiQuest.gameObject);
			}

			grid.Reposition();
		}

		static private int Sort(Transform a, Transform b)
		{
			UIQuest aUIQuest = a.GetComponent<UIQuest>();
			UIQuest bUIQuest = b.GetComponent<UIQuest>();
			// actionType이 NULL이면 최상단으로 가게끔 정렬
			int aLayer = (int)aUIQuest.Quest.ActionType;
			int bLayer = (int)bUIQuest.Quest.ActionType;

			// layer가 같으면 오래된 놈이 먼저 보이게 정렬
			if(aLayer == bLayer)
			{
				aLayer = TimeTable.Instance.Day - aUIQuest.PastDays;
				bLayer = TimeTable.Instance.Day - bUIQuest.PastDays;
			}

			return aLayer.CompareTo(bLayer);
		}

		public IEnumerator TreatPastQuests()
		{
			List<UIQuest> tempList = new List<UIQuest>(uiQuestList);
			foreach(UIQuest quest in tempList)
			{
				yield return quest.TreatPastQuest();
			}
		}

		public IEnumerator Activate()
		{
			UIQuest uiQuest = this.Find(GameManager.Instance.Character.CurCity);
			if (uiQuest == null)
				yield break;

			yield return uiQuest.Activate();
		}
	}
}
