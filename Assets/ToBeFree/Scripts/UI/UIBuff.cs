using UnityEngine;

namespace ToBeFree
{
	public class UIBuff : MonoBehaviour
	{
		public UILabel nameLabel;
		public UILabel stackLabel;
		public string uiScript;

		private Buff buff;

		public void SetInfo(Buff buff, bool isStack)
		{
			this.buff = buff;
			nameLabel.text = buff.Name;
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
	}
}