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
