using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
	public class UIBuff : MonoBehaviour
	{
		public UILabel buffName;
		public UILabel amount;
		public string uiScript;

		private Buff buff;

		public void SetInfo(Buff buff)
		{
			this.buff = buff;
			buffName.text = buff.Name;
			uiScript = buff.Script;
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