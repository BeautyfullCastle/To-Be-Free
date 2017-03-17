using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class BlackFader : MonoBehaviour
	{
		public float duration;

		private UISprite sprite;
		private TweenAlpha tweenAlpha;

		void Awake()
		{
			this.sprite = this.GetComponent<UISprite>();
			this.tweenAlpha = this.GetComponent<TweenAlpha>();
			this.tweenAlpha.duration = this.duration;
		}

		public IEnumerator Fade(bool isIn)
		{
			if(isIn)
			{
				this.tweenAlpha.PlayForward();
			}
			else
			{
				this.tweenAlpha.PlayReverse();
				this.tweenAlpha.ResetToBeginning();
			}
			
			yield return new WaitForSeconds(this.duration);
		}
	}
}
