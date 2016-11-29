﻿using UnityEngine;

namespace ToBeFree
{
	public enum eUIEventLabelType
	{
		EVENT, RESULT, RESULT_EFFECT, DICENUM
	}

	public class UIEventManager : MonoBehaviour
	{
		public UILabel eventScript;
		public UILabel resultScript;
		public UILabel[] selectLabels;
		public UILabel resultEffectScript;
		public UILabel diceNum;
		public UIButton okButton;
		
		private UILabel[] allLabel;
		private Select[] selectList;
		
		private void Awake()
		{
			this.gameObject.SetActive(true);
		}

		public void Start()
		{
			allLabel = new UILabel[7];
			allLabel[0] = eventScript;
			allLabel[1] = resultScript;
			allLabel[2] = resultEffectScript;
			allLabel[3] = diceNum;
			allLabel[4] = selectLabels[0];
			allLabel[5] = selectLabels[1];
			allLabel[6] = selectLabels[2];

			EventManager.UIChanged += OnChanged;
			EventManager.SelectUIChanged += OnSelectUIChanged;
			EventManager.UIOpen += OpenUI;

			ClearAll();
			gameObject.SetActive(false);
		}

		public void OpenUI()
		{
			gameObject.SetActive(true);
			ClearAll();
		}

		public void OnChanged(eUIEventLabelType type, string text)
		{
			gameObject.SetActive(true);
			switch (type)
			{
				case eUIEventLabelType.EVENT:
					eventScript.text = text;
					break;
				case eUIEventLabelType.RESULT:
					resultScript.text = text;
					break;
				case eUIEventLabelType.RESULT_EFFECT:
					resultEffectScript.text = text;
					break;
				case eUIEventLabelType.DICENUM:
					diceNum.text = text;
					break;
				default:
					break;
			}
		}

		public void OnSelectUIChanged(Select[] selectList)
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