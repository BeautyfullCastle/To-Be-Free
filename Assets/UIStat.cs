using UnityEngine;
using System.Collections;

public class UIStat : MonoBehaviour {

	private UILabel label;

	// Use this for initialization
	void Start () {
		label = this.GetComponent<UILabel>();
		ToBeFree.Character.OnValueChange += OnValueChange;
	}
	
	void OnValueChange(int value)
	{
		label.text = value.ToString();
	}
}
