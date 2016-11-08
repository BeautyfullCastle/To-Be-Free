using UnityEngine;
using System.Collections;
using ToBeFree;

public class UICommand : MonoBehaviour {
	private bool deactive;
	private UILabel nameLabel;

	// Use this for initialization
	void Awake () {
		Effect.DeactiveEvent += DeactiveEvent;

		deactive = false;

		nameLabel = this.transform.GetComponentInChildren<UILabel>();
		nameLabel.text = this.gameObject.name;

		this.GetComponent<UISprite>().spriteName = this.gameObject.name;

		EventDelegate.Parameter param = new EventDelegate.Parameter(this, "name");
		NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), this.GetComponent<UIButton>(), "ClickCommand", new EventDelegate.Parameter[] { param });
	}

	public void SetActiveCommands()
	{
		if (deactive == true)
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

	private void DeactiveEvent(eCommand commandType, bool deactive)
	{
		if(this.gameObject.name == commandType.ToString())
		{
			this.deactive = deactive;
		}
	}

}
