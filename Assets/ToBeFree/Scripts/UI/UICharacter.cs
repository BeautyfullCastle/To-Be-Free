using UnityEngine;

namespace ToBeFree
{
	public class UICharacter : MonoBehaviour
	{
		public UILabel nameLabel;
		public UISprite sprite;

		public void Refresh()
		{
			Character character = GameManager.Instance.Character;
			if (character == null)
				return;

			nameLabel.text = character.Name;

			UISpriteData spriteData = sprite.atlas.GetSprite("Character_" + character.EngName);
			if (spriteData == null)
				return;

			this.sprite.spriteName = spriteData.name;
		}
	}
}