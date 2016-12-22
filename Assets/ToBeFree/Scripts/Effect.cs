using System.Collections;
using UnityEngine;
using System;

namespace ToBeFree
{
	public enum eSubjectType
	{
		STAT, DICE,
		INFO, POLICE, CHARACTER,
		ITEM, MONEY, SATIETY,
		ACTINGPOWER, ABNORMAL,
		COMMAND,
		EVENT, QUEST,
		ROOT,
		BROKER,
		MENTAL,
		NULL,
		ACTION,
		RESULT,
		DDAY,
		FOOD,
		ENDING
	}

	public enum eVerbType
	{
		NONE,
		ADD, DEL, MOVE,
		DEACTIVE,
		SKIP, LOAD, OPEN,
		REROLL,
		IN,
		SET,
		NULL,
		SUCCESS,
		REVEAL
	}

	// 목적어
	public enum eObjectType
	{
		NONE,
		HP,
		FAR_CLOSE, CLOSE_FAR, RAND_RAND, RAND_CLOSE, CLOSE_CLOSE,
		CLOSE, FAR, RAND, 
		ALL, TAG, INDEX, SELECT, SELECT_ALL, SELECT_TAG,
		WORK, MOVE, REST, REST_CURE, SPECIAL, INFO, FOOD, SHOP, INVESTIGATION,
		STRENGTH, AGILITY, CONCENTRATION, TALENT,
		SOUTHEAST_ASIA, MONGOLIA,
		SPECIFIC, RAND_3,
		SUCCESSNUM,
		INVEN,
		DETENTION,
		NULL,
		CANCEL,
		VIEWRANGE,
		NUMBER,
		CRACKDOWN_PROBABILITY,
		MENTAL, HP_MENTAL, PATIENCE, LUCK, // 지워야 되는 것들
		DICE
	}

	public class Effect
	{
		readonly private int index;
		readonly private eSubjectType subjectType;
		readonly private eVerbType verbType;
		readonly private eObjectType objectType;
		private string script;
		
		public delegate void DeactiveEventHandler(eCommand commandType, bool deactive);
		public static event DeactiveEventHandler DeactiveEvent;

		public Effect(int index, eSubjectType subjectType, eVerbType verbType, eObjectType objectType, string script)
		{
			this.index = index;
			this.subjectType = subjectType;
			this.verbType = verbType;
			this.objectType = objectType;
			this.script = script;
		}

		public Effect(Effect effect) : this(effect.index, effect.subjectType, effect.verbType, effect.objectType, effect.script)
		{
		}
		
		public override string ToString()
		{
			if(script == "NULL")
			{
				return string.Empty;
			}
			return script;
		}

		public IEnumerator Activate(Character character, int amount)
		{
			switch (subjectType)
			{
				case eSubjectType.CHARACTER:
					yield return character.Activate(verbType, objectType, amount);
					break;
					
				case eSubjectType.STAT:
					if (verbType == eVerbType.ADD)
					{
						character.Stat.Set(objectType, amount);
					}
					break;

				case eSubjectType.POLICE:
					if (verbType == eVerbType.REVEAL)
					{
						// reveal number of polices in this position
						if (objectType == eObjectType.NUMBER)
						{
							yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_PoliceRevealNumber));

							foreach (IconCity c in GameManager.Instance.iconCities)
							{
								c.SetEnable(true);
							}

							GameManager.Instance.revealPoliceNum = false;
							while (GameManager.Instance.revealPoliceNum == false)
							{
								yield return null;
							}

							City city = GameManager.Instance.ClickedIconCity.City;
							int policeNumInClickedCity = PieceManager.Instance.FindAll(eSubjectType.POLICE, city).Count;
							
							yield return GameManager.Instance.uiEventManager.OnChanged(LanguageManager.Instance.Find(eLanguageKey.Event_PoliceNumber) + " : " + policeNumInClickedCity);
						}
						// reveal police's crackdown probability
						else if (objectType == eObjectType.CRACKDOWN_PROBABILITY)
						{
							yield return GameManager.Instance.uiEventManager.OnChanged(this.ToString() + " : " + CrackDown.Instance.Probability + "%");
						}
					}
					break;
					
				case eSubjectType.ITEM:
					Item item = ItemManager.Instance.GetByType(objectType, amount);
					if (item == null)
					{
						throw new System.Exception("item is null");
					}

					if (verbType == eVerbType.ADD)
					{
						character.Inven.AddItem(item);
					}
					else if (verbType == eVerbType.DEL)
					{
						yield return character.Inven.Delete(item, character);
					}
					break;
					
				case eSubjectType.MONEY:
					if (verbType == eVerbType.ADD)
					{
						if (objectType == eObjectType.SPECIFIC)
						{
							character.Stat.Money += amount;
						}
						// can add more : RAND ?
						else if (objectType == eObjectType.RAND_3)
						{
							int middleMoney = 3;
							System.Random r = new System.Random();
							int money = r.Next(-middleMoney, middleMoney) + amount;
							character.Stat.Money += money;
						}
					}
					break;
					
