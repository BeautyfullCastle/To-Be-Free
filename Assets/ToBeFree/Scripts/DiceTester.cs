using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class DiceTester : Singleton<DiceTester>
	{
		private int minSuccessNum;
		private int prevMinSuccessNum;
		private GameObject diceObj;

		public void Init()
		{
			minSuccessNum = 6;
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
			while (dice.IsRolling())
			{
				yield return new WaitForSecondsRealtime(1f);
			}

			yield return dice.StartEffect(MinSuccessNum);

			yield return new WaitForSecondsRealtime(2f);

			resultNum = dice.GetSuccessNum(MinSuccessNum);
			
			setResultNum(resultNum);
			diceObj.SetActive(false);

			GameManager.Instance.Character.Stat.Restore(EnumConvert<eObjectType>.ToEnum(stat.ToString()));
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

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);

			while (demo.mouseDown == false)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.TEST, GameManager.Instance.Character);

			yield return new WaitForSecondsRealtime(2f);

			for (int i = 0; i < demo.dices.Length; ++i)
			{
				while (demo.dices[i].IsRolling())
				{
					yield return new WaitForSecondsRealtime(1f);
					continue;
				}
				resultNums[i] = demo.dices[i].GetSuccessNum(MinSuccessNum);
			}

			yield return demo.dices[0].StartEffect(demo.dices[1], minSuccessNum);

			yield return new WaitForSecondsRealtime(2f);

			setResultNum(resultNums[0], resultNums[1]);
			diceObj.SetActive(false);

			GameManager.Instance.Character.Stat.Restore(EnumConvert<eObjectType>.ToEnum(stat.ToString()));
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
