using UnityEngine;

namespace ToBeFree
{
	public class UIDragDropMyItem : UIDragDropItem
	{
		[HideInInspector]
		public int currSiblingIndex;
		[HideInInspector]
		public UIItem uiItem;

		/// <summary>
		/// Called on the cloned object when it was duplicated.
		/// </summary>
		protected override void OnClone(GameObject original)
		{
			currSiblingIndex = original.transform.GetSiblingIndex();
			UIItem originUIItem = original.GetComponent<ToBeFree.UIItem>();
			if (originUIItem == null)
				return;
			this.uiItem = originUIItem;
		}
	}
}
