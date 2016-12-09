﻿using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class DiceTester : Singleton<DiceTester>
	{
		private int minSuccessNum;
		private int prevMinSuccessNum;
		private int resultNum;
		private GameObject diceObj;

		public void Init()
		{
			minSuccessNum = 6;
			resultNum = -99;
			diceObj = GameManager.Instance.diceObj;
			diceObj.SetActive(false);
		}

		public IEnumerator Test(eTestStat stat, int diceNum, System.Action<int> setResultNum)
		{
			if(diceObj == null)
			{
				diceObj = GameObject.Find("Dice Tester");
				yield break;
			}
			
			AppDemo demo = diceObj.GetComponent<AppDemo>();
			demo.Init(stat, diceNum);
			int resultNum = 0;

			diceObj.SetActive(true);

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);

			while(demo.mouseDown == false)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);

			Dice dice = demo.dices[0];
			while (dice.IsHitToGround() == false || dice.GetSuccessNum(MinSuccessNum) == -99)
			{
				yield return new WaitForSecondsRealtime(1f);
			}

			yield return new WaitForSecondsRealtime(2f);

			resultNum = dice.GetSuccessNum(MinSuccessNum);
			
			setResultNum(resultNum);
			diceObj.SetActive(false);
		}

		public IEnumerator Test(eTestStat stat, int characterDiceNum, int policeDiceNum, System.Action<int, int> setResultNum)
		{
			if (diceObj == null)
			{
				diceObj = GameObject.Find("Dice Tester");
				yield break;
			}

			AppDemo demo = diceObj.GetComponent<AppDemo>();
			diceObj.SetActive(true);
			demo.Init(stat, characterDiceNum, policeDiceNum);
			int[] resultNums = { 0, 0 };

			while (demo.mouseDown == false)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}

			yield return new WaitForSecondsRealtime(2f);

			for (int i = 0; i < demo.dices.Length; ++i)
			{
				while (demo.dices[i].IsHitToGround() == false || demo.dices[i].rolling)
				{
					yield return new WaitForSecondsRealtime(1f);
					continue;
				}
				resultNums[i] = demo.dices[i].GetSuccessNum(MinSuccessNum);
			}

			yield return new WaitForSecondsRealtime(2f);

			setResultNum(resultNums[0], resultNums[1]);
			diceObj.SetActive(false);
		}

		public int MinSuccessNum
		{
			get
			{
				return minSuccessNum;
			}

			set
			{
				if (!(value == 5 || value == 6))
				{
					throw new System.Exception("Input Dice success num is not 5 or 6.");
				}
				prevMinSuccessNum = value;
				minSuccessNum = value;
			}
		}

		public int PrevMinSuccessNum
		{
			get
			{
				return prevMinSuccessNum;
			}
		}

	}
}
