using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class BuffManager : Singleton<BuffManager>
	{
		private List<Buff> buffList;

		public BuffManager()
		{
			buffList = new List<Buff>();

			TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;
		}

		public void Reset()
		{
			buffList.Clear();
		}
		
		public void Save(List<BuffSaveData> dataList)
		{
			for(int i=0; i<buffList.Count; ++i)
			{
				BuffSaveData data = new BuffSaveData(i, AbnormalConditionManager.Instance.GetByIndex(buffList[i].Index).Index, buffList[i].AliveDays);
				dataList.Add(data);
			}
		}

		public IEnumerator Load(List<BuffSaveData> dataList)
		{
			for(int i=0; i<dataList.Count; ++i)
			{
				AbnormalCondition abnormalCondition = AbnormalConditionManager.Instance.GetByIndex(dataList[i].abnormalIndex);
				if (abnormalCondition == null)
				{
					yield break;
				}
				
				Buff buff = abnormalCondition.Buff;
				yield return this.Add(buff, abnormalCondition.IsStack);
			}
		}

		private void Instance_NotifyEveryday()
		{
			foreach(Buff buff in buffList)
			{
				buff.AliveDays++;
			}
		}

		public bool Exist(Buff buff)
		{
			return buffList.Exists(x => x == buff);
		}

		public IEnumerator Add(Buff buff, bool isStack)
		{
			if (buffList == null || buff == null)
			{
				yield break;
			}
			buffList.Add(buff);
			GameManager.Instance.uiBuffManager.AddBuff(buff, isStack);
			//Debug.Log(buff.Name + " is added to buff list.");

			yield return ActivateEffectByStartTime(eStartTime.NOW, GameManager.Instance.Character);

			yield return null;
		}

		public IEnumerator Delete(Buff buff, Character character)
		{
			if (buffList == null || buffList.Count == 0 || buff == null)
			{
				yield break;
			}

			GameManager.Instance.uiBuffManager.DeleteBuff(buff);
			buffList.Remove(buff);

			yield return null;
		}
		
		public IEnumerator ActivateEffectByStartTime(eStartTime startTime, Character character)
		{
			yield return character.Inven.CheckItem(startTime, true, character);

			foreach (Buff buff in buffList)
			{
				if (buff.StartTime == startTime)
				{
					yield return buff.ActivateEffect(character);
				}
			}
		}

		public IEnumerator DeactivateEffectByStartTime(eStartTime startTime, Character character)
		{
			yield return character.Inven.CheckItem(startTime, false, character);

			foreach (Buff buff in buffList)
			{
				if (buff.StartTime == startTime)
				{
					yield return buff.DeactivateEffect(character);
				}
			}
		}

		public IEnumerator CheckDuration(Character character)
		{
			List<Buff> buffsToDelete = new List<Buff>();
			foreach (Buff buff in buffList)
			{
				if (buff.CheckDuration())
				{
					// DAY_TEST는 매일밤 주사위 하나 굴려서 4 이하면 사라짐.
					if(buff.Duration == eDuration.DAY_TEST)
					{
						int resultNum = 0;
						yield return DiceTester.Instance.Test(eTestStat.NULL, 1, 0, (x, x1) => { resultNum = x; } );

						if (resultNum <= 4)
						{
							buffsToDelete.Add(buff);
						}
					}
					else
					{
						buffsToDelete.Add(buff);
					}
				}
			}

			foreach (Buff buff in buffsToDelete)
			{
				if(buff.StartTime == eStartTime.NOW)
				{
					yield return buff.DeactivateEffect(character);
				}

				AbnormalCondition condition = AbnormalConditionManager.Instance.GetByIndex(buff.Index);
				if(condition == null)
				{
					Debug.LogError("Can't find buff : " + buff.Name);
					continue;
				}
				yield return condition.DeActivate(character);
			}
			yield return null;
		}

		public IEnumerator Rest_Cure_PatienceTest(Character character, int testSuccessNum)
		{
			string script = string.Empty;
			List<Buff> buffsToDelete = new List<Buff>();
			int curedHP = 0;
			
			if (testSuccessNum > 0)
			{
				// 주사위 개수만큼 버프들부터 제거 후 나머지는 HP 회복에 씀
				List<Buff> buffs = buffList.FindAll(x => x.Duration == eDuration.REST_PATIENCE);
				
				if (buffs != null)
				{
					for (int i = 0; i < buffs.Count; ++i)
					{
						Buff buff = buffs[i];

						if (testSuccessNum > 0)
						{
							buffsToDelete.Add(buff);
							
							testSuccessNum--;
						}
					}
				}
				curedHP = testSuccessNum;
			}

			foreach(Buff buff in buffsToDelete)
			{
				script += buff.Name + "\n";
				yield return buff.DeactivateEffect(character);
				yield return this.Delete(buff, character);
			}
			script += LanguageManager.Instance.Find(eLanguageKey.UI_HP) + ": + " + curedHP;
			character.Stat.HP += curedHP;

			yield return GameManager.Instance.uiEventManager.OnChanged(script, false, true);
		}

		public bool Contains(Buff buff)
		{
			return buffList.Contains(buff);
		}

		public bool Contains(string name)
		{
			return buffList.Contains(buffList.Find(x => x.Name == name));
		}

		public Buff Find(string name)
		{
			return buffList.Find(x => x.Name == name);
		}
	}

	
}