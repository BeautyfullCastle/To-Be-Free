using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class ItemManager : Singleton<ItemManager>
    {
        private readonly Item[] list;
        private readonly ItemData[] dataList;
        private readonly string file = Application.streamingAssetsPath + "/Item.json";

        
        public ItemManager()
        {
            DataList<ItemData> cDataList = new DataList<ItemData>(file);
            dataList = cDataList.dataList;
            if (dataList == null)
                return;

            list = new Item[dataList.Length];

            ParseData();
        }

        private void ParseData()
        {
            foreach (ItemData data in dataList)
            {
                EffectAmount[] effectAmountList = new EffectAmount[data.effectIndexList.Length];
                for (int i = 0; i < data.effectIndexList.Length; ++i)
                {
                    if(data.effectIndexList[i] == -99)
                    {
                        continue;
                    }
                    EffectAmount effectAmount = new EffectAmount(EffectManager.Instance.List[data.effectIndexList[i]], data.amountList[i]);
                    effectAmountList[i] = effectAmount;
                }

                Buff buff = new Buff(data.name, effectAmountList, bool.Parse(data.restore),
                                        EnumConvert<eStartTime>.ToEnum(data.startTime), EnumConvert<eDuration>.ToEnum(data.duration));

                Item item = new Item(data.name, buff, data.price);

                if (list[data.index] != null)
                {
                    throw new Exception("Item data.index " + data.index + " is duplicated.");
                }
                list[data.index] = item;
            }
        }

        public Item GetRand()
        {
            System.Random r = new System.Random();
            int index = r.Next(0, list.Length - 1);
            return list[index];
        }

        public Item GetTagRand(int iTag)
        {
            // TO DO : compare tag list to iTag
            //List<Item> tagItems = list.FindAll(x => x.Tag == ToString(iTag));
            //System.Random r = new System.Random();
            //int index = r.Next(0, tagItems.Count - 1);
            //return tagItems[index];

            return null;
        }

        internal void Add(Item item)
        {
            //this.list.Add(item.DeepCopy());
        }

        public Item GetByIndex(int index)
        {
            return list[index];
        }

        public Item GetByType(eSubjectType bigType, eVerbType middleType, eObjectType detailType)
        {
            //foreach (Item item in list)
            //{
            //    if (item.Buff.EffectAmount.SubjectType == bigType && item.Buff.Effect.VerbType == middleType
            //        && item.Buff.Effect.ObjectType == detailType)
            //    {
            //        return item;
            //    }
            //}
            return null;
        }

        public Item[] List
        {
            get
            {
                return list;
            }
        }
    }
}