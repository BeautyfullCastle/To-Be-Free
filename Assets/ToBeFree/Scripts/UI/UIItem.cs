using System;
using UnityEngine;

namespace ToBeFree
{
	public class UIItem : MonoBehaviour
	{
		public enum eBelong { SHOP, INVEN }

		public eBelong belong = eBelong.SHOP;

		public UILabel itemName;
		public UILabel itemPrice;
		public UILabel explanation;
		public UISprite sprite;
		public GameObject itemEffectObj;

		private Item item;

		private Color defaultColor;
		private Color hover;
		private Color pressed;
		private Color disabledColor;

		UIButtonEventSynchronizer synchronizer;

		void Awake()
		{
			itemName = transform.FindChild("Name").GetComponent<UILabel>();
			itemPrice = transform.FindChild("Price").GetComponent<UILabel>();

			sprite = transform.GetComponent<UISprite>();

			defaultColor = this.GetComponent<UIButton>().defaultColor;
			hover = this.GetComponent<UIButton>().hover;
			pressed = this.GetComponent<UIButton>().pressed;
			disabledColor = this.GetComponent<UIButton>().disabledColor;

			synchronizer = this.GetComponent<UIButtonEventSynchronizer>();
		}

		void Start()
		{
			if (belong == eBelong.SHOP)
			{
				return;
			}

			if(itemPrice != null)
			{
				itemPrice.enabled = false;
			}

			if (item == null)
			{
				return;
			}

			if(item.Buff == null)
			{
				return;
			}

			if (item.Buff.StartTime != eStartTime.NOW || item.Buff.Duration == eDuration.EQUIP)
			{
				SetEnable(false);
			}
		}

		void OnClick()
		{
			if (UICamera.currentTouchID == -2)
			{
				if (this.Item == null)
					return;

				if (this.enabled == false)
					return;

				if (belong == eBelong.SHOP)
				{
					transform.GetComponentInParent<UIShop>().OnClick(this);
				}
			}
		}

		void OnPress(bool pressed)
		{
			if (belong == eBelong.SHOP)
			{
				return;
			}

			if(UICamera.currentTouchID == -2 && pressed)
			{
				Debug.Log(this.name + " : right click in OnPress()");
				if (this.Item == null)
					return;

				if (this.enabled == false)
					return;

				if (belong == eBelong.INVEN)
				{
					GameObject effectObj = GameObject.Instantiate(itemEffectObj, this.transform.position, Quaternion.identity, GameManager.Instance.uiInventory.transform) as GameObject;
					GameObject.Destroy(effectObj, effectObj.GetComponent<ParticleSystem>().duration);

					StartCoroutine(GameManager.Instance.Character.Inven.UseItem(this.Item, GameManager.Instance.Character));
				}
				return;
			}

			TipManager.Instance.Show(eTipTiming.GrabItem);

			// 아이템을 누르고 있는 동안은 충돌체를 비활성화한다.
			GetComponent<Collider2D>().enabled = !pressed;

			// 아이템을 드롭하면,
			if (!pressed)
			{
				// UICamera가 감지한 충돌체를 찾는다.
				Collider col = UICamera.lastHit.collider;
				// 감지한 충돌체가 없거나, 드롭 영역이 아니면
				if (col == null || col.GetComponent<UITrashCan>() == null)
				{
					// 부모인 Grid를 찾아서
					UIGrid grid = NGUITools.FindInParents<UIGrid>(this.gameObject);
					// 원래 위치로 돌아온다.
					if (grid != null)
						grid.Reposition();
				}
			}
		}

		public void Refresh()
		{
			if (this.item == null)
				return;

			Item foundItem = ItemManager.Instance.GetByIndex(this.item.Index);
			if (foundItem == null)
				return;

			this.itemName.text = foundItem.Name;
			if(this.explanation)
			{
				this.explanation.text = foundItem.Buff.Script;
			}
		}

		void OnDrop(GameObject dropped)
		{
			UIDragDropMyItem droppedDragDropItem = dropped.GetComponent<UIDragDropMyItem>();
			if (droppedDragDropItem)
			{
				int tempCurrIndex = this.transform.GetSiblingIndex();
				
				UIInventory uiInventory = GameObject.FindObjectOfType<UIInventory>();
				//string droppedItemName = dropped.transform.FindChild("Name").GetComponent<UILabel>().text;
				UIItem droppedUIItem = uiInventory.GetByGridIndex(droppedDragDropItem.currSiblingIndex);

				this.transform.SetSiblingIndex(droppedDragDropItem.currSiblingIndex);
				droppedUIItem.transform.SetSiblingIndex(tempCurrIndex);

				// 부모인 Grid를 찾아서
				UIGrid grid = NGUITools.FindInParents<UIGrid>(this.gameObject);
				// 원래 위치로 돌아온다.
				if (grid != null)
					grid.Reposition();
			}
		}

		void OnTooltip(bool show)
		{
			if(this.belong == eBelong.SHOP)
			{
				UITooltip.Hide();
				return;
			}

			Item item = show ? this.item : null;
			if (item == null)
			{
				UITooltip.Hide();
				return;
			}

			string description = this.itemName.text + "\\n";
			description += this.item.Buff.Script;
			UITooltip.Show(description);
		}

		public void SetInfo(Item item)
		{
			if (item == null)
				return;

			this.Item = item;
			itemName.text = item.Name;
			itemPrice.text = item.Price.ToString();

			if (explanation)
			{
				explanation.text = this.item.Buff.Script;
			}

			if (sprite == null)
				return;

			string nameForSprite = item.EngName;
			UISpriteData spriteData = sprite.atlas.GetSprite("ITEM_" + nameForSprite);
			if(spriteData != null)
			{
				sprite.spriteName = spriteData.name;
				UIButton button = GetComponent<UIButton>();
				if(button)
				{
					button.normalSprite = sprite.spriteName;
				}
			}
		}

		public void SetEnable(bool isEnable)
		{
			if(this.item.Buff.StartTime == eStartTime.NOW)
			{
				this.enabled = true;
			}
			else
			{
				this.enabled = isEnable;
			}

			if (this.enabled)
			{
				this.GetComponent<UIButton>().defaultColor = defaultColor;
				this.GetComponent<UIButton>().hover = hover;
				this.GetComponent<UIButton>().pressed = pressed;
				this.GetComponent<UIButton>().disabledColor = disabledColor;
			}
			else
			{ 
				this.GetComponent<UIButton>().defaultColor = Color.gray;
				this.GetComponent<UIButton>().hover = Color.gray;
				this.GetComponent<UIButton>().pressed = Color.gray;
				this.GetComponent<UIButton>().disabledColor = Color.gray;
			}

			if (this.synchronizer == null)
			{
				return;
			}
			this.synchronizer.enabled = isEnable;
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