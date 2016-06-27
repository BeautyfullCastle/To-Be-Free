using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree
{
    public class UIInventory : MonoBehaviour
    {
        private List<UIItem> items = new List<UIItem>();
        // 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
        public GameObject objSampleItem;
        // 그리드를 reset position 하기위해 선언합니다.
        public UIGrid grid;

        void Awake()
        {
            Inventory.AddedItem += AddItem;
            Inventory.DeletedItem += DeleteItem;
        }

        private void AddItem(Item item)
        {
            GameObject gObjItem = NGUITools.AddChild(grid.gameObject, objSampleItem);
            // 이제 이름과 아이콘을 세팅할께요.
            // 그럴려면 먼저 아까 만든 ItemScript를 가져와야겠죠.
            UIItem itemScript = gObjItem.GetComponent<UIItem>();
            itemScript.SetInfo(item);
            // 이제 그리드와 스크롤뷰를 재정렬 시킵시다.
            grid.Reposition();
            // 그리고 관리를 위해 만든걸 리스트에 넣어둡시다.
            items.Add(itemScript);
        }

        private void DeleteItem(Item item)
        {
            UIItem uiItem = items.Find(x => x.itemName.text == item.Name);
            if(uiItem == null)
            {
                return;
            }
            DestroyImmediate(uiItem.gameObject);
            items.Remove(uiItem);
            grid.Reposition();
        }
    }
}