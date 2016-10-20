using UnityEngine;
using System.Collections;
using ToBeFree;

public class UICommand : MonoBehaviour {
	public eCommand commandType;
	private int skippedDay;

	// Use this for initialization
	void Awake () {
		Effect.SkipEvent += Effect_SkipEvent;
		TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;

		skippedDay = 10;

		EventDelegate.Parameter param = new EventDelegate.Parameter(this, gameObject.name);
		NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), this.GetComponent<UIButton>(), "ClickCommand", new EventDelegate.Parameter[] { param });

	}

	public void SetActiveCommands()
	{
		if (skippedDay < 2)
		{
			this.GetComponent<UIButton>().isEnabled = false;
		}
		else
		{
			// one command can use in one day.
			this.GetComponent<UIButton>().isEnabled = GameManager.Instance.Character.CanAction[(int)EnumConvert<eCommand>.ToEnum(this.gameObject.name)];

			if(this.GetComponent<UIButton>().isEnabled == false)
			{
				return;
			}

			// 브로커나 퀘스트가 있으면 활성화
			if (this.name == "BROKER")
			{
				bool hasBroker = PieceManager.Instance.GetNumberOfPiece(eSubjectType.BROKER, GameManager.Instance.Character.CurCity) > 0;

				this.GetComponent<UIButton>().isEnabled = hasBroker;
			}
			else if (this.name == "QUEST")
			{
				QuestPiece piece = PieceManager.Instance.Find(eSubjectType.QUEST, GameManager.Instance.Character.CurCity) as QuestPiece;
				bool hasAndCanDoQuest = false;
				if (piece == null)
				{
					hasAndCanDoQuest = false;
				}
				else
				{
					Quest quest = piece.CurQuest;
					hasAndCanDoQuest = quest.CheckCondition(GameManager.Instance.Character);
				}

				this.GetComponent<UIButton>().isEnabled = hasAndCanDoQuest;
			}
			else if(this.name == "WORK")
			{
				this.GetComponent<UIButton>().isEnabled = !(GameManager.Instance.Character.CurCity.Type == ToBeFree.eNodeType.MOUNTAIN
															|| GameManager.Instance.Character.CurCity.Type == ToBeFree.eNodeType.TOWN);
			}
			else if (this.name == "SHOP")
			{
				this.GetComponent<UIButton>().isEnabled = (GameManager.Instance.Character.CurCity.Type == ToBeFree.eNodeType.BIGCITY
										|| GameManager.Instance.Character.CurCity.Type == ToBeFree.eNodeType.MIDDLECITY);
			}
		}
	}

	private void Instance_NotifyEveryday()
	{
		skippedDay++;
	}

	private void Effect_SkipEvent(eCommand commandType)
	{
		if(this.commandType == commandType)
		{
			skippedDay = 0;
			this.gameObject.SetActive(false);
		}
	}

}
