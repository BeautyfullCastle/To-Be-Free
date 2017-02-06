using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class DiceTester : Singleton<DiceTester>
	{
		public AppDemo demo;
		private GameObject diceObj;

		private int minSuccessNum;
		private int prevMinSuccessNum;
		private int additionalDie;

		public void Init()
		{
			minSuccessNum = 6;
			diceObj = GameManager.Instance.diceObj;
			diceObj.SetActive(false);

			demo = diceObj.GetComponent<AppDemo>();

			additionalDie = 0;
		}

		public IEnumerator Test(eTestStat stat, int characterDiceNum, int policeDiceNum, System.Action<int, int> setResultNum)
		{
			if (diceObj == null)
			{
				diceObj = GameObject.Find("Dice Tester");
				yield break;
			}

			diceObj.SetActive(true);
			
			yield return demo.Init(stat, characterDiceNum + additionalDie, policeDiceNum);

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);

			demo.SetEnableRollButton();

			int[] resultNums = { 0, 0 };
			
			while (demo.IsMouseDown == false)
			{
				yield return new WaitForSeconds(1f);
			}

			yield return new WaitForSeconds(1f);

			for (int i = 0; i < demo.dices.Length; ++i)
			{
				if(demo.dices[i].gameObject.activeSelf == false)
				{
					continue;
				}

				while (demo.dices[i].IsRolling())
				{
					yield return new WaitForSeconds(1f);
					continue;
				}

				yield return new WaitForSeconds(1f);

				resultNums[i] = demo.dices[i].GetSuccessNum(MinSuccessNum);
			}

			yield return demo.dices[0].StartEffect(demo.dices[1], minSuccessNum);

			yield return new WaitForSeconds(1f);

			setResultNum(resultNums[0], resultNums[1]);
			
			diceObj.SetActive(false);

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);
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

		public int AdditionalDie
		{
			get
			{
				return additionalDie;
			}

			set
			{
				additionalDie = value;
			}
		}
	}
}
