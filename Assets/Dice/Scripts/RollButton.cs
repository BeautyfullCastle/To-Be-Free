using UnityEngine;
using System.Collections;
using ToBeFree;
using System;

public class RollButton : MonoBehaviour {

	public float speed = 0.1f;
	public float minCorrectGage = 0.7f;
	public float maxCorrectGage = 0.8f;

	public AppDemo demo;
	public UIProgressBar bar;

	private bool bClick = false;
	
	
	public void OnPress()
	{
		bClick = true;
		AudioManager.Instance.Find("dice_hand").Play();
	}

	public void OnRelease()
	{
		StartCoroutine(Roll());
	}

	private IEnumerator Roll()
	{
		bClick = false;

		if (bar.value >= minCorrectGage && bar.value <= maxCorrectGage)
		{
			yield return demo.AddDie();
		}
		demo.OnButtonClick();
		AudioManager.Instance.Find("dice_hand").Stop();
		AudioManager.Instance.Find("dice_full").Play();
		this.GetComponent<BoxCollider2D>().enabled = false;
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
