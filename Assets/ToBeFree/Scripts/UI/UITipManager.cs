using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class UITipManager : MonoBehaviour
	{
		private UITip uiTip;

		void Awake()
		{
			uiTip = this.GetComponentInChildren<UITip>();
			uiTip.gameObject.SetActive(false);
		}

		public void Init()
		{
			uiTip.Init();
		}

		public void Show(Tip tip)
		{
			if (uiTip == null)
				return;

			uiTip.SetInfo(tip);
		}

		public void Refresh()
		{
			if (uiTip == null)
				return;

			uiTip.Refresh();
		}
	}
}
