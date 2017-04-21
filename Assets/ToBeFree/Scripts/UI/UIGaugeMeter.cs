using UnityEngine;
using System.Collections.Generic;
using ToBeFree;
using System;

public class UIGaugeMeter : MonoBehaviour
{
	[SerializeField]
	private eStat stat;

	[SerializeField]
	private UIGrid grid;
	[SerializeField]
	private GameObject cellObj;
	[SerializeField]
	private int cellSize;
	[SerializeField]
	private Color cellColor;

	private List<UIGaugeCell> cellList;

	private int currentGauge;
	private bool startWithFullGauge;

	void Awake()
	{
		Stat.OnValueChange += OnValueChange;
	}

	private void OnValueChange(int value, eStat stat)
	{
		string enumName = EnumConvert<eStat>.ToString(stat);
		bool isTotal = enumName.Contains("TOTAL");
		if (isTotal)
		{
			enumName = enumName.Replace("TOTAL", "");
		}

		if (!(stat == this.stat || EnumConvert<eStat>.ToEnum(enumName) == this.stat))
		{
			return;
		}

		if(isTotal)
		{
			this.Init(value, true);
		}
		else
		{
			this.SetCellNum(value);
		}
	}

	public void SetCellNum(int value)
	{
		if (value < 0 || value > this.TotalGauge)
		{
			Debug.LogError(this.gameObject.name + " : SetCellNum : value is over the range : " + value);
			return;
		}

		/* ex ) 셀 5개
		 * value : 0
		 * - 전부 false
		 * value : 1
		 * - 0번 true
		 * value : list.count
		 * - 전부 true
		 */
		bool isCellActive = false;
		for (int i = 0; i < cellList.Count; ++i)
		{
			// 0 : 5 = true
			// 4 : 5 = true
			isCellActive = i < value;
			//if (isCellActive)
			//	CurrentGauge = i;
			cellList[i].TurnOnSprite(isCellActive);
		}
		CurrentGauge = value;
	}

	public void Init(int cellNum, bool startWithFullGauge)
	{
		if (grid == null)
		{
			this.grid = this.GetComponent<UIGrid>();
		}
		if(cellObj == null)
		{
			Debug.LogError("cellObj is null.");
			return;
		}

		if(cellList == null)
		{
			cellList = new List<UIGaugeCell>();
			for (int i = 0; i < cellNum; ++i)
			{
				GameObject obj = GameObject.Instantiate(cellObj, grid.transform) as GameObject;
				obj.transform.localScale = new Vector3(1, 1, 1);
				UIGaugeCell cell = obj.GetComponent<UIGaugeCell>();
				if (cell == null)
					continue;

				cell.ChangeSpritesParam(this.cellSize, this.cellColor);
				cellList.Add(cell);
			}
		}
		
		grid.Reposition();
		
		this.startWithFullGauge = startWithFullGauge;
		this.Reset();
	}

	public void Reset()
	{
		if (startWithFullGauge)
		{
			this.CurrentGauge = this.TotalGauge;
		}
		else
		{
			this.CurrentGauge = 0;
		}

		foreach (UIGaugeCell cell in cellList)
		{
			cell.TurnOnSprite(startWithFullGauge);
		}
	}

	public bool TurnUpAndCheckIsFull()
	{
		//if(cellList[CurrentGauge].IsOn())
		//{
		//	CurrentGauge++;
		//}

		bool isFull = IsFull();
		if (isFull)
			return true;

		cellList[CurrentGauge++].TurnOnSprite(true);

		return isFull;
	}

	public bool TurnDownAndCheckIsEmpty()
	{
		//if(cellList[CurrentGauge].IsOn() == false)
		//{
		//	CurrentGauge--;
		//}

		bool isEmpty = this.IsEmpty();
		if (isEmpty)
			return true;

		cellList[CurrentGauge--].TurnOnSprite(false);

		return isEmpty;
	}

	public bool IsFull()
	{
		return this.CurrentGauge >= this.TotalGauge;
	}

	public bool IsEmpty()
	{
		return CurrentGauge <= 0;
	}

	public int CurrentGauge
	{
		get
		{
			return this.currentGauge;
		}
		private set
		{
			if(value < 0 || value > this.cellList.Count)
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
			if (this.cellList == null)
			{
				Debug.LogError(this.gameObject.name + "'s cellList' null.");
				return -1;
			}

			return this.cellList.Count;
		}
	}
}
