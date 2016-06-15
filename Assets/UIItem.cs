using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
    public class UIItem : MonoBehaviour
    {
        public UILabel itemName;

        private Item item;
        
        void OnClick()
        {
            NGUIDebug.Log(itemName.text);

            // for the test of delte
            GameManager.Instance.Character.Inven.Delete(this.item, GameManager.Instance.Character);
        }

        internal void SetInfo(Item item)
        {
            this.item = item;
            itemName.text = item.Name;
        }
    }
}