using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public enum eLanguage
    {
        ENGLISH, KOREAN
    }

    public class LanguageManager<T> : Singleton<LanguageManager<T>> where T : IData
    {
        private T[] koreanDataList;
        private T[] englishDataList;
        private readonly string file = "City.json";

        public T[] EnglishDataList
        {
            get
            {
                return englishDataList;
            }
        }

        public T[] KoreanDataList
        {
            get
            {
                return koreanDataList;
            }
        }

        public LanguageManager()
        {
            
        }

        public void Init()
        {
            DataList<T> cDataList = new DataList<T>(Application.streamingAssetsPath + "/Language/Korean/" + file);
            koreanDataList = cDataList.dataList;
            DataList<T> cEngDataList = new DataList<T>(Application.streamingAssetsPath + "/Language/" + "English/" + file);
            englishDataList = cEngDataList.dataList;
        }
    }
}