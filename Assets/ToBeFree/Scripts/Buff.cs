using System;
using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public enum eBuff
	{
		NAME, AMOUNT, STACK
	}

	[Serializable]
	public class BuffSaveData
	{
		public BuffSaveData(int index, int abnormalIndex, int aliveDays)
		{
			this.index = index;
			this.abnormalIndex = abnormalIndex;
			this.aliveDays = aliveDays;
		}

		public int index;
		public int abnormalIndex;
		public int aliveDays;
	}

	public class Buff
	{
		private readonly int index;
		private readonly string name;
		private string script;
		private readonly EffectAmount[] effectAmountList;
		private readonly bool isRestore;
		private readonly eStartTime startTime;
		private readonly eDuration duration;

		private int aliveDays;

		
		public Buff(int index, string name, string script, EffectAmount[] effectAmountList, bool isRestore,
			eStartTime startTime, eDuration duration)
		{
			this.index = index;
			this.name = name;
			this.script = script;
			this.effectAmountList = effectAmountList;
			this.isRestore = isRestore;
			this.startTime = startTime;
			this.duration = duration;
		}

		public Buff(Buff buff) : this(buff.index, buff.name, buff.script, buff.effectAmountList, buff.isRestore,
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

		public string Script
		{
			get
			{
				return script;
			}
			set
			{
				script = value;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public bool CheckDuration()
		{
			if(Duration == eDuration.DAY && this.AliveDays >= 2)
			{
				return true;
			}
			else if (Duration == eDuration.TODAY && this.AliveDays >= 1)
			{
				return true;
			}
			else if (Duration == eDuration.DAY_TEST && this.AliveDays >= 2)
			{
				return true;
			}
			
			return false;
		}
	}
}
