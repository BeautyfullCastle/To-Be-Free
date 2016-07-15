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
                if (this.item == null)
                    return;

                StartCoroutine(GameManager.Instance.Character.Inven.BuyItem(this.item, GameManager.Instance.Character));
            }
        }

        public void SetInfo(Item item)
        {
            if (item == null)
                return;

            this.item = item;
            itemName.text = item.Name;
        }
    }
}