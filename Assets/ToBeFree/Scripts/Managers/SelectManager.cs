using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class SelectManager : Singleton<SelectManager>
    {
        private readonly Select[] list;
        private readonly SelectData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Select.json";

        private int selectedIndex;

        public SelectManager()
        {
            DataList<SelectData> cDataList = new DataList<SelectData>(file);
            //SelectDataList cDataList = new SelectDataList(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Select[dataList.Length];

            selectedIndex = -99;

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
            selectedIndex = -99;
            while(selectedIndex == -99)
            {
                yield return new WaitForSeconds(0.1f);
            }
            yield return OnClick(list[selectedIndex]);
        }

        public IEnumerator OnClick(Select select)
        {
            if(select.LinkType == eSelectLinkType.RESULT)
            {
                yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);
                EventManager.Instance.CalculateTestResult(select.Result.TestStat, GameManager.Instance.Character);
                yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);                
                yield return EventManager.Instance.TreatResult(select.Result, GameManager.Instance.Character);
            }
            else if(select.LinkType == eSelectLinkType.EVENT)
            {
                yield return EventManager.Instance.ActivateEvent(select.Event, GameManager.Instance.Character);
            }
            yield return null;
        }

        public Select[] List
        {
            get
            {
                return list;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }

            set
            {
                selectedIndex = value;
            }
        }
    }
}
