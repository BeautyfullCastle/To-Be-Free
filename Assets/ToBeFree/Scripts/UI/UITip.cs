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

			if (this.scriptLabel == null)
				return;
			this.scriptLabel.text = tip.Script;
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