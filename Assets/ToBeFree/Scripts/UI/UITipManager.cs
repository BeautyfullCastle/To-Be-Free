using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class UITipManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject tipPref;
		[SerializeField]
		private UIGrid grid;
		private List<UITip> list = new List<UITip>();
		
		public void Show(Tip tip)
		{
			UITip duplicatedUITip = list.Find(x => x.Tip.Timing == tip.Timing);
			if (duplicatedUITip)
				return;

			if(grid == null)
				return;
			if(tipPref == null)
				return;

			if (list.Count >= 3)
			{
				UITip deleteUITip = this.list[0];
				if (deleteUITip == null)
					return;

				this.list.Remove(deleteUITip);
				DestroyImmediate(deleteUITip.gameObject);
			}
			
			GameObject obj = Instantiate(tipPref) as GameObject;
			obj.transform.SetParent(this.grid.transform);
			obj.transform.localScale = Vector3.one;
			
			UITip uiTip = obj.GetComponent<UITip>();
			if (uiTip == null)
				return;

			uiTip.SetInfo(tip);
			this.list.Add(uiTip);

			
			grid.Reposition();
		}
	}
}
