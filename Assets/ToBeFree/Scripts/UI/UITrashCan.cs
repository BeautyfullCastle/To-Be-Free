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
			
			UIItem droppedUIItem = dropped.GetComponent<UIItem>();
			if (droppedUIItem == null)
				return;
			
			if (droppedUIItem.belong != UIItem.eBelong.INVEN)
				return;

			UIDragDropMyItem dragdropItem = droppedUIItem.GetComponent<UIDragDropMyItem>();
			if (dragdropItem == null)
				return;

			Item item = dragdropItem.item;
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
