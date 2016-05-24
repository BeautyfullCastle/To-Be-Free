using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToBeFree
{
    class PieceManager : Singleton<PieceManager>
    {
        private List<Information> informList;
        private List<Police> policeList;

        public PieceManager()
        {
            informList = new List<Information>();
            policeList = new List<Police>();
        }

        public List<Information> InformList
        {
            get
            {
                return informList;
            }

            set
            {
                informList = value;
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

        public Piece GetRand(string type)
        {
            Random r = new Random();

            if(type == "INFORM")
            {
                int randI = r.Next(0, informList.Count);
                return informList[randI] as Piece;
            }
            else if(type == "POLICE")
            {
                int randI = r.Next(0, policeList.Count);
                return policeList[randI] as Piece;
            }
            
            return null;
        }

        public Piece GetLast(string type)
        {
            if(type == "INFORM")
            {
                return informList[informList.Count - 1] as Piece;
            }
            if(type == "POLICE")
            {
                return policeList[policeList.Count - 1] as Piece;
            }

            return null;
        }

        public Piece GetFirst(string type)
        {
            if (type == "INFORM")
            {
                return informList[0] as Piece;
            }
            if (type == "POLICE")
            {
                return policeList[0] as Piece;
            }

            return null;
        }

        public void Delete(Piece piece, string type)
        {
            if(type == "INFORM")
            {
                informList.Remove(piece as Information);
            }
            else if(type == "POLICE")
            {
                policeList.Remove(piece as Police);
            }
        }
    }
}
