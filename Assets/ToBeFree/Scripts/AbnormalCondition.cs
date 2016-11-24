using System;
using System.Collections;

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
		public int index;
		public int stack;
		public int buffAliveDays;
	}

	public class AbnormalCondition
	{
		protected readonly int index;
		private readonly string name;
		protected readonly Buff buff;
		protected readonly Condition spawnCondition;
		protected int stack;
		protected bool isStack;
		protected readonly eBodyMental isBody; // body or mental
		protected readonly ePositiveNegative isPositive;

		protected int firstAmount;

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
			yield return GameManager.Instance.ShowStateLabel(this.name + " is added.", 0.5f);

			if (BuffManager.Instance.Exist(this.buff))
			{
				if(isStack)
				{
					stack++;
					buff.EffectAmountList[0].Amount += firstAmount;
				}
			}
			else
			{
				yield return BuffManager.Instance.Add(this.buff);
			}
		}

		public virtual IEnumerator DeActivate(Character character)
		{
			yield return GameManager.Instance.ShowStateLabel(this.name + " is deleted.", 0.5f);

			yield return BuffManager.Instance.Delete(this.buff, character);

			this.stack = 1;
			this.buff.AliveDays = 0;

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

		public override IEnumerator DeActivate(Character character)
		{
			yield return base.DeActivate(character);
			character.IsDetention = false;
			yield return null;
		}
	}
}