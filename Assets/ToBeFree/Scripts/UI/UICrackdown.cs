using UnityEngine;
using ToBeFree;
using System;

public class UICrackdown : MonoBehaviour
{
	[SerializeField]
	private UICrackdownMeter shortTermMeter;
	[SerializeField]
	private UICrackdownMeter longTermMeter;
	[SerializeField]
	private UICrackdownMeter crackdownMeter;
	[SerializeField]
	private Transform axis;

	void Start()
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

	public bool TurnDownCrackdownGauge()
	{
		return crackdownMeter.TurnDownAndCheckIsEmpty();
	}

	public void SwitchGauge(bool isCrackdown)
	{
		int isPositive = isCrackdown ? -1 : 1;
		float rotateAngle = 90f * isPositive;
		axis.Rotate(Vector3.right, -90f);
	}
}