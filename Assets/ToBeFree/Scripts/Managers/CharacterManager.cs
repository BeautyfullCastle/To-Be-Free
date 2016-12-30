using System;
using System.Collections.Generic;
using Language;
using UnityEngine;

namespace ToBeFree
{
	public class CharacterManager : Singleton<CharacterManager>
	{
		private Character[] list;
		private CharacterData[] dataList;
		private string file = Application.streamingAssetsPath + fileName;
		private const string fileName = "/Character.json";
		private Language.CharacterData[] engList;
		private Language.CharacterData[] korList;
		private List<Language.CharacterData[]> languageList;
				
		public void Init()
		{
			DataList<CharacterData> cDataList = new DataList<CharacterData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Character[dataList.Length];

			engList = new DataList<Language.CharacterData>(Application.streamingAssetsPath + "/Language/English" + fileName).dataList;
			korList = new DataList<Language.CharacterData>(Application.streamingAssetsPath + "/Language/Korean" + fileName).dataList;
			languageList = new List<Language.CharacterData[]>(2);
			languageList.Add(engList);
			languageList.Add(korList);

			LanguageSelection.selectLanguage += ChangeLanguage;

			ParseData();
		}

		private void ParseData()
		{
			foreach (CharacterData data in dataList)
			{
				Stat stat = new Stat(data.HP, data.strength, data.agility, data.concentration, data.talent, data.startMoney);
				Inventory inven = new Inventory(data.startInven);

				Character character = new Character(data.index, data.name, data.script, stat, data.startCity, inven, data.eventIndex, data.skillScript, data.abnormalIndex);

				for (int i = 0; i < data.itemIndex.Length; ++i)
				{
					Item item = new Item(ItemManager.Instance.GetByIndex(data.itemIndex[i]));
					character.Inven.list.Add(item);
				}

				if (list[data.index] != null)
				{
					throw new Exception("Character data.index " + data.index + " is duplicated.");
				}
				list[data.index] = character;
			}
		}

		public void ChangeLanguage(eLanguage language)
		{
			foreach (Language.CharacterData data in languageList[(int)language])
			{
				try
				{
					Character character = GetByIndex(data.index);
					if (character == null)
						continue;

					character.Name = data.name;
				}
				catch (Exception e)
				{
					Debug.LogError(data.index.ToString() + " : " + e);
				}
			}
			GameManager.Instance.uiCharacter.Refresh();
		}

		public void Save(CharacterSaveData data)
		{
			Character character = GameManager.Instance.Character;
			data.index = character.Index;
			data.stat = new StatSaveData(character.Stat);
			data.inventory = new List<ItemSaveData>();
			data.maxSlot = character.Inven.MaxSlot;
			for (int i = 0; i < character.Inven.list.Count; ++i)
			{
				Item item = character.Inven.list[i];
				data.inventory.Add(new ItemSaveData(item.Index));
			}
			data.specialEventProbability = character.SpecialEventProbability;
			data.caughtPolicePieceIndex = PieceManager.Instance.List.IndexOf(character.CaughtPolice);
			data.curCityIndex = character.CurCity.Index;
			data.isDetention = character.IsDetention;
			data.isActionSkip = character.IsActionSkip;

			SaveLoadManager.Instance.data.character = data;
		}

		public Character Load(CharacterSaveData data)
		{
			Character character = this.list[data.index];
			character.Stat = new Stat(data.stat);
			character.Inven = new Inventory(data.inventory, data.maxSlot);

			character.SpecialEventProbability = data.specialEventProbability;
			if (data.caughtPolicePieceIndex == -1)
			{
				character.CaughtPolice = null;
			}
			else
			{
				character.CaughtPolice = PieceManager.Instance.List[data.caughtPolicePieceIndex] as Police;
			}
			character.CurCity = CityManager.Instance.EveryCity[data.curCityIndex];
			character.IsDetention = data.isDetention;
			character.IsActionSkip = data.isActionSkip;

			return character;
		}

		public Character GetByIndex(int index)
		{
			if(index < 0 || index >= list.Length)
			{
				return null;
			}
			return list[index];
		}

		public Language.CharacterData GetLanguageData(eLanguage language)
		{
			Language.CharacterData[] languageList = null;
			if(language == eLanguage.ENGLISH)
			{
				languageList = engList;
			}
			else
			{
				languageList = korList;
			}

			int index = GameManager.Instance.Character.Index;
			if(index < 0 || index >= languageList.Length)
			{
				return null;
			}

			return languageList[index];
		}
	}
}
