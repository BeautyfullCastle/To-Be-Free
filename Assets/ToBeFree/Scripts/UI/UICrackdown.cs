using UnityEngine;
using ToBeFree;
using System;
using System.Collections;

public class UICrackdown : MonoBehaviour
{
	[SerializeField]
	private int shortTermCellNum;
	[SerializeField]
	private UIGaugeMeter shortTermMeter;
	[SerializeField]
	private int longTermCellNum;
	[SerializeField]
	private UIGaugeMeter longTermMeter;
	[SerializeField]
	private int crackdownCellNum;
	[SerializeField]
	private UIGaugeMeter crackdownMeter;
	[SerializeField]
	private TweenRotation axisTween;

	public void Init()
	{
		if (shortTermMeter == null)
		{
			Debug.LogError("shortTermMeter Init");
			return;
		}
		if(shortTermCellNum <= 0)
		{
			Debug.LogError("longTerm Cell Num is wrong.");
		}
		shortTermMeter.Init(shortTermCellNum, false);

		if (longTermMeter == null)
		{
			Debug.LogError("longTermMeter Init");
			return;
		}
		if (longTermCellNum <= 0)
		{
			Debug.LogError("shorTerm Cell Num is wrong.");
		}
		longTermMeter.Init(longTermCellNum, false);

		if (crackdownMeter == null)
		{
			Debug.LogError("crackdownMeter Init");
			return;
		}
		if (crackdownCellNum <= 0)
		{
			Debug.LogError("crackdown Cell Num is wrong.");
		}
		crackdownMeter.Init(crackdownCellNum, true);
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
			crackdownMeter.Reset();
		}
		else
		{
			shortTermMeter.Reset();
		}
		
		if (axisTween == null)
		{
			yield break;
		}

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