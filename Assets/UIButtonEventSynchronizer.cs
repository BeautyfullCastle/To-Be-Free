using UnityEngine;
using System.Collections;

public class UIButtonEventSynchronizer : MonoBehaviour {

	public UIWidget receiver;

	private UIButton button;
	
	// Use this for initialization
	void Awake () {
		button = this.GetComponent<UIButton>();
		this.GetOrAddComponent<TweenColor>();
	}

	void OnHover(bool isOver)
	{
		if (isOver)
			TweenColor.Begin(receiver.gameObject, button.duration, button.hover);
		else
			TweenColor.Begin(receiver.gameObject, button.duration, button.defaultColor);
	}
	
	void OnPress(bool isDown)
	{
		if (isDown)
			receiver.color = button.pressed;
		else
			receiver.color = button.defaultColor;
	}

	void OnEnable()
	{
		receiver.color = button.defaultColor;
	}

	void OnDisable()
	{
		receiver.color = button.disabledColor;
	}
}
