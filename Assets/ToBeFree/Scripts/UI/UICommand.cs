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

	}

	public void SetActiveCommands()
	{
		if (skippedDay < 2)
		{
			this.GetComponent<UIButton>().isEnabled = false;
		}
		else
		{
			if (this.name == "QUEST" || this.name == "INFO" || this.name == "BROKER")
			{
				this.GetComponent<UIButton>().isEnabled = PieceManager.Instance.GetNumberOfPiece(EnumConvert<eSubjectType>.ToEnum(this.name), GameManager.Instance.Character.CurCity) > 0;
			}
			else
			{
				this.GetComponent<UIButton>().isEnabled = true;
			}

			if (this.name == "QUEST" && GetComponent<UIButton>().isEnabled)
			{
				QuestPiece piece = PieceManager.Instance.Find(eSubjectType.QUEST, GameManager.Instance.Character.CurCity) as QuestPiece;
				Quest quest = piece.CurQuest;
				this.GetComponent<UIButton>().isEnabled = quest.CheckCondition(GameManager.Instance.Character);
			}

			if(this.name == "SHOP")
			{
				this.GetComponent<UIButton>().isEnabled = (GameManager.Instance.Character.CurCity.Type == ToBeFree.eNodeType.BIGCITY
										|| GameManager.Instance.Character.CurCity.Type == ToBeFree.eNodeType.MIDDLECITY);
			}

			// one command can use in one day.
			if (this.GetComponent<UIButton>().isEnabled)
			{
				this.GetComponent<UIButton>().isEnabled = GameManager.Instance.Character.CanAction[(int)EnumConvert<eCommand>.ToEnum(this.gameObject.name)];
			}
		}

		//if(this.GetComponent<UIButton>().isEnabled == false)
		//{
		//	this.GetComponent<UIButton>().isEnabled = true;
		//}
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
