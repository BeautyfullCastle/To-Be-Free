using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    internal class PieceManager : Singleton<PieceManager>
    {
        private List<Information> infoList;
        private List<Police> policeList;
        private List<Quest> questList;

        public PieceManager()
        {
            infoList = new List<Information>();
            policeList = new List<Police>();
            questList = new List<Quest>();
        }

        public void Init()
        {
            List<City> bigCityList = CityManager.Instance.FindCitiesBySize(eCitySize.BIG);
            foreach (City city in bigCityList)
            {
                this.Add(city, eSubjectType.POLICE);
            }
        }

        // GET ******************************* //
        public Piece GetRand(eSubjectType bigType)
        {
            System.Random r = new System.Random();

            if (bigType == eSubjectType.INFO)
            {
                int randI = r.Next(0, infoList.Count);
                return infoList[randI] as Piece;
            }
            else if (bigType == eSubjectType.POLICE)
            {
                int randI = r.Next(0, policeList.Count);
                return policeList[randI] as Piece;
            }

            return null;
        }

        public Piece GetLast(eSubjectType bigType)
        {
            if (bigType == eSubjectType.INFO)
            {
                return infoList[infoList.Count - 1] as Piece;
            }
            if (bigType == eSubjectType.POLICE)
            {
                return policeList[policeList.Count - 1] as Piece;
            }

            return null;
        }

        public Piece GetFirst(eSubjectType bigType)
        {
            if (bigType == eSubjectType.INFO)
            {
                if (infoList.Count != 0)
                {
                    return infoList[0] as Piece;
                }
            }
            if (bigType == eSubjectType.POLICE)
            {
                if (policeList.Count != 0)
                {
                    return policeList[0] as Piece;
                }
            }

            return null;
        }

        // ******* Delete **************
        public void Delete(Piece piece)
        {
            if (piece == null)
            {
                Debug.LogError("piece is null");
                return;
            }
            if (piece is Information)
            {
                infoList.Remove(piece as Information);
            }
            else if (piece is Police)
            {
                policeList.Remove(piece as Police);
            }
            else if (piece is Quest)
            {
                questList.Remove(piece as Quest);
            }
        }

        // ********* Add ***********
        public Piece Add(City city, eSubjectType bigType)
        {
            Piece piece = null;
            if (bigType == eSubjectType.INFO)
            {
                piece = new Information(city);
                infoList.Add((Information)piece);
            }
            else if (bigType == eSubjectType.POLICE)
            {
                piece = new Police(city);
                policeList.Add((Police)piece);
            }

            if (piece == null)
            {
                Debug.LogError(bigType + " is wrong : Piece Add(..) ");
            }
            return piece;
        }

        public Quest AddQuest(City city, Character character, Event curEvent)
        {
            Quest quest = new Quest(curEvent, character, city);
            questList.Add(quest);

            return quest;
        }

        public List<Information> InformList
        {
            get
            {
                return infoList;
            }

            set
            {
                infoList = value;
            }
        }

        public List<Police> PoliceList
        {
            get
            {
                return policeList;
            }

            set
            {
                policeList = value;
            }
        }

        public List<Quest> QuestList
        {
            get
            {
                return questList;
            }

            set
            {
                questList = value;
            }
        }
    }
}