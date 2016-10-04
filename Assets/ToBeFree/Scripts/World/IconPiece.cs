using UnityEngine;
using System;
using ToBeFree;

public class IconPiece : MonoBehaviour
{
	public eSubjectType subjectType;
	private UILabel powerLabel;
	private UILabel movementLabel;
	private UILabel numberLabel;

	public void Awake()
	{
		powerLabel = this.transform.FindChild("Power").GetComponent<UILabel>();
		movementLabel = this.transform.FindChild("Movement").GetComponent<UILabel>();
		numberLabel = this.transform.FindChild("Number").GetComponent<UILabel>();

		powerLabel.gameObject.SetActive(false);
		movementLabel.gameObject.SetActive(false);
		numberLabel.gameObject.SetActive(false);
	}

	public void Init(eSubjectType subjectType)
	{
		this.subjectType = subjectType;
		this.gameObject.name = subjectType.ToString();
		
		if (subjectType == eSubjectType.POLICE)
		{
			this.transform.localPosition = new Vector3(transform.localPosition.x - 30, transform.localPosition.y + 30);
		}
		else if(subjectType == eSubjectType.QUEST)
		{
			this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 30);
			this.GetComponent<UISprite>().spriteName = "Orc Armor - Boots";
		}
	}

	public void Init(eSubjectType subjectType, int power, int movement)
	{
		Init(subjectType);

		if(subjectType == eSubjectType.POLICE)
		{
			powerLabel.gameObject.SetActive(true);
			Power = power;

			movementLabel.gameObject.SetActive(true);
			Movement = movement;

			numberLabel.gameObject.SetActive(true);
			Number = 1;
		}
	}

	public int Power
	{
		get
		{
			return int.Parse(powerLabel.text);
		}
		set
		{
			powerLabel.text = value.ToString();
		}
	}

	public int Movement
	{
		get
		{
			return int.Parse(movementLabel.text);
		}
		set
		{
			movementLabel.text = value.ToString();
		}
	}

	public int Number
	{
		get
		{
			return int.Parse(numberLabel.text);
		}
		set
		{
			numberLabel.text = value.ToString();
		}
	}
}