using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class SelectManager : Singleton<SelectManager>
	{
		private Select[] list;
		private SelectData[] dataList;
		private const string fileName = "/Select.json";
		private readonly string file = Application.streamingAssetsPath + fileName;

		private Language.SelectData[] engList;
		private Language.SelectData[] korList;
		private List<Language.SelectData[]> languageList;

		private Select currentSelect;

		public void Init()
		{
			DataList<SelectData> cDataList = new DataList<SelectData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Select[dataList.Length];

			engList = new DataList<Language.SelectData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.SelectData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.SelectData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			currentSelect = null;

			ParseData();
		}

		private void ParseData()
		{
			foreach (SelectData data in dataList)
			{
				Select select = new Select(EnumConvert<eSubjectType>.ToEnum(data.subjectType), EnumConvert<eObjectType>.ToEnum(data.objectType),
											data.comparisonOperator, data.compareAmount, data.script, 
											EnumConvert<eSelectLinkType>.ToEnum(data.linkType), data.linkIndex);

				list[data.index] = select;
			}
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.SelectData data in languageList[(int)language])
			{
				try
				{
					list[data.index].Script = data.script;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
			}
		}

		public IEnumerator WaitForSelect()
		{
			currentSelect = null;
			while(currentSelect == null)
			{
				yield return new WaitForSeconds(0.1f);
			}
			yield return OnClick(currentSelect);
		}

		public IEnumerator OnClick(Select select)
		{
			if(select.LinkType == eSelectLinkType.RESULT)
			{
				yield return EventManager.Instance.CalculateTestResult(select.Result.TestStat, GameManager.Instance.Character);
				yield return EventManager.Instance.TreatResult(select.Result, GameManager.Instance.Character);
			}
			else if(select.LinkType == eSelectLinkType.EVENT)
			{
				yield return EventManager.Instance.ActivateEvent(select.Event, GameManager.Instance.Character);
			}
			currentSelect = null;
		}

		public Select CurrentSelect
		{
			get
			{
				return currentSelect;
			}
			set
			{
				currentSelect = value;
			}
		}

		public Select GetByIndex(int index)
		{
			if (index < 0 || index  >= list.Length)
				return null;

			return list[index];
		}
	}
}
