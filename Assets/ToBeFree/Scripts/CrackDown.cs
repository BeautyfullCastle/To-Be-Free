using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eGauge
	{
		Short, Long, Crackdown
	}

	[Serializable]
	public class CrackDownSaveData
	{
		public bool isCrackDown;
		public bool isEternalCrackdown;
		public int shortTermGauge;
		public int longTermGauge;
		public int crackdownGauge;
	}

	public class CrackDown : Singleton<CrackDown>
	{
		private bool isCrackDown;
		private bool isEternalCrackdown;
		private UISprite sprite;

		private UICrackdown uiCrackdown;

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

			if(this.uiCrackdown == null)
			{
				this.uiCrackdown = GameObject.FindObjectOfType<UICrackdown>();
			}
			else
			{
				this.uiCrackdown.Init();
			}
		}

		public void Save(CrackDownSaveData data)
		{
			data.isCrackDown = this.IsCrackDown;
			data.isEternalCrackdown = this.isEternalCrackdown;
			data.shortTermGauge = this.uiCrackdown.ShortTermMeter.CurrentGauge;
			data.longTermGauge = this.uiCrackdown.LongTermMeter.CurrentGauge;
			data.crackdownGauge = this.uiCrackdown.CrackdownMeter.CurrentGauge;

			SaveLoadManager.Instance.data.crackdown = data;
		}

		public void Load(CrackDownSaveData data)
		{
			this.IsCrackDown = data.isCrackDown;
			this.isEternalCrackdown = data.isEternalCrackdown;

			//for(int i = 0; i < data.shortTermGauge; ++i)
			//	this.uiCrackdown.ShortTermMeter.TurnUpAndCheckIsFull();

			//for (int i = 0; i < data.longTermGauge; ++i)
			//	this.uiCrackdown.LongTermMeter.TurnUpAndCheckIsFull();

			//for (int i = 0; i < data.crackdownGauge; ++i)
			//	this.uiCrackdown.CrackdownMeter.TurnUpAndCheckIsFull();

			this.uiCrackdown.ShortTermMeter.SetCellNum(data.shortTermGauge);
			this.uiCrackdown.LongTermMeter.SetCellNum(data.longTermGauge);
			this.uiCrackdown.CrackdownMeter.SetCellNum(data.crackdownGauge);

			if(isEternalCrackdown)
			{
				this.uiCrackdown.AxisTween.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			}
			else
			{
				if (isCrackDown)
				{
					this.uiCrackdown.AxisTween.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
					AudioManager.Instance.ChangeBGM("Crackdown");
				}
				else
				{
					this.uiCrackdown.AxisTween.transform.localRotation = Quaternion.identity;
					AudioManager.Instance.ChangeBGM("Main");
				}
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
						AudioManager.Instance.ChangeBGM("Main");
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
							yield return police.MoveToRandomCity();
						}
					}
				}
			}
		}

		public IEnumerator DecreaseShortTermGauge(int amount)
		{
			if (amount <= 0)
				yield break;

			for(int i=0; i<amount; ++i)
			{
				if(uiCrackdown.TurnDownShortTermGauge())
				{
					break;
				}
				yield return new WaitForSeconds(0.5f);
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
					yield return police.MoveToRandomCity();
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
				GameObject effect = GameObject.Find("CrackDown Effect");
				if (effect == null)
					return;

				effect.GetComponent<UISprite>().enabled = isCrackDown;
			}
		}
	}
}