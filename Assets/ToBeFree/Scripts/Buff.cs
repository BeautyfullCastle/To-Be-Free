using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public enum eBuff
	{
		NAME, AMOUNT, STACK
	}

	public class Buff
	{
		private readonly string name;
		private readonly EffectAmount[] effectAmountList;
		private readonly bool isRestore;
		private readonly eStartTime startTime;
		private readonly eDuration duration;

		private int aliveDays;

		
		public Buff(string name, EffectAmount[] effectAmountList, bool isRestore,
			eStartTime startTime, eDuration duration)
		{
			this.name = name;
			this.effectAmountList = effectAmountList;
			this.isRestore = isRestore;
			this.startTime = startTime;
			this.duration = duration;
		}

		public Buff(Buff buff) : this(buff.name, buff.effectAmountList, buff.isRestore,
			buff.startTime, buff.duration)
		{
			aliveDays = 0;
		}

		public IEnumerator ActivateEffect(Character character)
		{
			Debug.Log("buff " + name + "'s effect activate");
			foreach (EffectAmount effectAmount in effectAmountList)
			{
				yield return effectAmount.Activate(character);
			}
		}

		public IEnumerator DeactivateEffect(Character character)
		{
			Debug.Log("buff " + name + "'s effect deactivate");

			foreach (EffectAmount effectAmount in effectAmountList)
			{
				yield return effectAmount.Deactivate(character);
			}
		}

		public eStartTime StartTime
		{
			get
			{
				return startTime;
			}
		}

		public eDuration Duration
		{
			get
			{
				return duration;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public bool IsRestore
		{
			get
			{
				return isRestore;
			}
		}

		public EffectAmount[] EffectAmountList
		{
			get
			{
				return effectAmountList;
			}
		}

		public int AliveDays
		{
			get
			{
				return aliveDays;
			}

			set
			{
				aliveDays = value;
			}
		}

		public bool CheckDuration()
		{
			if(Duration == eDuration.DAY && this.AliveDays >= 1)
			{
				return true;
			}
			else if (Duration == eDuration.DAY_TEST && this.AliveDays >= 1)
			{
				return true;
			}
			else if(Duration == eDuration.WEEK && this.AliveDays >= 7)
			{
				return true;
			}
			return false;
		}
	}
}
