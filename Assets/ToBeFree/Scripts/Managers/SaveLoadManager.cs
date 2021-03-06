﻿using UnityEngine;
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

		private UIButton continueButton;

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
				crackdown = new CrackDownSaveData();
				tipList = new List<TipSaveData>();
			}

			public CharacterSaveData character;
			public List<PieceSaveData> pieceList;
			public List<BuffSaveData> buffList;
			public List<AbnormalConditionSaveData> abnormalList;
			public List<QuestSaveData> questList;
			public List<CitySaveData> cityList;
			public TimeSaveData time;
			public CrackDownSaveData crackdown;
			public List<TipSaveData> tipList;
		}

		public void Init()
		{
			file = Application.streamingAssetsPath + "/saveData.json";
			Load();

			if (continueButton == null)
			{
				continueButton = GameObject.Find("CONTINUE").GetComponent<UIButton>();
			}
			continueButton.isEnabled = data != null;
		}

		public void Save()
		{
			// write
			data = new Data();
			PieceManager.Instance.Save(data.pieceList);
			BuffManager.Instance.Save(data.buffList);
			AbnormalConditionManager.Instance.Save(data.abnormalList);
			GameManager.Instance.uiQuestManager.Save(data.questList);
			CityManager.Instance.Save(data.cityList);
			CharacterManager.Instance.Save(data.character);
			TimeTable.Instance.Save(data.time);
			CrackDown.Instance.Save(data.crackdown);
			TipManager.Instance.Save(data.tipList);

			string s = JsonUtility.ToJson(data);
			Debug.Log(s);
			File.WriteAllText(file, s);
		}

		public void Delete()
		{
			File.WriteAllText(file, "");
		}

		public void Load()
		{
			// read
			StreamReader reader = new StreamReader(file);
			string json = reader.ReadToEnd();

			data = JsonUtility.FromJson<Data>(json);

			reader.Close();
		}
	}
}