				case eSubjectType.COMMAND:
					if (verbType == eVerbType.DEACTIVE)
					{
						// other commands.
						DeactiveEvent(EnumConvert<eCommand>.ToEnum(objectType.ToString()), true);
					}
					break;
					
				case eSubjectType.DICE:
					if (verbType == eVerbType.SET)
					{
						if (objectType == eObjectType.SUCCESSNUM)
						{
							DiceTester.Instance.MinSuccessNum = amount;
						}
					}
					else if(verbType == eVerbType.ADD)
					{
						DiceTester.Instance.AdditionalDie = amount;
					}
					break;
				case eSubjectType.ACTION:
					if(verbType == eVerbType.SKIP)
					{
						if (objectType == eObjectType.ALL)
						{
							character.IsActionSkip = true;
						}
					}
					break;
				case eSubjectType.EVENT:
					// later.
					//if (verbType == eVerbType.SKIP)
					//{
					//    // fix. to make buff for skip event and make it succeed.
					//    SkipEvent(EnumConvert<eObjectType>.ToString(objectType));
					//    if (objectType == eObjectType.WORK)
					//    { 
					//    }
					//    else if (objectType == eObjectType.MOVE) { }
					//    else if (objectType == eObjectType.INFO) { }
					//}
					if(verbType == eVerbType.SKIP)
					{
						if (objectType == eObjectType.FOOD)
						{
							character.IsFull = true;
						}
						// can't cure when rest event activated
						else if (objectType == eObjectType.REST_CURE)
						{
							character.CantCure = true;
						}
					}
					else if (verbType == eVerbType.LOAD)
					{
						if (amount > EventManager.Instance.List.Length)
						{
							Debug.LogError("Event index " + amount + " is larger than events' length.");
							yield break;
						}
						yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[amount], character);
					}
					break;
				case eSubjectType.QUEST:
					if(amount > QuestManager.Instance.List.Length)
					{
						Debug.LogError("Quest index " + amount + " is larger than events' length.");
						yield break;
					}

					Quest quest = QuestManager.Instance.List[amount];
					if (verbType == eVerbType.LOAD)
					{
						yield return QuestManager.Instance.Load(quest, character);
					}
					else if(verbType == eVerbType.DEL)
					{
						GameObject.FindObjectOfType<UIQuestManager>().DeleteQuest(quest);
					}
					break;
				case eSubjectType.RESULT:
					if(verbType == eVerbType.LOAD)
					{
						if (amount > ResultManager.Instance.List.Length)
						{
							Debug.LogError("Result index " + amount + " is larger than events' length.");
							yield break;
						}
						Result result = ResultManager.Instance.List[amount];
						yield return EventManager.Instance.CalculateTestResult(result.TestStat, character);
						yield return EventManager.Instance.TreatResult(result, character);
					}
					break;
				case eSubjectType.ABNORMAL:
					AbnormalCondition abnormalCondition = AbnormalConditionManager.Instance.GetByIndex(amount);
					if (abnormalCondition == null)
						yield break;

					if (verbType == eVerbType.ADD)
					{
						yield return abnormalCondition.Activate(character);
					}
					else if (verbType == eVerbType.DEL)
					{
						yield return abnormalCondition.DeActivate(character);
					}
					break;
				case eSubjectType.ENDING:
					if(verbType == eVerbType.LOAD)
					{
						// (HP=0,) 북송, 해피엔딩
						yield return GameManager.Instance.endingManager.StartEnding((eEnding)amount);
					}
					break;
				default:
					break;
			}
			yield return null;
		}

		public IEnumerator Deactivate(Character character)
		{
			switch (subjectType)
			{
				case eSubjectType.DICE:
					if (verbType == eVerbType.SET)
					{
						if (objectType == eObjectType.SUCCESSNUM)
						{
							DiceTester.Instance.MinSuccessNum = DiceTester.Instance.PrevMinSuccessNum;
						}
					}
					else if (verbType == eVerbType.ADD)
					{
						DiceTester.Instance.AdditionalDie = 0;
					}
					break;
				case eSubjectType.STAT:
					if (verbType == eVerbType.ADD)
					{
						character.Stat.Restore(objectType);
					}
					break;
				case eSubjectType.EVENT:
					if (verbType == eVerbType.SKIP)
					{
						if (objectType == eObjectType.FOOD)
						{
							character.IsFull = false;
						}
						else if (objectType == eObjectType.REST_CURE)
						{
							character.CantCure = false;
						}
					}
					break;
				case eSubjectType.CHARACTER:
					yield return character.Deactivate(verbType, objectType);
					break;
				case eSubjectType.COMMAND:
					if (verbType == eVerbType.DEACTIVE)
					{
						// other commands.
						DeactiveEvent(EnumConvert<eCommand>.ToEnum(objectType.ToString()), false);
					}
					break;
			}
			yield return null;
		}
		
		public eSubjectType SubjectType { get { return subjectType; } }

		public eVerbType VerbType { get { return verbType; } }

		public eObjectType ObjectType { get { return objectType; } }

		public int Index
		{
			get
			{
				return index;
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
	}
}