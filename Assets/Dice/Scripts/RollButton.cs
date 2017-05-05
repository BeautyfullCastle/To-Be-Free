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

	private BoxCollider2D collider;
	private bool bClick = false;
	
	void Awake()
	{
		this.collider = this.GetComponent<BoxCollider2D>();
	}
	
	public void OnPress()
	{
		bClick = true;
		AudioManager.Instance.Find("dice_hand").Play();
	}

	public void OnRelease()
	{
		this.SetEnable(false);
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
		
	}

	public void SetEnable(bool isEnable)
	{
		if (this.collider)
		{
			collider.enabled = isEnable;
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
