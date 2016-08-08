using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree {
    public class UIShop : MonoBehaviour {
        
        // 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
        public GameObject objSampleItem;
        
        public List<UIItem> basicItems = new List<UIItem>();
        public List<UIItem> cityItems;
        public List<UIItem> randomItems;

        // 그리드를 reset position 하기위해 선언합니다.
        private UIGrid[] grids;

        private int discountNum;
        
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
        
        void OnEnable()
        {
            if (GameManager.Instance.Character.Inven.Exist(GameManager.Instance.Character.CurCity.ItemList[0]))
            {
                cityItems[0].transform.GetComponent<UIButton>().isEnabled = false;
            }
            else
            {
                cityItems[0].SetInfo(GameManager.Instance.Character.CurCity.ItemList[0]);
                cityItems[0].transform.GetComponent<UIButton>().isEnabled = true;
            }


            List<Item> randomItemList = new List<Item>(ItemManager.Instance.List);
            randomItemList.Remove(basicItems[0].Item);
            randomItemList.Remove(basicItems[1].Item);
            randomItemList.Remove(cityItems[0].Item);
            randomItemList.RemoveAll(x => GameManager.Instance.Character.Inven.Exist(x));

            System.Random r = new System.Random();
            for (int i = 0; i < 3; ++i)
            {
                int randIndex = r.Next(0, randomItemList.Count - 1);
                randomItems[i].SetInfo(randomItemList[randIndex]);
                randomItems[i].transform.GetComponent<UIButton>().isEnabled = true;
                randomItemList.Remove(randomItems[i].Item);
            }
        }

        // click to buy item
        public void OnClick(UIItem uiItem)
        {
            if (uiItem == randomItems[0])
            {
                if (randomItems[0].Item != null && GameManager.Instance.Character.Inven.Exist(randomItems[0].Item))
                {
                    return;
                }
                StartCoroutine(GameManager.Instance.Character.Inven.BuyItem(uiItem.Item, discountNum, GameManager.Instance.Character));
                randomItems[0].transform.GetComponent<UIButton>().isEnabled = false;
            }
            else if (uiItem == cityItems[0])
            {
                if (GameManager.Instance.Character.Inven.Exist(GameManager.Instance.Character.CurCity.ItemList[0]))
                {
                    return;
                }
                else
                {
                    StartCoroutine(GameManager.Instance.Character.Inven.BuyItem(uiItem.Item, discountNum, GameManager.Instance.Character));                    
                    cityItems[0].transform.GetComponent<UIButton>().isEnabled = false;
                }
            }
            else
            {
                StartCoroutine(GameManager.Instance.Character.Inven.BuyItem(uiItem.Item, discountNum, GameManager.Instance.Character));
            }
        }

        public void CheckItems()
        {
            if (GameManager.Instance.Character.Inven.Exist(GameManager.Instance.Character.CurCity.ItemList[0]))
            {
                cityItems[0].transform.GetComponent<UIButton>().isEnabled = false;
            }
            else
            {
                cityItems[0].transform.GetComponent<UIButton>().isEnabled = true;
            }

            if (randomItems[0].Item != null && GameManager.Instance.Character.Inven.Exist(randomItems[0].Item))
            {
                randomItems[0].transform.GetComponent<UIButton>().isEnabled = false;
            }
            else
            {
                randomItems[0].transform.GetComponent<UIButton>().isEnabled = true;
            }
        }

        public void OnExit()
        {
            TimeTable.Instance.DayIsGone();
            EventManager.Instance.OnClickOK();
            this.gameObject.SetActive(false);
        }

        public int DiscountNum
        {
            get
            {
                return discountNum;
            }

            set
            {
                discountNum = value;
            }
        }
    }
}