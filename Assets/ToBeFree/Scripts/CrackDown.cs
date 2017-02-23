﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eGauge
	{
		Short, Long, Crackdown
	}

	public class CrackDown : Singleton<CrackDown>
	{
		private bool isCrackDown;
		private bool isEternalCrackdown;
		private UISprite sprite;
		private UILabel label;

		private UICrackdown uiCrackdown;
		
		public CrackDown()
		{
			Reset();
		}

		public void Reset()
		{
			this.isCrackDown = false;
			this.isEternalCrackdown = false;
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

			if(this.uiCrackdown == null)
			{
				this.uiCrackdown = GameObject.FindObjectOfType<UICrackdown>();
			}
			else
			{
				this.uiCrackdown.Init();
			}
		}

		private bool TurnUpAndCheckProbability()
		{
			return uiCrackdown.TurnUpShortTermGauge();
		}

		public IEnumerator Check()
		{
			TipManager.Instance.Show(eTipTiming.PoliceTurn);

			yield return GameManager.Instance.ShowStateLabel(LanguageManager.Instance.Find(eLanguageKey.UI_Police_Turn), 1f);

			if(isEternalCrackdown)
			{

			}
			else
			{
				if (isCrackDown)
				{
					bool isFinishedCrackdown = uiCrackdown.TurnDownCrackdownGauge();
					if (isFinishedCrackdown)
					{
						isCrackDown = false;
						if (this.sprite)
						{
							sprite.enabled = false;
						}
						if (this.label)
						{
							this.label.enabled = false;
						}
						yield return uiCrackdown.SwitchGauge(isCrackDown);
					}
				}
				else
				{
					// 집중단속이 시작되면
					if (TurnUpAndCheckProbability())
					{
						yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_Police_CrackDown));

						isCrackDown = true;
						if (this.sprite)
						{
							this.sprite.enabled = true;
						}
						if (this.label)
						{
							this.label.enabled = true;
						}
						isEternalCrackdown = uiCrackdown.CheckLongTermGauge();
						if (isEternalCrackdown)
						{

						}
						else
						{
							yield return uiCrackdown.SwitchGauge(isCrackDown);
							TipManager.Instance.Show(eTipTiming.Crackdown);
							AudioManager.Instance.ChangeBGM("Crackdown");
						}
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
	}
}