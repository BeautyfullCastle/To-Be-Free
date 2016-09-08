﻿using System;
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

	public class AbnormalCondition
	{
		private readonly string name;
		readonly protected Buff buff;
		readonly protected Condition spawnCondition;
		protected int stack;
		protected bool isStack;
		readonly protected eBodyMental isBody; // body or mental
		readonly protected ePositiveNegative isPositive;

		protected int firstAmount;

		public AbnormalCondition(string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive)
		{
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
				if (isStack)
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

	public class Despair : AbnormalCondition
	{
		public Despair(string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isStack, isBody, isPositive)
		{
			Stat.OnValueChange += Stat_OnValueChange;
		}

		private void Stat_OnValueChange(int value, eStat stat)
		{
			if (CheckCondition(GameManager.Instance.Character))
			{
				if (BuffManager.Instance.Exist(this.buff))
				{
					return;
				}
				GameManager.Instance.StartCoroutine(Activate(GameManager.Instance.Character));
			}
		}

		public override IEnumerator Activate(Character character)
		{
			yield return base.Activate(character);

			Buff buff_Exhilaration = BuffManager.Instance.Find("Exhilaration");
			if (buff_Exhilaration == null)
			{
				yield break;
			}
			yield return BuffManager.Instance.Delete(buff_Exhilaration, character);
			yield return null;
		}
	}

	public class Exhilaration : AbnormalCondition
	{
		public Exhilaration(string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isStack, isBody, isPositive)
		{

		}

		public override IEnumerator Activate(Character character)
		{
			yield return base.Activate(character);

			Buff buff_Despair = BuffManager.Instance.Find("Despair");
			if (buff_Despair == null)
			{
				yield break;
			}
			yield return BuffManager.Instance.Delete(buff_Despair, character);
			yield return null;
		}
	}

	public class Detention : AbnormalCondition
	{
		public Detention(string name, Buff buff, Condition spawnCondition, bool isStack, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isStack, isBody, isPositive)
		{

		}

		public override IEnumerator Activate(Character character)
		{
			yield return base.Activate(character);

			if (character.IsDetention == false)
			{
				CityManager.Instance.FindNearestPath(character.CurCity, CityManager.Instance.Find("TUMEN"));
			}
			character.IsDetention = true;

			yield return null;
		}

		public override IEnumerator DeActivate(Character character)
		{
			yield return base.DeActivate(character);
			character.IsDetention = false;
			yield return null;
		}
	}
}