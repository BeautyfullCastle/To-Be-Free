using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class UICaution : MonoBehaviour
	{
		[SerializeField]
		private UILabelLanguageChange scriptLabelChange;

		private bool bClicked = false;
		private bool bClickYes = false;

		void Awake()
		{
			this.gameObject.SetActive(false);
		}
		
		public IEnumerator Show(eLanguageKey key)
		{
			scriptLabelChange.Refresh(key);
			this.gameObject.SetActive(true);
			while(bClicked == false)
			{
				yield return new WaitForSeconds(0.1f);
			}
			bClicked = false;
			this.gameObject.SetActive(false);
		}

		public void OnClickYes()
		{
			bClicked = true;
			bClickYes = true;
		}

		public void OnClickNo()
		{
			bClicked = true;
			bClickYes = false;
		}

		public bool BClickYes
		{
			get
			{
				return bClickYes;
			}
		}
	}
}
