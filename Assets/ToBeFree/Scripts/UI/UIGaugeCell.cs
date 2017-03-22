using System;
using UnityEngine;

public class UIGaugeCell : MonoBehaviour
{
	[SerializeField]
	private UISprite sprite;

	private TweenAlpha tweenAlpha;

	void Awake()
	{
		tweenAlpha = this.GetComponentInChildren<TweenAlpha>();
	}

	public void TurnOnSprite(bool isActive)
	{
		if (sprite == null)
			return;

		if(isActive)
		{
			tweenAlpha.PlayForward();
		}
		else
		{
			tweenAlpha.PlayReverse();
		}
	}

	public bool IsOn()
	{
		return this.sprite.alpha == 1;
	}

	public void ChangeSpritesParam(int size, Color color)
	{
		UISprite mySprite = this.GetComponent<UISprite>();
		ChangeSpriteParam(mySprite, size, color);
		ChangeSpriteParam(this.sprite, size, color);
	}

	private void ChangeSpriteParam(UISprite sprite, int size, Color color)
	{
		if (sprite == null)
			return;

		sprite.width = size;
		sprite.height = size;
		sprite.color = color;
	}
}
