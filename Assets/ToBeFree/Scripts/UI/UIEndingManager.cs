using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eEnding
	{
		STARVATION = 0, REPATRIATE, HAPPY, END
	}

	[Serializable]
	public class UIEnding
	{
		public enum eEndingEffect
		{
			AUDIO,
			FADE_IN, FADE_OUT,
			FADE_COLOR_WHITE, FADE_COLOR_BLACK,
			MOVE_UP_DOWN, MOVE_RIGHT_LEFT,
			ZOOM_IN,
		}

		public eEndingEffect effect;

		public string audioName;
		public Texture texture;

		public float startTime;
		public float endTime;

		public float scale;

		public IEnumerator Play()
		{
			yield return new WaitForSeconds(startTime);

			switch(effect)
			{
				case eEndingEffect.AUDIO:
					AudioManager.Instance.ChangeBGM(audioName);
					break;
				case eEndingEffect.FADE_IN:
				case eEndingEffect.FADE_OUT:
					yield return GameManager.Instance.endingManager.ChangeMainTexture(texture, endTime - startTime, effect);
					break;
				case eEndingEffect.FADE_COLOR_BLACK:
					yield return GameManager.Instance.endingManager.ChangeBackgroundTextureColor(endTime - startTime, Color.white, Color.black);
					break;
				case eEndingEffect.FADE_COLOR_WHITE:
					yield return GameManager.Instance.endingManager.ChangeBackgroundTextureColor(endTime - startTime, Color.white, Color.black);
					break;
			}
		}
	}

	public class UIEndingManager : MonoBehaviour
	{
		[SerializeField]
		private UITexture page;
		[SerializeField]
		private UITexture background;

		// HP=0, 북송, 해피엔딩
		public Texture[] starvationTextures;
		public Texture[] repatriateTextures;
		public Texture[] happyTextures;

		public UIEnding[] happyEndingArr;

		void Awake()
		{
			this.page = this.GetComponentInChildren<UITexture>();
		}

		public IEnumerator StartEnding(eEnding ending)
		{
			yield return GameManager.Instance.ChangeScene(GameManager.eSceneState.Ending);
			
			switch(ending)
			{
				case eEnding.STARVATION:
					//AudioManager.Instance.ChangeBGM("GameOver");
					//yield return TurnPages(starvationTextures);
					yield return Play(happyEndingArr);
					break;
				case eEnding.REPATRIATE:
					//AudioManager.Instance.ChangeBGM("GameOver");
					//yield return TurnPages(repatriateTextures);
					yield return Play(happyEndingArr);
					break;
				case eEnding.HAPPY:
					AudioManager.Instance.ChangeBGM("HappyEnding"); 
					yield return TurnPages(happyTextures);
					break;
			}

			SaveLoadManager.Instance.Delete();
			yield return (GameManager.Instance.ChangeToMain());
		}

		private IEnumerator Play(UIEnding[] endings)
		{
			foreach(UIEnding ending in endings)
			{
				yield return (ending.Play());
			}
		}

		private IEnumerator TurnPages(Texture[] textures)
		{
			foreach(Texture texture in textures)
			{
				TweenColor.Begin(this.gameObject, 1f, Color.black);
				yield return new WaitForSeconds(1f);

				if(page != null)
				{
					page.mainTexture = texture;
				}

				TweenColor.Begin(this.gameObject, 1f, Color.white);
				yield return new WaitForSeconds(1f);
			}
		}

		public IEnumerator ChangeMainTexture(Texture texture, float duration, UIEnding.eEndingEffect effect)
		{
			if (texture == null)
				yield break;

			TweenAlpha tween = this.page.GetComponent<TweenAlpha>();
			if (tween == null)
				yield break;

			if (duration <= 0f)
				yield break;

			this.page.mainTexture = texture;

			tween.duration = duration;
			if (effect == UIEnding.eEndingEffect.FADE_IN)
			{
				tween.PlayForward();
			}
			else if(effect == UIEnding.eEndingEffect.FADE_OUT)
			{
				tween.PlayReverse();
			}
			yield return new WaitForSeconds(duration);
		}

		public IEnumerator ChangeBackgroundTextureColor(float duration, Color startColor, Color endColor)
		{
			TweenColor tweener = background.GetComponent<TweenColor>();
			if (tweener == null)
				yield break;

			if (duration <= 0f)
				yield break;

			if(startColor == endColor)
			{
				Debug.LogWarning("ChangeBackgroundTextureColor : start color and end color is same.");
			}

			tweener.duration = duration;
			tweener.from = startColor;
			tweener.to = endColor;

			tweener.PlayForward();
			yield return new WaitForSeconds(duration);
		}
	}
}