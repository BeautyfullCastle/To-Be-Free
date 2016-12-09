using UnityEngine;
using System.Collections;
using ToBeFree;

public class RollButton : MonoBehaviour {

	public AppDemo demo;
	public UIProgressBar bar;

	private bool bClick = false;
	private float speed = 0.1f;

	public void OnButtonClick()
	{
		bClick = !bClick;

		if(bClick == true)
		{
			AudioManager.Instance.Find("dice_hand").Play();
		}
		else
		{
			if(bar.value >= 0.7f && bar.value <= 0.9f)
			{
				demo.AddDie();
			}
			demo.OnButtonClick();
			AudioManager.Instance.Find("dice_hand").Stop();
			AudioManager.Instance.Find("dice_full").Play();
			this.GetComponent<BoxCollider2D>().enabled = false;
		}
	}

	void OnEnable()
	{
		this.GetComponent<BoxCollider2D>().enabled = true;
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
