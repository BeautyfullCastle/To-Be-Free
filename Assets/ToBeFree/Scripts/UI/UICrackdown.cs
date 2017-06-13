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
	[SerializeField]
	private TweenAlpha backgroundTween;
	[SerializeField]
	private UILabel label;
	[SerializeField]
	private UILabel labelForEternal;

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
		shortTermMeter.gameObject.SetActive(true);
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
		longTermMeter.gameObject.SetActive(true);
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
		crackdownMeter.gameObject.SetActive(true);
		crackdownMeter.Init(crackdownCellNum, true);

		axisTween.gameObject.SetActive(true);
		longTermMeter.gameObject.SetActive(true);
		SetBackgroundTween(false);

		label.enabled = true;
		labelForEternal.enabled = false;
	}

	public bool TurnUpShortTermGauge()
	{
		bool isFull = shortTermMeter.TurnUpAndCheckIsFull();
		if (isFull)
		{
			longTermMeter.TurnUpAndCheckIsFull();
		}

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
	
	public void SetToEternalCrackdown()
	{
		shortTermMeter.gameObject.SetActive(false);
		crackdownMeter.gameObject.SetActive(false);
		longTermMeter.gameObject.SetActive(false);
		SetBackgroundTween(true);
		label.enabled = false;
		labelForEternal.enabled = true;
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

		SetBackgroundTween(isCrackdown);

		yield return new WaitForSeconds(axisTween.duration);
	}

	private void SetBackgroundTween(bool isCrackdown)
	{
		backgroundTween.ResetToBeginning();
		backgroundTween.enabled = isCrackdown;
	}

	public UIGaugeMeter ShortTermMeter
	{
		get
		{
			return shortTermMeter;
		}
	}

	public UIGaugeMeter LongTermMeter
	{
		get
		{
			return longTermMeter;
		}
	}

	public UIGaugeMeter CrackdownMeter
	{
		get
		{
			return crackdownMeter;
		}
	}

	public TweenRotation AxisTween
	{
		get
		{
			return axisTween;
		}
	}
}