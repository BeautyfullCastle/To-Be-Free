using System;
using ToBeFree;
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
		
		if(GameManager.Instance.State != GameManager.GameState.StartDay)
		{
			AudioManager.Instance.Find("gauge_move").Play();
		}
	}

	public bool IsOn()
	{
		return this.sprite.alpha == 1;
	}

	public void ChangeSpritesParam(int width, int height, Color color)
	{
		UISprite mySprite = this.GetComponent<UISprite>();
		ChangeSpriteParam(mySprite, width, height, color);
		ChangeSpriteParam(this.sprite, width, height, color);
	}

	private void ChangeSpriteParam(UISprite sprite, int width, int height, Color color)
	{
		if (sprite == null)
			return;

		sprite.width = width;
		sprite.height = height;
		sprite.color = color;
	}
}
