using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReplaceFont : MonoBehaviour
{
#if UNITY_EDITOR
	// Add a menu item named "Do Something" to MyMenu in the menu bar.
	[MenuItem("MyMenu/Replace All Label Fonts")]
	static void replaceAllFonts()
	{
		UILabel[] labels = GameObject.Find("World").GetComponentsInChildren<UILabel>();

		Debug.Log("We found :" + labels.Length + " labels.");

		Font go = Resources.Load("UnJamoBatang") as Font;

		if (go == null)
			Debug.LogWarning("We failed to load font.");
		else
		{
			Debug.Log("Loaded font.");

			foreach (UILabel l in labels)
			{
				l.trueTypeFont = go;
			}

			Debug.Log("success!");
		}
	}
#endif
}