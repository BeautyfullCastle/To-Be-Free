using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class CrackDown : Singleton<CrackDown>
	{
		private int probability;
		private int increasingProbability;
		private bool isCrackDown;
		private UISprite crackDownEffect;

		public CrackDown()
		{
			probability = 0;
			increasingProbability = 20;
			isCrackDown = false;
			crackDownEffect = GameObject.Find("CrackDown Effect").GetComponent<UISprite>();
		}

		private bool RaiseAndCheckProbability()
		{
			probability += increasingProbability;
			NGUIDebug.Log("Crackdown probability : " + probability);
			return probability > UnityEngine.Random.Range(0, 100);
		}

		public IEnumerator Check()
		{
			if(isCrackDown)
			{
				isCrackDown = false;
				crackDownEffect.enabled = false;
			}
			else
			{
				if (RaiseAndCheckProbability())
				{
					isCrackDown = true;
					crackDownEffect.enabled = true;
					probability = 0;
				}
				else
				{

					int randIndex = UnityEngine.Random.Range(0, 3);
					// add one more police
					if (randIndex == 0)
					{
						Police police = new Police(CityManager.Instance.GetRand(), eSubjectType.POLICE);
						NGUIDebug.Log("CrackDown : Added Police");
						yield return PieceManager.Instance.Add(police);
					}
					// add one police's stat
					else if (randIndex == 1)
					{
						Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
						NGUIDebug.Log("CrackDown : Added Police's STAT in " + police.City.Name);
						yield return police.AddStat(isCrackDown);
					}
					// move one police
					else if (randIndex == 2)
					{
						Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
						NGUIDebug.Log("CrackDown : Moved Police from " + police.City.Name);
						yield return police.Move();
					}
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