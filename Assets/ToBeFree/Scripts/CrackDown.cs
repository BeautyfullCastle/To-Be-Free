using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class CrackDown : Singleton<CrackDown>
	{
		private int probability;
		private int increasingProbability;
		private int duration;
		private bool isCrackDown;

		public CrackDown()
		{
			probability = 0;
			increasingProbability = 20;
			duration = 0;
			isCrackDown = false;

			TimeTable.Instance.NotifyEveryWeek += Instance_NotifyEveryWeek;
		}

		private void Instance_NotifyEveryWeek()
		{
			if(isCrackDown)
			{
				isCrackDown = false;
			}
		}

		private bool RaiseAndCheckProbability()
		{
			probability += increasingProbability;
			return probability > UnityEngine.Random.Range(0, 100);
		}

		public IEnumerator Check()
		{
			if(RaiseAndCheckProbability())
			{
				isCrackDown = true;
			}
			else
			{
				
				int randIndex = UnityEngine.Random.Range(0, 3);
				// add one more police
				if (randIndex == 0)
				{
					Police police = new Police(CityManager.Instance.GetRand(), eSubjectType.POLICE);
					PieceManager.Instance.Add(police);
				}
				// add one police's stat
				else if (randIndex == 1)
				{
					Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
					yield return police.AddStat(isCrackDown);
				}
				// move one police
				else if(randIndex == 2)
				{
					Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
					yield return police.Move();
				}
			}
		}

		public bool IsCrackDown
		{
			get
			{
				return isCrackDown;
			}
			set
			{
				isCrackDown = value;
				GameObject.Find("CrackDown Effect").GetComponent<UISprite>().enabled = isCrackDown;
			}
		}
	}
}