using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree {
    public class UIShop : MonoBehaviour {
        
        // 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
        public GameObject objSampleItem;
        
        public List<UIItem> basicItems = new List<UIItem>();
        private List<UIItem> cityItems = new List<UIItem>();
        private List<UIItem> randomItems = new List<UIItem>();

        // 그리드를 reset position 하기위해 선언합니다.
        private UIGrid[] grids;


        // Use this for initialization
        void Start()
        {            
            grids = GetComponentsInChildren<UIGrid>();

            basicItems[0].SetInfo(ItemManager.Instance.GetByIndex(0));

            foreach(UIGrid grid in grids)
            {
                grid.Reposition();
            }            
        }

        public void OnEnter()
        {
            this.gameObject.SetActive(true);
        }

        public void OnExit()
        {
            TimeTable.Instance.DayIsGone();
            EventManager.Instance.OnClickOK();
            this.gameObject.SetActive(false);
        }
    }
}