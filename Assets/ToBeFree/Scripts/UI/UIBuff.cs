using System;
using UnityEngine;

namespace ToBeFree
{
	public class UIBuff : MonoBehaviour
	{
		public UILabel nameLabel;
		public UILabel stackLabel;
		public UISprite sprite;
		public string uiScript;

		private Buff buff;

		public void SetInfo(Buff buff, bool isStack)
		{
			this.buff = buff;
			nameLabel.text = buff.Name;
			sprite.spriteName = "BUFF_" + AbnormalConditionManager.Instance.GetEngName(buff.Index);
			uiScript = buff.Script;
			stackLabel.enabled = isStack;
			if(stackLabel.enabled)
			{
				stackLabel.text = "1";
			}
		}

		void OnTooltip(bool show)
		{
			if (show == false)
			{
				UITooltip.Hide();
				return;
			}
			
			UITooltip.Show(uiScript);
		}

		public void Refresh()
		{
			AbnormalCondition ab = AbnormalConditionManager.Instance.GetByIndex(this.Index);
			if (ab == null)
				return;

			this.nameLabel.text = ab.Name;
			this.uiScript = ab.Buff.Script;
		}

		public int Index
		{
			get
			{
				return this.buff.Index;
			}
		}
	}
}