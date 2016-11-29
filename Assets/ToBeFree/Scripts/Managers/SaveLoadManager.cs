using UnityEngine;
using LitJson;
using System;
using System.IO;
using System.Collections.Generic;

namespace ToBeFree
{
	public class SaveLoadManager : Singleton<SaveLoadManager>
	{
		public Data data;

		private string file;

		[Serializable]
		public class Data : IData
		{
			public Data()
			{
				character = new CharacterSaveData();
				pieceList = new List<PieceSaveData>();
				buffList = new List<BuffSaveData>();
				abnormalList = new List<AbnormalConditionSaveData>();
				questList = new List<QuestSaveData>();
				cityList = new List<CitySaveData>();
				time = new TimeSaveData();
			}

			public CharacterSaveData character;
			public List<PieceSaveData> pieceList;
			public List<BuffSaveData> buffList;
			public List<AbnormalConditionSaveData> abnormalList;
			public List<QuestSaveData> questList;
			public List<CitySaveData> cityList;
			public TimeSaveData time;
		}

		public void Init()
		{
			file = Application.dataPath + "/saveData.json";
			Load();
		}

		public void Save()
		{
			// write
			data = new Data();
			PieceManager.Instance.Save(data.pieceList);
			BuffManager.Instance.Save(data.buffList);
			AbnormalConditionManager.Instance.Save(data.abnormalList);
			QuestManager.Instance.Save(data.questList);
			CityManager.Instance.Save(data.cityList);
			CharacterManager.Instance.Save(data.character);
			TimeTable.Instance.Save(data.time);
			
			//JsonData jsonData = JsonMapper.ToJson(data);
			//Debug.Log(jsonData);
			string s = JsonUtility.ToJson(data);
			Debug.Log(s);
			string file = Application.dataPath + "/saveData.json";
			File.WriteAllText(file, s);
		}

		public void Load()
		{
			// read
			StreamReader reader = new StreamReader(file);
			string json = reader.ReadToEnd();

			data = JsonUtility.FromJson<Data>(json);
			Debug.Log(data.character.index);

			reader.Close();
		}
	}
}