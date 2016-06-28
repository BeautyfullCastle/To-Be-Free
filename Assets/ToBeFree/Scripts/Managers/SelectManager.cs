using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class SelectManager : Singleton<SelectManager>
    {
        private readonly Select[] list;
        private readonly SelectData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Select.json";
        

        public SelectManager()
        {
            DataList<SelectData> cDataList = new DataList<SelectData>(file);
            //SelectDataList cDataList = new SelectDataList(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Select[dataList.Length];

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

        public Select[] List
        {
            get
            {
                return list;
            }
        }

        public void OnClick(string text)
        {
            Select select = Array.Find(list, x => x.Script == text);
            if(select.LinkType == eSelectLinkType.RESULT)
            {
                EventManager.Instance.TreatResult(select.Result);
            }
            else if(select.LinkType == eSelectLinkType.EVENT)
            {
                EventManager.Instance.ActivateEvent(select.Event, GameManager.Instance.Character);
            }
        }
    }
}
