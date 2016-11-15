using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class DiceTester : Singleton<DiceTester>
	{
		private int minSuccessNum;
		private int resultNum;
		private GameObject diceObj;

		public void Init()
		{
			minSuccessNum = 5;
			resultNum = -99;
			diceObj = GameObject.Find("Dice Tester");
			diceObj.SetActive(false);
		}

		public IEnumerator Test(int diceNum, System.Action<int> setResultNum)
		{
			if(diceObj == null)
			{
				diceObj = GameObject.Find("Dice Tester");
				yield break;
			}

			diceObj.SetActive(true);
			diceObj.GetComponent<AppDemo>().diceNum = diceNum;
			resultNum = -99;
			Dice.Clear();
			
			while (Dice.GetSuccessNum(MinSuccessNum) == -99)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}

			resultNum = Dice.GetSuccessNum(MinSuccessNum);
			setResultNum(resultNum);
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
				minSuccessNum = value;
			}
		}
	}
}
