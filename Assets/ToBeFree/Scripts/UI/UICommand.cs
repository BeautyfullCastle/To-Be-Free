using UnityEngine;
using ToBeFree;

public class UICommand : MonoBehaviour
{
	[SerializeField]
	private eCommand commandType;
	private bool deactive;
	private UILabel nameLabel;
	private string tooltip;

	// Use this for initialization
	void Awake () {
		Effect.DeactiveEvent += DeactiveEvent;
		ToBeFree.LanguageSelection.selectLanguage += ChangeLanguage;

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
		if(this.commandType == commandType)
		{
			this.deactive = deactive;
		}
	}

	public void ChangeLanguage(eLanguage language)
	{
		eLanguageKey nameKey = eLanguageKey.UI_Move;
		eLanguageKey tooltipKey = eLanguageKey.Over_Move;
		if (commandType == eCommand.MOVE)
		{
			nameKey = eLanguageKey.UI_Move;
			tooltipKey = eLanguageKey.Over_Move;
		}
		else if (commandType == eCommand.WORK)
		{
			nameKey = eLanguageKey.UI_Work;
			tooltipKey = eLanguageKey.Over_Work;
		}
		else if (commandType == eCommand.INVESTIGATION)
		{
			nameKey = eLanguageKey.UI_Inquiry;
			tooltipKey = eLanguageKey.Over_Inquiry;
		}
		else if (commandType == eCommand.REST)
		{
			nameKey = eLanguageKey.UI_Rest;
			tooltipKey = eLanguageKey.Over_Rest;
		}
		else if (commandType == eCommand.SHOP)
		{
			nameKey = eLanguageKey.UI_Shop;
			tooltipKey = eLanguageKey.Over_Shop;
		}
		else if (commandType == eCommand.BROKER)
		{
			nameKey = eLanguageKey.UI_Broker;
			tooltipKey = eLanguageKey.Over_Broker;
		}
		else if(commandType == eCommand.QUEST)
		{
			nameKey = eLanguageKey.UI_Quest;
			tooltipKey = eLanguageKey.Over_Quest;
		}
		else if(commandType == eCommand.ABILITY)
		{
			nameKey = eLanguageKey.UI_Abilty;
			tooltipKey = eLanguageKey.Over_Abilty;
		}
		nameLabel.text = LanguageManager.Instance.Find(nameKey);
		tooltip = LanguageManager.Instance.Find(tooltipKey);
	}

	void OnTooltip(bool show)
	{
		if (tooltip == string.Empty || show == false)
		{
			UITooltip.Hide();
			return;
		}

		UITooltip.Show(tooltip);
	}
}