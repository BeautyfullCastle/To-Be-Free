using UnityEngine;
using System.Collections.Generic;
using System;

public class UICrackdownMeter : MonoBehaviour
{
	private int totalGauge;
	private int currentGauge;

	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private UICrackdownCell[] cells;

	public void Init(bool isMax)
	{
		if (cells == null)
		{
			Debug.LogError("You should Init cells : " + this.name);
			return;
		}
		else if(cells.Length == 0)
		{
			Debug.LogError("You should Init cells : " + this.name);
			return;
		}
		
		totalGauge = cells.Length;
		if (isMax)
		{
			this.currentGauge = totalGauge - 1;
		}
		else
		{
			this.currentGauge = 0;
		}

		foreach(UICrackdownCell cell in cells)
		{
			cell.TurnOnSprite(isMax);
		}
	}

	public bool TurnUpAndCheckIsFull()
	{
		if(IsFull())
			return true;
		
		cells[currentGauge].TurnOnSprite(true);
		currentGauge++;

		return IsFull();
	}

	public bool TurnDownAndCheckIsEmpty()
	{
		if (IsEmpty())
			return true;
		
		cells[currentGauge].TurnOnSprite(false);
		currentGauge--;

		return IsEmpty();
	}

	public bool IsFull()
	{
		return currentGauge >= totalGauge;
	}

	public bool IsEmpty()
	{
		return currentGauge < 0;
	}
}
