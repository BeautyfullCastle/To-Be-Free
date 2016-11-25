using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class BuffManager : Singleton<BuffManager>
	{
		private List<Buff> buffList;

		public delegate void UpdateListHandler(Buff buff);
		static public event UpdateListHandler AddedBuff;
		static public event UpdateListHandler DeletedBuff;

		public BuffManager()
		{
			buffList = new List<Buff>();

			TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;
		}
		
		public void Save(List<BuffSaveData> dataList)
		{
			for(int i=0; i<buffList.Count; ++i)
			{
				BuffSaveData data = new BuffSaveData(i, AbnormalConditionManager.Instance.Find(buffList[i].Name).Index, buffList[i].AliveDays);
				dataList.Add(data);
			}
		}

		public IEnumerator Load(List<BuffSaveData> dataList)
		{
			for(int i=0; i<dataList.Count; ++i)
			{
				Buff buff = AbnormalConditionManager.Instance.List[dataList[i].abnormalIndex].Buff;
				yield return this.Add(buff);
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

		public IEnumerator Add(Buff buff)
		{
			if (buffList == null || buff == null)
			{
				yield break;
			}
			buffList.Add(buff);
			//AddedBuff(buff);
			GameObject.Find("BuffManager").GetComponent<UIBuffManager>().AddBuff(buff);
			Debug.Log(buff.Name + " is added to buff list.");

			yield return ActivateEffectByStartTime(eStartTime.NOW, GameManager.Instance.Character);

			yield return null;
		}

		public IEnumerator Delete(Buff buff, Character character)
		{
			if (buffList == null || buffList.Count == 0 || buff == null)
			{
				yield break;
			}
			
			DeletedBuff(buff);
			buffList.Remove(buff);

			yield return null;
		}
		
		public IEnumerator ActivateEffectByStartTime(eStartTime startTime, Character character)
		{
			yield return character.Inven.CheckItem(startTime, true, character);

			yield return GameManager.Instance.ShowStateLabel("Activate Effect : " + startTime.ToString(), 0.5f);
			
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

			yield return GameManager.Instance.ShowStateLabel("DeActivate Effect : " + startTime.ToString(), 0.5f);

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
			yield return GameManager.Instance.ShowStateLabel("Check Buff's duration", 0.5f);

			List<Buff> buffsToDelete = new List<Buff>();
			foreach (Buff buff in buffList)
			{
				if (buff.CheckDuration())
				{
					// DAY_TEST는 매일밤 주사위 하나 굴려서 4 이하면 사라짐.
					if(buff.Duration == eDuration.DAY_TEST)
					{
						GameManager.Instance.uiEventManager.OpenUI();
						GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.EVENT, "DAY_TEST for " + buff.Name);
						yield return EventManager.Instance.WaitUntilFinish();

						int resultNum = 0;
						yield return DiceTester.Instance.Test(1, x => resultNum = x);

						GameManager.Instance.uiEventManager.OpenUI();
						GameManager.Instance.uiEventManager.OnChanged(eUIEventLabelType.DICENUM, resultNum.ToString());
						yield return EventManager.Instance.WaitUntilFinish();

						if (resultNum > 0)
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
				
				yield return AbnormalConditionManager.Instance.Find(buff.Name).DeActivate(character);
			}
			yield return null;
		}

		public IEnumerator Rest_Cure_PatienceTest(Character character, int testSuccessNum)
		{
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
							yield return buff.DeactivateEffect(character);
							yield return this.Delete(buff, character);
							testSuccessNum--;
						}
					}
				}

				character.Stat.HP += testSuccessNum;
			}

			yield return EventManager.Instance.WaitUntilFinish();
			
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