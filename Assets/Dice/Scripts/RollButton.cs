using UnityEngine;
using System.Collections;

public class RollButton : MonoBehaviour {

	public AppDemo demo;
	public UIProgressBar bar;

	private bool bClick = false;
	private float speed = 0.1f;

	public void OnButtonClick()
	{
		bClick = !bClick;

		if(bClick == false)
		{
			if(bar.value >= 0.7f && bar.value <= 0.9f)
			{
				demo.AddDie();
			}			
			demo.OnButtonClick();
			
		}
	}

	void OnDisable()
	{
		bar.value = 0;
	}

	void Update()
	{
		if(bClick)
		{
			if (bar.value == 0 || bar.value == 1)
				speed *= -1f;

			bar.value += speed * Time.timeScale;
		}
	}
}
