using System;
using System.Collections;
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

                Character character = new Character(data.name, data.script, stat, data.startCity, inven);

                for (int i = 0; i < data.itemIndex.Length; ++i)
                {
                    Item item = new Item(ItemManager.Instance.List[data.itemIndex[i]]);
                    character.Inven.list.Add(item);
                    character.Stat.AddFood(item);
                    GameObject.FindObjectOfType<UIInventory>().AddItem(item);
                }

                if (List[data.index] != null)
                {
                    throw new Exception("Character data.index " + data.index + " is duplicated.");
                }
                List[data.index] = character;
            }
        }
    }
}
