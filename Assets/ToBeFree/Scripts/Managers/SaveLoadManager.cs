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
			}

			public CharacterSaveData character;
			public List<PieceSaveData> pieceList;
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
			GameManager.Instance.Character.Save(data.character);
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
		}
	}
}