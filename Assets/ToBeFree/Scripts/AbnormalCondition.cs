using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eBodyMental
	{
		BODY, MENTAL, NULL
	}

	public enum ePositiveNegative
	{
		POSITIVE, NEGATIVE, NULL
	}

	[Serializable]
	public class AbnormalConditionSaveData
	{
		public AbnormalConditionSaveData(int index, int stack, int amount)
		{
			this.index = index;
			this.stack = stack;
			this.amount = amount;
		}

		public int index;
		public int stack;
		public int amount;
	}

	public class AbnormalCondition
	{
		private readonly int index;
		private string name;
		protected readonly Buff buff;
		protected readonly Condition spawnCondition;
		protected int stack;
		protected bool isStack;
		protected readonly eBodyMental isBody; // body or mental
		protected readonly ePositiveNegative isPositive;

		private int firstAmount;

		public AbnormalCondition(int index, string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive)
		{
			this.index = index;
			this.name = name;
			this.buff = new Buff(buff);
			this.spawnCondition = spawnCondition;
			this.isStack = isStack;
			this.isBody = isBody;
			this.isPositive = isPositive;

			this.stack = 1;
			this.firstAmount = buff.EffectAmountList[0].Amount;
		}

		public virtual IEnumerator Activate(Character character)
		{
			if (BuffManager.Instance.Exist(this.buff))
			{
				if(isStack)
				{
					stack++;
					this.buff.EffectAmountList[0].Amount = firstAmount * stack;
					UIBuff uiBuff = GameManager.Instance.uiBuffManager.Find(this.buff);
					if(uiBuff)
					{
						uiBuff.stackLabel.text = stack.ToString();
					}
				}
			}
			else
			{
				yield return BuffManager.Instance.Add(this.buff, this.isStack);
			}
		}

		public virtual IEnumerator DeActivate(Character character)
		{
			yield return BuffManager.Instance.Delete(this.buff, character);

			this.stack = 1;
			this.buff.AliveDays = 0;
			this.buff.EffectAmountList[0].Amount = firstAmount;
			yield return null;
		}

		public virtual bool CheckCondition(Character character)
		{
			return spawnCondition.CheckCondition(character);
		}
		
		public Buff Buff
		{
			get
			{
				return buff;
			}
		}

		public Condition SpawnCondition
		{
			get
			{
				return spawnCondition;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public int Stack
		{
			get
			{
				return stack;
			}

			set
			{
				stack = value;
			}
		}

		public int Amount
		{
			get
			{
				return buff.EffectAmountList[0].Amount;
			}
			set
			{
				buff.EffectAmountList[0].Amount = value;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public bool IsStack
		{
			get
			{
				return isStack;
			}
		}
	}

	public class Hunger : AbnormalCondition
	{
		public Hunger(int index, string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive) : base(index, name, buff, spawnCondition, isStack, isBody, isPositive)
		{
			Stat.OnValueChange += Stat_OnValueChange;
		}

		private void Stat_OnValueChange(int value, eStat stat)
		{
			//밤마다 배고픔 수치만큼 체력피해를 입습니다.\n 식량을 먹으면 사라집니다.
			if (BuffManager.Instance.Exist(this.buff))
			{
				if(stat == eStat.SATIETY && value >= 1)
				{
					GameManager.Instance.StartCoroutine(this.DeActivate(GameManager.Instance.Character));
				}
			}
		}
	}

	public class Detention : AbnormalCondition
	{
		public Detention(int index, string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive) : base(index, name, buff, spawnCondition, isStack, isBody, isPositive)
		{

		}

		public override IEnumerator Activate(Character character)
		{
			yield return base.Activate(character);
			AudioManager.Instance.ChangeBGM("Detention");
		}

		public override IEnumerator DeActivate(Character character)
		{
			yield return base.DeActivate(character);
			character.IsDetention = false;
			character.CaughtPolice = null;
			AudioManager.Instance.ChangeToPrevBGM();
			yield return null;
		}
	}
}