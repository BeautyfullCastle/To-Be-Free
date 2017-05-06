using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class UIEndingComponent : MonoBehaviour
	{
		[SerializeField]
		private string audioName;

		private UITexture texture;
		private TweenAlpha tweenAlpha;
		private TweenColor tweenColor;
		private TweenPosition tweenPosition;
		private TweenScale tweenScale;
		
		private Vector3 pos;

		public void Init()
		{
			this.texture = this.GetComponent<UITexture>();

			this.tweenAlpha = this.GetComponent<TweenAlpha>();
			this.tweenColor = this.GetComponent<TweenColor>();
			this.tweenPosition = this.GetComponent<TweenPosition>();
			this.tweenScale = this.GetComponent<TweenScale>();

			this.pos = this.transform.position;

			if (this.texture)
			{
				if(this.gameObject.name == "Background")
				{
					this.texture.enabled = true;
					this.texture.color = new Color(0f, 0f, 0f, 1f);
				}
				else
				{
					this.texture.enabled = false;
					this.texture.color = new Color(1f, 1f, 1f, 0f);
				}
			}
			if(this.tweenAlpha)
			{
				this.tweenAlpha.enabled = false;
			}
			if(this.tweenColor)
			{
				this.tweenColor.enabled = false;
			}
			if(this.tweenPosition)
			{
				this.tweenPosition.enabled = false;
			}
			if(this.tweenScale)
			{
				this.tweenScale.enabled = false;
			}
			//this.transform.position = pos;
		}

		public IEnumerator Play(eEndingEffect effect, float startTime, float endTime)
		{
			yield return new WaitForSeconds(startTime);

			switch (effect)
			{
				case eEndingEffect.AUDIO:
					AudioManager.Instance.ChangeBGM(AudioName);
					break;
				case eEndingEffect.FADE_IN:
					yield return Fade(tweenAlpha, endTime - startTime, true);
					break;
				case eEndingEffect.FADE_OUT:
					yield return Fade(tweenAlpha, endTime - startTime, false);
					break;
				case eEndingEffect.FADE_COLOR_BLACK:
					yield return Fade(tweenColor, endTime - startTime, true);
					break;
				case eEndingEffect.FADE_COLOR_WHITE:
					yield return Fade(tweenColor, endTime - startTime, false);
					break;
				case eEndingEffect.MOVE:
					yield return Fade(tweenPosition, endTime - startTime, true);
					break;
				case eEndingEffect.ZOOM_IN:
					yield return Fade(tweenScale, endTime - startTime, true);
					break;
			}
		}

		private IEnumerator Fade(UITweener tweener, float duration, bool playForward)
		{
			if (this.texture == null)
				yield break;

			if (tweener == null)
				yield break;

			if (duration <= 0f)
				yield break;

			this.texture.enabled = true;
			tweener.duration = duration;
			if(playForward)
			{
				tweener.PlayForward();
			}
			else
			{
				tweener.PlayReverse();
			}

			yield return new WaitForSeconds(duration);
		}

		public UITexture Texture
		{
			get
			{
				return texture;
			}
		}

		public string AudioName
		{
			get
			{
				return audioName;
			}
		}
	}
}