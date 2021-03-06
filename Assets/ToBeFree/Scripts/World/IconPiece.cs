﻿using UnityEngine;
using ToBeFree;
using System;
using System.Collections;

public class IconPiece : MonoBehaviour
{
	public eSubjectType subjectType;
	[HideInInspector]
	public TweenAlpha exclamationAlpha;

	private UILabel powerLabel;
	private UILabel movementLabel;
	

	public void Awake()
	{
		powerLabel = this.transform.FindChild("Power").GetComponent<UILabel>();
		movementLabel = this.transform.FindChild("Movement").GetComponent<UILabel>();
		exclamationAlpha = this.transform.FindChild("Exclamation").GetComponent<TweenAlpha>();

		powerLabel.gameObject.SetActive(false);
		movementLabel.gameObject.SetActive(false);
	}

	void OnDisable()
	{
		//Destroy(this.gameObject, 0.1f);
	}

	public void Init(eSubjectType subjectType)
	{
		this.subjectType = subjectType;
		this.gameObject.name = EnumConvert<eSubjectType>.ToString(subjectType);

		UISprite sprite = this.GetComponent<UISprite>();

		if (sprite == null)
			return;

		if (subjectType == eSubjectType.POLICE)
		{
			//this.transform.localPosition = new Vector3(transform.localPosition.x - 30, transform.localPosition.y + 30);
		}
		else if(subjectType == eSubjectType.QUEST)
		{
			//this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 30);
			sprite.spriteName = "quest";
			sprite.width = 32;
			sprite.height = 32;
		}
		else if(subjectType == eSubjectType.BROKER)
		{
			sprite.spriteName = "broker";
			sprite.width = 32;
			sprite.height = 32;
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
			
			if(GameManager.Instance.Character != null)
			{
				GameManager.Instance.Character.Stat.SetViewRange();
			}

			PlayExclamation();
		}
	}

	public void PlayExclamation()
	{
		if (this.gameObject.activeSelf)
			StartCoroutine(TweenExclamation());
	}

	private IEnumerator TweenExclamation()
	{
		exclamationAlpha.PlayForward();
		yield return new WaitForSeconds(exclamationAlpha.duration);
		//exclamationAlpha.PlayReverse();
		//yield return new WaitForSeconds(exclamationAlpha.duration);
		exclamationAlpha.value = exclamationAlpha.from;
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
}