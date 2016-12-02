using System;
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

		public IEnumerator Test(int diceNum, System.Action<int> setResultNum)
		{
			if(diceObj == null)
			{
				diceObj = GameObject.Find("Dice Tester");
				yield break;
			}
			
			AppDemo demo = diceObj.GetComponent<AppDemo>();
			demo.Init(diceNum);
			int resultNum = 0;

			diceObj.SetActive(true);

			Dice dice = demo.dices[0];
			while (dice.IsHitToGround() == false || dice.GetSuccessNum(MinSuccessNum) == -99 || demo.mouseDown == false)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			resultNum = dice.GetSuccessNum(MinSuccessNum);
			
			setResultNum(resultNum);
			diceObj.SetActive(false);
		}

		public IEnumerator Test(int characterDiceNum, int policeDiceNum, System.Action<int, int> setResultNum)
		{
			if (diceObj == null)
			{
				diceObj = GameObject.Find("Dice Tester");
				yield break;
			}

			AppDemo demo = diceObj.GetComponent<AppDemo>();
			demo.Init(characterDiceNum, policeDiceNum);
			int[] resultNums = { 0, 0 };

			diceObj.SetActive(true);
			
			for (int i = 0; i < demo.dices.Length; ++i)
			{
				while (demo.dices[i].IsHitToGround() == false || demo.dices[i].GetSuccessNum(MinSuccessNum) == -99 || demo.mouseDown == false)
				{
					yield return new WaitForSecondsRealtime(1f);
				}
				resultNums[i] = demo.dices[i].GetSuccessNum(MinSuccessNum);
			}

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
