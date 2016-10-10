﻿using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
	public class UIItem : MonoBehaviour
	{
		public enum eBelong { SHOP, INVEN }

		public eBelong belong = eBelong.SHOP;

		public UILabel itemName;
		public UILabel itemPrice;

		private Item item;

		void Awake()
		{
			itemName = transform.FindChild("Name").GetComponent<UILabel>();
			itemPrice = transform.FindChild("Price").GetComponent<UILabel>();
		}
		void Start()
		{
			if(belong == eBelong.SHOP)
			{
				GetComponent<UIDragDropItem>().enabled = false;
			}
			else
			{
				itemPrice.enabled = false;
			}
		}
		
		void OnClick()
		{
			if (this.Item == null)
				return;

			if (belong == eBelong.SHOP)
			{
				transform.GetComponentInParent<UIShop>().OnClick(this);
			}
		}

		void OnPress(bool pressed)
		{
			if(belong != eBelong.INVEN)
			{
				return;
			}

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
					UIGrid grid = NGUITools.FindInParents<UIGrid>(gameObject);
					// 원래 위치로 돌아온다.
					if (grid != null) grid.Reposition();
				}
			}
		}

		void OnTooltip(bool show)
		{
			Item item = show ? this.item : null;            
			if (item==null)
			{
				UITooltip.Hide();
				return;
			}
			
			string description = this.itemName.text + "\\n";

			foreach(EffectAmount effectAmount in this.item.Buff.EffectAmountList)
			{
				description += effectAmount.ToString() + "\\n";
			}
			UITooltip.Show(description);
		}

		public void SetInfo(Item item)
		{
			if (item == null)
				return;

			this.Item = item;
			itemName.text = item.Name;
			itemPrice.text = item.Price.ToString();

			if (item.Buff.StartTime != eStartTime.NOW)
			{
				GetComponent<UIButton>().isEnabled = false;
			}
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