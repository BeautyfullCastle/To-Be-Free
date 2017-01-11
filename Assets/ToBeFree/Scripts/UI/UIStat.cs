using UnityEngine;

namespace ToBeFree
{
	public class UIStat : MonoBehaviour
	{
		public eStat stat;
		private UILabel label;

		// Use this for initialization
		void Awake()
		{
			label = this.GetComponent<UILabel>();
			Stat.OnValueChange += OnValueChange;
			LanguageSelection.selectLanguage += LanguageSelection_selectLanguage;
		}

		private void LanguageSelection_selectLanguage(eLanguage language)
		{
			eLanguageKey key = eLanguageKey.UI_HP;
			if(stat == eStat.HP || stat == eStat.TOTALHP)
			{
				key = eLanguageKey.UI_HP;
			}
			else if (stat == eStat.SATIETY || stat == eStat.TOTALSATIETY)
			{
				key = eLanguageKey.UI_Satiety;
			}
			else if (stat == eStat.MONEY)
			{
				key = eLanguageKey.UI_Money;
			}
			else if (stat == eStat.INFO)
			{
				key = eLanguageKey.UI_Info;
			}
			else if (stat == eStat.STRENGTH)
			{
				key = eLanguageKey.UI_Strength;
			}
			else if (stat == eStat.AGILITY)
			{
				key = eLanguageKey.UI_Agility;
			}
			else if (stat == eStat.TALENT)
			{
				key = eLanguageKey.UI_Talent;
			}
			else if (stat == eStat.FOCUS)
			{
				key = eLanguageKey.UI_Focus;
			}
			this.transform.parent.GetComponent<UILabel>().text =  LanguageManager.Instance.Find(key);
		}

		void OnValueChange(int value, eStat stat)
		{
			if(stat != this.stat)
			{
				return;
			}

			label.text = value.ToString();
		}
	}
}