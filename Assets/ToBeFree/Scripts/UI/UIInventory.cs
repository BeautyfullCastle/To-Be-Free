using UnityEngine;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
	public class UIInventory : MonoBehaviour
	{
		private List<UIItem> items;
		// 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
		public GameObject objSampleItem;
		// 그리드를 reset position 하기위해 선언합니다.
		public UIGrid grid;

		void Awake()
		{
			items = new List<UIItem>();
		}

		public void Init(Inventory inven)
		{
			OnDisable();

			foreach(Item item in inven.list)
			{
				AddItem(item);
			}
			this.Refresh();
		}

		void OnDisable()
		{
			if (items.Count == 0)
				return;

			foreach (UIItem uiItem in items)
			{
				DestroyImmediate(uiItem.gameObject);
			}
			items.Clear();
			grid.Reposition();
		}

		public void AddItem(Item item)
		{
			GameObject gObjItem = NGUITools.AddChild(grid.gameObject, objSampleItem);
			// 이제 이름과 아이콘을 세팅할께요.
			// 그럴려면 먼저 아까 만든 ItemScript를 가져와야겠죠.
			UIItem itemScript = gObjItem.GetComponent<UIItem>();
			itemScript.SetInfo(item, false);
			itemScript.belong = UIItem.eBelong.INVEN;

			// 이제 그리드와 스크롤뷰를 재정렬 시킵시다.
			grid.Reposition();
			// 그리고 관리를 위해 만든걸 리스트에 넣어둡시다.
			items.Add(itemScript);
		}

		public void DeleteItem(Item item)
		{
			UIItem uiItem = items.Find(x => x.Item.Index == item.Index);
			if(uiItem == null)
			{
				return;
			}
			this.DeleteItem(uiItem);
		}

		public void DeleteItem(UIItem uiItem)
		{
			if (uiItem == null)
			{
				return;
			}
			items.Remove(uiItem);
			DestroyImmediate(uiItem.gameObject);

			grid.Reposition();
		}

		public List<UIItem> FindAll(Item item)
		{
			return this.items.FindAll(x => x.Item.Index == item.Index);
		}

		public UIItem GetByGridIndex(int currSiblingIndex)
		{
			return this.items.Find(x => x.transform.GetSiblingIndex() == currSiblingIndex);
		}

		public void Refresh()
		{
			foreach(UIItem uiItem in items)
			{
				uiItem.Refresh();
			}
		}
	}
}