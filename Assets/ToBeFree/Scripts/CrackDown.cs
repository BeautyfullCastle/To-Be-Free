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
		private UISprite sprite;
		private UILabel label;

		public CrackDown()
		{
			Reset();
		}

		public void Reset()
		{
			this.probability = 0;
			this.increasingProbability = 20;
			this.isCrackDown = false;
			if(this.sprite == null)
			{
				GameObject crackdownObj = GameObject.Find("CrackDown Effect");
				if(crackdownObj)
				{
					this.sprite = crackdownObj.GetComponent<UISprite>();
				}
			}
			else
			{
				this.sprite.enabled = false;
			}
			if(this.label == null)
			{
				UITimeTable uiTimeTable = GameObject.FindObjectOfType<UITimeTable>();
				if(uiTimeTable)
				{
					this.label = uiTimeTable.crackdownLabel;
				}
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

			yield return GameManager.Instance.ShowStateLabel(LanguageManager.Instance.Find(eLanguageKey.UI_Police_Turn), 1f);

			if(isCrackDown)
			{
				isCrackDown = false;
				if(this.sprite)
				{
					sprite.enabled = false;
				}
				if(this.label)
				{
					this.label.enabled = false;
				}
			}
			else
			{
				if (RaiseAndCheckProbability())
				{
					yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Police_CrackDown));

					isCrackDown = true;
					if(this.sprite)
					{
						this.sprite.enabled = true;
					}
					if(this.label)
					{
						this.label.enabled = true;
					}
					probability = 0;
					TipManager.Instance.Show(eTipTiming.Crackdown);
				}
				else
				{

					int randIndex = UnityEngine.Random.Range(0, 3);
					// add one more police
					if (randIndex == 0)
					{
						yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Police_Add));

						Police police = new Police(CityManager.Instance.GetRand(), eSubjectType.POLICE);
						PieceManager.Instance.Add(police);
					}
					// add one police's stat
					else if (randIndex == 1)
					{
						yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Police_AddStat));

						Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
						yield return police.AddStat(isCrackDown);
					}
					// move one police
					else if (randIndex == 2)
					{
						yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Police_Move));

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