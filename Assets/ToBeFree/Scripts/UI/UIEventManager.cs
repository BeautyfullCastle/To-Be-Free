using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class UIEventManager : MonoBehaviour
	{
		public UILabel eventScript;
		public UILabel[] selectLabels;
		public UIButton okButton;
		
		private UILabel[] allLabel;
		private Select[] selectList;
		
		private void Awake()
		{
			this.gameObject.SetActive(true);
		}

		public void Reset()
		{
			allLabel = new UILabel[4];
			allLabel[0] = eventScript;
			allLabel[1] = selectLabels[0];
			allLabel[2] = selectLabels[1];
			allLabel[3] = selectLabels[2];

			ClearAll();
			gameObject.SetActive(false);
		}

		public IEnumerator OnChanged(string text, bool isNew = true, bool waitOk = true)
		{
			if(isNew)
			{
				gameObject.SetActive(true);
				ClearAll();

				eventScript.text = text;
			}
			else
			{
				eventScript.text += ("\n" + text);
			}

			if (waitOk)
				yield return EventManager.Instance.WaitUntilFinish();
			else
				yield return null;
		}

		public IEnumerator OnSelectUIChanged(Select[] selectList)
		{
			this.selectList = selectList;

			for (int i = 0; i < selectLabels.Length; ++i)
			{
				selectLabels[i].text = string.Empty;
			}

			for (int i=0; i<selectList.Length; ++i)
			{
				selectLabels[i].text = selectList[i].Script;
				selectLabels[i].GetComponent<UIButton>().isEnabled = selectList[i].CheckCondition(GameManager.Instance.Character);
			}

			yield return SelectManager.Instance.WaitForSelect();
		}
		
		public void OnClick(string index)
		{
			int iIndex = int.Parse(index);
			if(selectList == null || iIndex > selectList.Length-1)
			{
				return;
			}
			Select select = selectList[int.Parse(index)];
			if (select == null)
			{
				return;
			}

			ClearAll();
			SelectManager.Instance.CurrentSelect = select;
		}

		public void OnClickOK()
		{
			ClearAll();
			EventManager.Instance.OnClickOK();
			gameObject.SetActive(false);
		}

		public void ClearAll()
		{
			if (allLabel == null)
				return;

			for (int i = 0; i < allLabel.Length; ++i)
			{
				allLabel[i].text = string.Empty;
			}
			selectList = null;
		}
	}
}