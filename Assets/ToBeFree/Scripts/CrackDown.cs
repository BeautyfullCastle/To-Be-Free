using System.Collections;
using System.Collections.Generic;
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
			Reset();
		}

		public void Reset()
		{
			probability = 0;
			increasingProbability = 20;
			isCrackDown = false;
			if(crackDownEffect == null)
			{
				crackDownEffect = GameObject.Find("CrackDown Effect").GetComponent<UISprite>();
			}
		}

		private bool RaiseAndCheckProbability()
		{
			probability += increasingProbability;
			NGUIDebug.Log("Crackdown probability : " + probability);
			return probability > UnityEngine.Random.Range(0, 100);
		}

		public IEnumerator Check()
		{
			TipManager.Instance.Show(eTipTiming.PoliceTurn);

			yield return GameManager.Instance.ShowStateLabel("Police Turn", 2f);

			if(isCrackDown)
			{
				isCrackDown = false;
				crackDownEffect.enabled = false;
				GameObject.FindObjectOfType<UITimeTable>().crackdownLabel.enabled = false;
			}
			else
			{
				if (RaiseAndCheckProbability())
				{
					GameManager.Instance.OpenEventUI();
					GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
						LanguageManager.Instance.Find(eLanguageKey.Event_Police_CrackDown));
					yield return EventManager.Instance.WaitUntilFinish();

					isCrackDown = true;
					crackDownEffect.enabled = true;
					GameObject.FindObjectOfType<UITimeTable>().crackdownLabel.enabled = true;
					probability = 0;
					TipManager.Instance.Show(eTipTiming.Crackdown);
				}
				else
				{

					int randIndex = UnityEngine.Random.Range(0, 3);
					// add one more police
					if (randIndex == 0)
					{
						GameManager.Instance.OpenEventUI();
						GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
							LanguageManager.Instance.Find(eLanguageKey.Event_Police_Add));
						yield return EventManager.Instance.WaitUntilFinish();

						Police police = new Police(CityManager.Instance.GetRand(), eSubjectType.POLICE);
						PieceManager.Instance.Add(police);
					}
					// add one police's stat
					else if (randIndex == 1)
					{
						GameManager.Instance.OpenEventUI();
						GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
							LanguageManager.Instance.Find(eLanguageKey.Event_Police_AddStat));
						yield return EventManager.Instance.WaitUntilFinish();

						Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
						yield return police.AddStat(isCrackDown);
					}
					// move one police
					else if (randIndex == 2)
					{
						GameManager.Instance.OpenEventUI();
						GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT,
							LanguageManager.Instance.Find(eLanguageKey.Event_Police_Move));
						yield return EventManager.Instance.WaitUntilFinish();

						Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
						yield return police.Move();
					}
				}
			}
		}

		public IEnumerator MoveEveryPolice()
		{
			if (isCrackDown)
			{
				List<Piece> pieces = PieceManager.Instance.FindAll(eSubjectType.POLICE);
				foreach (Piece piece in pieces)
				{
					Police police = piece as Police;
					
					// 체포해 후송 중인 공안은 따로 이동시키지 않는다.
					if(police == GameManager.Instance.Character.CaughtPolice)
					{
						continue;
					}
					NGUIDebug.Log("CrackDown : Move Police from " + police.City.Name);
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

		public int Probability
		{
			get
			{
				return probability;
			}
		}
	}
}