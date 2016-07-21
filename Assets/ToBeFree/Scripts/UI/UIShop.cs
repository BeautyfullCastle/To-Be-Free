using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree {
    public class UIShop : MonoBehaviour {
        
        // 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
        public GameObject objSampleItem;
        
        public List<UIItem> basicItems = new List<UIItem>();
        public List<UIItem> cityItems;
        public List<UIItem> randomItems;

        // 그리드를 reset position 하기위해 선언합니다.
        private UIGrid[] grids;
        

        // Use this for initialization
        void Start()
        {            
            grids = GetComponentsInChildren<UIGrid>();

            basicItems[0].SetInfo(ItemManager.Instance.GetByIndex(0));
            basicItems[1].SetInfo(ItemManager.Instance.GetByIndex(63));
            
            foreach (UIGrid grid in grids)
            {
                grid.Reposition();
            }            
        }

        public void CheckCityItems()
        {
            if (GameManager.Instance.Character.Inven.Exist(GameManager.Instance.Character.CurCity.ItemList[0]))
            {
                cityItems[0].itemName.text = "재고 없음";
                cityItems[0].transform.GetComponent<UIButton>().isEnabled = false;
            }
            else
            {
                cityItems[0].SetInfo(GameManager.Instance.Character.CurCity.ItemList[0]);
                cityItems[0].transform.GetComponent<UIButton>().isEnabled = true;
            }
        }

        void OnEnable()
        {
            CheckCityItems();

            List<Item> randomItemList = new List<Item>(ItemManager.Instance.List);
            randomItemList.Remove(basicItems[0].Item);
            randomItemList.Remove(basicItems[1].Item);
            randomItemList.Remove(cityItems[0].Item);
            randomItemList.RemoveAll(x => GameManager.Instance.Character.Inven.Exist(x));

            System.Random r = new System.Random();
            int randIndex = r.Next(0, randomItemList.Count-1);

            randomItems[0].SetInfo(randomItemList[randIndex]);
        }

        public void OnExit()
        {
            TimeTable.Instance.DayIsGone();
            EventManager.Instance.OnClickOK();
            this.gameObject.SetActive(false);
        }
    }
}