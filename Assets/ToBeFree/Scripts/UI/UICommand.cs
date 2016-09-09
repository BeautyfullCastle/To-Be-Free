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
			this.gameObject.SetActive(false);
		}
		else
		{
			if (this.name == "QUEST" || this.name == "INFO" || this.name == "BROKER")
			{
				this.gameObject.SetActive(PieceManager.Instance.GetNumberOfPiece(EnumConvert<eSubjectType>.ToEnum(this.name), GameManager.Instance.Character.CurCity) > 0);
			}
			else
			{
				this.gameObject.SetActive(true);
			}

			if (this.name == "QUEST" && this.gameObject.activeSelf)
			{
				QuestPiece piece = PieceManager.Instance.Find(eSubjectType.QUEST, GameManager.Instance.Character.CurCity) as QuestPiece;
				Quest quest = piece.CurQuest;
				this.GetComponent<UIButton>().isEnabled = quest.CheckCondition(GameManager.Instance.Character);
			}

			// one command can use in one day.
			this.gameObject.SetActive(GameManager.Instance.Character.CanAction[(int)EnumConvert<eCommand>.ToEnum(this.gameObject.name)]);
		}

		//if(GameManager.Instance.Character.CurCity.Area == eArea.MONGOLIA)
		//{
		//	if(!(this.commandType == eCommand.MOVE || this.commandType == eCommand.REST))
		//	{
		//		this.GetComponent<UIButton>().isEnabled = false;
		//	}
		//}

		if(this.GetComponent<UIButton>().isEnabled == false)
		{
			//if (GameManager.Instance.Character.CurCity.Area != eArea.MONGOLIA)
			{
				this.GetComponent<UIButton>().isEnabled = true;
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
