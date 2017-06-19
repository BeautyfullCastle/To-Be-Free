
using System;
using UnityEngine;

namespace ToBeFree
{
	public class UICharacterSelect : MonoBehaviour
	{
		[SerializeField]
		private UIGrid grid;
		[SerializeField]
		private GameObject iconPrefab;
		[SerializeField]
		private UISprite sprite;
		[SerializeField]
		private UILabel nameLabel;
		[SerializeField]
		private UILabel descriptionLabel;
		[SerializeField]
		private UILabel passiveSkillLabel;
		[SerializeField]
		private UILabel activeSkillLabel;
		[SerializeField]
		private UILabel hpLabel;
		[SerializeField]
		private UILabel satietyLabel;
		[SerializeField]
		private UILabel strengthLabel;
		[SerializeField]
		private UILabel agilityLabel;
		[SerializeField]
		private UILabel talentLabel;
		[SerializeField]
		private UILabel focusLabel;
		[SerializeField]
		private UILabel moneyLabel;

		[SerializeField]
		private UIInventory uiInventory;

		private Character character;

		public void Init()
		{
			foreach(Character cha in CharacterManager.Instance.List)
			{
				GameObject gObjItem = NGUITools.AddChild(grid.gameObject, iconPrefab);
				UICharacterIcon icon = gObjItem.GetComponentInChildren<UICharacterIcon>();
				icon.SetSprite(cha);
				grid.Reposition();
			}

			// TODO: 첫번째 아이콘 OnClick
			grid.GetChild(0).GetComponent<UICharacterIcon>().OnClick();
		}

		public void SetCharacterUI(Character character)
		{
			this.character = character;

			UISpriteData spriteData = sprite.atlas.GetSprite("Character_" + character.EngName);
			if (spriteData != null)
			{
				this.sprite.spriteName = spriteData.name;
			}
			else
			{
				this.sprite.spriteName = "white";
			}

			nameLabel.text = character.Name;
			descriptionLabel.text = character.Script;
			// 사용 능력
			activeSkillLabel.text = character.SkillScript;
			// 지속 능력
			AbnormalCondition ab = AbnormalConditionManager.Instance.GetByIndex(character.AbnormalIndex);
			if(ab != null)
			{
				passiveSkillLabel.text = ab.Buff.Script;
			}
			else
			{
				passiveSkillLabel.text = LanguageManager.Instance.Find(eLanguageKey.UI_NoAbility);
			}

			Stat stat = character.Stat;
			hpLabel.text = stat.HP.ToString();
			satietyLabel.text = stat.Satiety.ToString();
			strengthLabel.text = stat.Strength.ToString();
			agilityLabel.text = stat.Agility.ToString();
			talentLabel.text = stat.Talent.ToString();
			focusLabel.text = stat.Concentration.ToString();
			moneyLabel.text = stat.Money.ToString();

			uiInventory.Init(character.Inven, UIItem.eBelong.CHARACTERSELECT);
		}

		public void StartGame()
		{
			GameManager.Instance.Character = this.character;
			GameManager.Instance.State = GameManager.GameState.InGame;
		}
	}
}
