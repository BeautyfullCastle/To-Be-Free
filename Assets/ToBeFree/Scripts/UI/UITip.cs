using System;
using UnityEngine;

namespace ToBeFree
{
	public class UITip : MonoBehaviour
	{
		[SerializeField]
		private TweenPosition tweenTip;
		[SerializeField]
		private UILabel titleLabel;
		[SerializeField]
		private UILabel scriptLabel;
		[SerializeField]
		private UISprite sprite;
		[SerializeField]
		private UISprite blur;
		
		[SerializeField]
		private UILabel exclamationTitleLabel;
		[SerializeField]
		private TweenPosition tweenExclamation;

		private Tip firstTip;
		private Tip currentTip;
		
		public void Init()
		{
			tweenTip.ResetToBeginning();
			tweenExclamation.ResetToBeginning();
			this.blur.enabled = false;
			this.gameObject.SetActive(false);
		}
		
		public void SetInfo(Tip tip)
		{
			if (tip == null)
				return;

			this.firstTip = tip;
			this.currentTip = firstTip;

			RefreshExclamation();

			if (tip.Watched)
				return;

			tweenTip.ResetToBeginning();
			tweenTip.PlayForward();
			
			Refresh();
		}

		private void RefreshExclamation()
		{
			exclamationTitleLabel.text = this.firstTip.Title;
			tweenExclamation.ResetToBeginning();
			tweenExclamation.PlayForward();
		}

		public void Refresh()
		{
			if (currentTip == null)
				return;

			this.titleLabel.text = currentTip.Title;
			this.scriptLabel.text = currentTip.Script;
			UISpriteData spriteData = this.sprite.atlas.GetSprite(currentTip.SpriteName);
			if (spriteData != null)
			{
				this.sprite.spriteName = spriteData.name;
			}

			exclamationTitleLabel.text = this.firstTip.Title;

			blur.enabled = true;
			this.gameObject.SetActive(true);
			if (DiceTester.Instance.demo != null)
			{
				if (DiceTester.Instance.demo.gameObject.activeSelf)
				{
					DiceTester.Instance.demo.SetEnableCameras(false);
				}
			}
			GameManager.Instance.ChangeUICameraMask();
		}

		public void OnClickOK()
		{
			Tip nextTip =TipManager.Instance.GetByIndex(currentTip.NextIndex);
			if (nextTip == null)
			{
				blur.enabled = false;
				this.gameObject.SetActive(false);
				GameManager.Instance.ChangeUICameraMask();
				if (DiceTester.Instance.demo != null)
				{
					if (DiceTester.Instance.demo.gameObject.activeSelf)
					{
						DiceTester.Instance.demo.SetEnableCameras(true);
					}
				}
				return;
			}

			currentTip = nextTip;
			Refresh();
		}

		public void OnClickExclamanation()
		{
			this.currentTip = this.firstTip;

			tweenTip.ResetToBeginning();
			tweenTip.PlayForward();

			Refresh();
		}

		public Tip Tip
		{
			get
			{
				return firstTip;
			}
		}
	}
}