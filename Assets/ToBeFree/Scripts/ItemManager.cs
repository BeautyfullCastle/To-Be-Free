using System.Collections.Generic;

namespace ToBeFree
{
    public class ItemManager : Singleton<ItemManager>
    {
        private List<Item> itemList;

        public ItemManager()
        {
            itemList = new List<Item>();
        }

        public Item GetRand()
        {
            System.Random r = new System.Random();
            int index = r.Next(0, itemList.Count - 1);
            return itemList[index];
        }

        public Item GetTagRand(int iTag)
        {
            // TO DO : compare tag list to iTag
            //List<Item> tagItems = itemList.FindAll(x => x.Tag == ToString(iTag));
            //System.Random r = new System.Random();
            //int index = r.Next(0, tagItems.Count - 1);
            //return tagItems[index];

            return null;
        }

        public Item GetByIndex(int index)
        {
            return itemList[index];
        }

        public Item GetByType(string bigType, string middleType, string detailType)
        {
            foreach (Item item in itemList)
            {
                if (item.Effect.BigType == bigType && item.Effect.MiddleType == middleType
                    && item.Effect.DetailType == detailType)
                {
                    return item;
                }
            }
            return null;
        }
    }
}