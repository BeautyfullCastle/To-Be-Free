using UnityEngine;
using System.Collections.Generic;
using System;

public class UICrackdownMeter : MonoBehaviour
{
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
		if (isMax)
		{
			this.CurrentGauge = this.TotalGauge;
		}
		else
		{
			this.CurrentGauge = 0;
		}

		foreach(UICrackdownCell cell in cells)
		{
			cell.TurnOnSprite(isMax);
		}
	}

	public bool TurnUpAndCheckIsFull()
	{
		if(cells[CurrentGauge].IsOn())
		{
			CurrentGauge++;
		}
		cells[CurrentGauge].TurnOnSprite(true);

		return IsFull();
	}

	public bool TurnDownAndCheckIsEmpty()
	{
		if(cells[CurrentGauge].IsOn() == false)
		{
			CurrentGauge--;
		}
		cells[CurrentGauge].TurnOnSprite(false);

		return IsEmpty();
	}

	public bool IsFull()
	{
		return this.CurrentGauge >= this.TotalGauge;
	}

	public bool IsEmpty()
	{
		return CurrentGauge < 0;
	}

	public int CurrentGauge
	{
		get
		{
			return this.currentGauge;
		}
		private set
		{
			if(value < 0 || value >= this.cells.Length)
			{
				return;
			}
			this.currentGauge = value;
		}
	}

	private int TotalGauge
	{
		get
		{
			if (this.cells == null)
			{
				Debug.LogError(this.gameObject.name + "'s cells' null.");
				return -1;
			}

			return this.cells.Length - 1;
		}
	}
}
