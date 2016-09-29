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

	public void Init(eSubjectType subjectType, Vector3 position)
	{
		this.subjectType = subjectType;
		this.gameObject.name = subjectType.ToString();
		this.transform.position = position;
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

	public void Init(eSubjectType subjectType, Vector3 position, int power, int movement)
	{
		Init(subjectType, position);

		if(subjectType == eSubjectType.POLICE)
		{
			powerLabel.gameObject.SetActive(true);
			powerLabel.text = power.ToString();

			movementLabel.gameObject.SetActive(true);
			movementLabel.text = movement.ToString();
		}
	}
}