using UnityEngine;
using ToBeFree;
using System;
using System.Collections;

public class UICrackdown : MonoBehaviour
{
	[SerializeField]
	private UICrackdownMeter shortTermMeter;
	[SerializeField]
	private UICrackdownMeter longTermMeter;
	[SerializeField]
	private UICrackdownMeter crackdownMeter;
	[SerializeField]
	private TweenRotation axisTween;

	public void Init()
	{
		if (shortTermMeter == null)
		{
			Debug.LogError("shortTermMeter Init");
			return;
		}
		shortTermMeter.Init(false);

		if (longTermMeter == null)
		{
			Debug.LogError("longTermMeter Init");
			return;
		}
		longTermMeter.Init(false);

		if (crackdownMeter == null)
		{
			Debug.LogError("crackdownMeter Init");
			return;
		}
		crackdownMeter.Init(true);
	}

	public bool TurnUpShortTermGauge()
	{
		bool isFull = shortTermMeter.TurnUpAndCheckIsFull();
		if (isFull)
			longTermMeter.TurnUpAndCheckIsFull();

		return isFull;
	}

	public bool TurnDownShortTermGauge()
	{
		if (shortTermMeter == null)
		{
			Debug.LogError("shortTermMeter is null");
			return false;
		}

		return shortTermMeter.TurnDownAndCheckIsEmpty();
	}

	public bool TurnDownCrackdownGauge()
	{
		return crackdownMeter.TurnDownAndCheckIsEmpty();
	}

	public bool CheckLongTermGauge()
	{
		return longTermMeter.IsFull();
	}

	public IEnumerator SwitchGauge(bool isCrackdown)
	{
		if (isCrackdown)
		{
			crackdownMeter.Init(true);
		}
		else
		{
			shortTermMeter.Init(false);
		}
		
		if (axisTween == null)
			yield break;

		if (isCrackdown)
		{
			axisTween.PlayForward();
		}
		else
		{
			axisTween.PlayReverse();
		}
		yield return new WaitForSeconds(axisTween.duration);
	}
}