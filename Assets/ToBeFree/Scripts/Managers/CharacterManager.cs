using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class CharacterManager : Singleton<CharacterManager>
	{
		private Character[] list;
		private CharacterData[] dataList;
		private string file = Application.streamingAssetsPath + "/Character.json";

		public Character[] List
		{
			get
			{
				return list;
			}
		}
		
		public void Init()
		{
			DataList<CharacterData> cDataList = new DataList<CharacterData>(file);
			dataList = cDataList.dataList;
			if (dataList == null)
				return;

			list = new Character[dataList.Length];

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
					Item item = new Item(ItemManager.Instance.List[data.itemIndex[i]]);
					character.Inven.AddItem(item, character);
				}

				if (List[data.index] != null)
				{
					throw new Exception("Character data.index " + data.index + " is duplicated.");
				}
				List[data.index] = character;
			}
		}

		public void Save(CharacterSaveData data)
		{
			Character character = GameManager.Instance.Character;
			data.index = character.Index;
			data.stat = new StatSaveData(character.Stat);
			data.inventory = new List<ItemSaveData>();
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
			character.Inven = new Inventory(data.inventory);

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
	}
}
