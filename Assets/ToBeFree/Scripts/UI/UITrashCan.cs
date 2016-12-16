using UnityEngine;

namespace ToBeFree
{
	public class UITrashCan : MonoBehaviour
	{
		public GameObject droppedItemPrefab;

		[SerializeField]
		private UIShop shop;

		public void Start()
		{
			if(shop == null)
			{
				shop = GameObject.FindObjectOfType<UIShop>();
			}			
		}

		public void OnDrop(GameObject dropped)
		{
			// 드롭된 게임오브젝트에 Z_Item 컴포넌트가 있는지 확인하다.
			UIItem droppedItem = dropped.GetComponent<UIItem>();
			// 컴포넌트가 없다면, 즉 아이템이 아니라면 더 이상 진행할 필요가 없다.
			if (droppedItem == null) return;

			if (dropped.GetComponent<UIItem>().belong != UIItem.eBelong.INVEN)
				return;

			// 드롭된 아이템 프리팹의 인스턴스를 생성한다.
			//GameObject newPower = NGUITools.AddChild(this.gameObject,
			//										 droppedItemPrefab);
			// 드롭된 게임오브젝트는 삭제한다.
			if(gameObject.name == "TrashCan")
			{
				StartCoroutine(GameManager.Instance.Character.Inven.Delete(droppedItem.Item, GameManager.Instance.Character));
			}
			else
			{
				if(droppedItem.enabled)
					StartCoroutine(GameManager.Instance.Character.Inven.UseItem(droppedItem.Item, GameManager.Instance.Character));
			}

			shop.CheckAllItems();
		}
	}
}
