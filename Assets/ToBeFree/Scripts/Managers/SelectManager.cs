using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class SelectManager : Singleton<SelectManager>
	{
		private readonly Select[] list;
		private readonly SelectData[] dataList;
		private readonly string file = Application.streamingAssetsPath + "/Select.json";
		
		private Select currentSelect;

		public SelectManager()
		{
			DataList<SelectData> cDataList = new DataList<SelectData>(file);
			//SelectDataList cDataList = new SelectDataList(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Select[dataList.Length];

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
				yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);
				yield return EventManager.Instance.CalculateTestResult(select.Result.TestStat, GameManager.Instance.Character);
				yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);                
				yield return EventManager.Instance.TreatResult(select.Result, GameManager.Instance.Character);
			}
			else if(select.LinkType == eSelectLinkType.EVENT)
			{
				yield return EventManager.Instance.ActivateEvent(select.Event, GameManager.Instance.Character);
			}
			currentSelect = null;
		}

		public Select[] List
		{
			get
			{
				return list;
			}
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
	}
}
