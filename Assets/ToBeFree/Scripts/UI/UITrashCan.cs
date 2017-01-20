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
			if (dropped == null)
				return;

			UIDragDropMyItem dragdropItem = dropped.GetComponent<UIDragDropMyItem>();
			if (dragdropItem == null)
				return;

			UIItem uiItem = dragdropItem.uiItem;
			if (uiItem == null)
				return;
			
			if (uiItem.belong != UIItem.eBelong.INVEN)
				return;

			// uiItem이 활성화되어있으면 폐기 불가
			if (uiItem.enabled)
				return;
			
			Item item = uiItem.Item;
			if (item == null)
				return;

			StartCoroutine(GameManager.Instance.Character.Inven.Delete(item, GameManager.Instance.Character, dragdropItem.currSiblingIndex));

			if (shop == null)
			{
				return;
			}
			shop.CheckAllItems();
		}
	}
}
