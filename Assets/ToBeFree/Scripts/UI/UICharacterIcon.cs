using System;
using UnityEngine;

namespace ToBeFree
{
	public class UICharacterIcon : MonoBehaviour
	{
		[SerializeField]
		private UISprite illustration;
		[SerializeField]
		private UISprite border;

		private Character character;

		public void SetSprite(Character character)
		{
			this.character = character;

			UISpriteData spriteData = illustration.atlas.GetSprite("Character_" + character.EngName);
			if (spriteData != null)
			{
				this.illustration.spriteName = spriteData.name;
			}
			else
			{
				this.illustration.spriteName = "white";
			}
		}

		public void OnClick()
		{
			foreach(UICharacterIcon icon in this.transform.parent.parent.GetComponentsInChildren<UICharacterIcon>())
			{
				icon.border.alpha = 0f;
			}
			this.border.alpha = 1f;

			GameManager.Instance.uiCharacterSelect.SetCharacterUI(this.character);
		}
	}
}
