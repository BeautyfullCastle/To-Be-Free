using UnityEngine;
using System.Collections;
using ToBeFree;

public class DoubleClick : MonoBehaviour
{
	public float delayBetween2Clicks; // Change value in editor
	private float lastClickTime = 0;

	public void OnClickCallBack()
	{
		if (Time.time - lastClickTime < delayBetween2Clicks)
		{
			Debug.Log("Double clicked");
		}
		else
		{
			StartCoroutine(OnClickCoroutine());
		}
		lastClickTime = Time.time;
	}

	IEnumerator OnClickCoroutine()
	{
		yield return new WaitForSeconds(delayBetween2Clicks);

		if (Time.time - lastClickTime < delayBetween2Clicks)
		{
			//yield return this.GetComponent<UIItem>().OnDoubleClick();
		}

		Debug.Log("Simple click");
	}

}