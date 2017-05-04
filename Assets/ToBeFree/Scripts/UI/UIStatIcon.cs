using UnityEngine;
using ToBeFree;

public class UIStatIcon : MonoBehaviour
{
	[SerializeField]
	private eLanguageKey type;
	private string tooltip;

	// Use this for initialization
	void Awake()
	{
		ToBeFree.LanguageSelection.selectLanguage += ChangeLanguage;
	}
	
	public void ChangeLanguage(eLanguage language)
	{
		tooltip = LanguageManager.Instance.Find(type);
	}

	void OnTooltip(bool show)
	{
		if (tooltip == string.Empty || show == false)
		{
			UITooltip.Hide();
			return;
		}

		UITooltip.Show(tooltip);
	}
}