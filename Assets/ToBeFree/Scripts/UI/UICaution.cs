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
			if(GameManager.Instance.menuObj.activeSelf)
			{
				this.gameObject.layer = LayerMask.NameToLayer("Setting");
				this.GetComponent<UIPanel>().depth = 22;
			}
			else
			{
				this.gameObject.layer = LayerMask.NameToLayer("UI");
				this.GetComponent<UIPanel>().depth = 19;
			}
			this.transform.SetChildLayer(this.gameObject.layer);
			
			if(DiceTester.Instance.demo != null)
			{
				if(DiceTester.Instance.demo.gameObject.activeSelf)
				{
					DiceTester.Instance.demo.SetEnableCameras(false);
				}
			}
			
			scriptLabelChange.Refresh(key);
			this.gameObject.SetActive(true);
			while(bClicked == false)
			{
				yield return new WaitForSeconds(0.1f);
			}
			bClicked = false;

			if (DiceTester.Instance.demo != null)
			{
				if (DiceTester.Instance.demo.gameObject.activeSelf)
				{
					DiceTester.Instance.demo.SetEnableCameras(true);
				}
			}
			
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
