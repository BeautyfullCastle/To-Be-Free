using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class BlackFader : MonoBehaviour
	{
		public float duration;
		
		private TweenAlpha tweenAlpha;

		void Awake()
		{
			this.tweenAlpha = this.GetComponent<TweenAlpha>();
			this.tweenAlpha.duration = this.duration;
		}

		public IEnumerator Fade(bool isIn)
		{
			if(this.tweenAlpha == null)
			{
				this.Awake();
			}

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
