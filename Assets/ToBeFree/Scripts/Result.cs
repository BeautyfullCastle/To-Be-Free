using System.Collections;
using UnityEngine;

namespace ToBeFree
{
	public class Result
	{
		private eTestStat testStat;
		private ResultScriptAndEffects success;
		private ResultScriptAndEffects failure;

		public Result(eTestStat testStat, ResultScriptAndEffects success, ResultScriptAndEffects failure)
		{
			this.testStat = testStat;
			this.success = success;
			this.failure = failure;
		}

		public eTestStat TestStat
		{
			get
			{
				return testStat;
			}
		}

		public ResultScriptAndEffects Success
		{
			get
			{
				return success;
			}
		}

		public ResultScriptAndEffects Failure
		{
			get
			{
				return failure;
			}
		}
	}

	public class ResultScriptAndEffects
	{
		private string script;
		private EffectAmount[] effects;

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

		public EffectAmount[] EffectAmounts
		{
			get
			{
				return effects;
			}
			set
			{
				effects = value;
			}
		}

		public ResultScriptAndEffects(string script, EffectAmount[] effects)
		{
			this.script = script;
			this.effects = effects;
		}
	}

	public class EffectAmount
	{
		private Effect effect;
		private int amount;
		

		public EffectAmount(Effect effect, int amount=0)
		{
			this.effect = effect;
			this.amount = amount;
		}

		public IEnumerator Activate(Character character)
		{
			if(effect==null)
			{
				Debug.LogError("effect is null.");
				yield break;
			}
			yield return effect.Activate(character, amount);
		}

		public IEnumerator Deactivate(Character character)
		{
			if (effect == null)
			{
				Debug.LogError("effect is null.");
				yield break;
			}
			yield return effect.Deactivate(character, amount);
		}

		public override string ToString()
		{
			if (effect == null)
				return string.Empty;

			// 수치
			// character add hp, food, info
			// stat add strength, agility, concentration, talent, all
			// money add specific, rand_3
			// 이름
			// item add, del
			// abnormal add, del
			string strAmount = string.Empty;
			switch(effect.SubjectType)
			{
				case eSubjectType.CHARACTER:
					switch (effect.ObjectType)
					{
						case eObjectType.HP:
						case eObjectType.FOOD:
						case eObjectType.INFO:
							strAmount = this.amount.ToString();
							break;
					}
					break;
				case eSubjectType.STAT:
				case eSubjectType.MONEY:
					strAmount = this.amount.ToString();
					break;
				case eSubjectType.ITEM:
					Item item = ItemManager.Instance.GetByIndex(this.amount);
					if(item != null)
					{
						strAmount = item.Name;
					}
					break;
				case eSubjectType.ABNORMAL:
					AbnormalCondition abnormal = AbnormalConditionManager.Instance.GetByIndex(this.amount);
					if(abnormal != null)
					{
						strAmount = abnormal.Name;
					}
					break;
			}
			
			return effect.ToString() + strAmount;
		}

		public int Amount
		{
			get
			{
				return amount;
			}
			set
			{
				if(amount == -99)
				{
					return;
				}
				amount = value;
			}
		}

		public Effect Effect
		{
			get
			{
				return effect;
			}
		}

	}
}