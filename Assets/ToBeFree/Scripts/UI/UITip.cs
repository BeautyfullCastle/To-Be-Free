using System;
using UnityEngine;

namespace ToBeFree
{
	public class UITip : MonoBehaviour
	{
		[SerializeField]
		private UILabel scriptLabel;

		private Tip tip;
		
		public void SetInfo(Tip tip)
		{
			this.tip = tip;

			SetScriptLabel(tip.Script);
		}
		
		public void Refresh()
		{
			this.SetScriptLabel(this.tip.Script);
		}

		private void SetScriptLabel(string script)
		{
			if (this.scriptLabel == null)
				return;
			this.scriptLabel.text = script;
		}

		public Tip Tip
		{
			get
			{
				return tip;
			}
		}
	}
}