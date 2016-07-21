using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
    public class UIItem : MonoBehaviour
    {
        public enum eBelong { SHOP, INVEN }

        public eBelong belong = eBelong.SHOP;

        public UILabel itemName;

        private Item item;
        
        void OnClick()
        {
            NGUIDebug.Log(itemName.text);

            if (belong == eBelong.SHOP)
            {
                if (this.Item == null)
                    return;

                StartCoroutine(GameManager.Instance.Character.Inven.BuyItem(this.Item, GameManager.Instance.Character));

                transform.GetComponentInParent<UIShop>().CheckCityItems();
            }
        }

        public void SetInfo(Item item)
        {
            if (item == null)
                return;

            this.Item = item;
            itemName.text = item.Name;
        }

        public Item Item
        {
            get
            {
                return this.item;
            }

            private set
            {
                this.item = value;
            }
        }
    }
}