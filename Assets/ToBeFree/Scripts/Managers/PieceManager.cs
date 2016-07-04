using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class PieceManager : Singleton<PieceManager>
    {
        private List<Piece> list;

        
        public PieceManager()
        {
            list = new List<Piece>();
        }
        
        public Piece FindRand(eSubjectType type)
        {
            List<Piece> specificTypelist = FindAll(type);

            System.Random r = new System.Random();
            int index = r.Next(0, list.Count);
            return list[index];
        }

        public List<Piece> FindAll(eSubjectType type)
        {
            return list.FindAll(x => x.SubjectType == type);
        }

        public Piece GetLast(eSubjectType subjectType)
        {
            return list.FindLast(x => x.SubjectType == subjectType);
        }

        public Piece GetLast()
        {
            return list[list.Count - 1];
        }

        public Piece GetFirst(eSubjectType subjectType)
        {
            if (subjectType == eSubjectType.POLICE)
            {
                return list.Find(x => x is Police);
            }
            else if (subjectType == eSubjectType.INFO)
            {
                return list.Find(x => x is Information);
            }
            else if (subjectType == eSubjectType.QUEST)
            {
                return list.Find(x => x is QuestPiece);
            }
            else
            {
                throw new Exception("subjectType is wrong>");
            }
        }

        public Piece GetFirst()
        {
            return list[0];
        }
        
        // ******* Delete **************

        public void Delete(Piece piece)
        {
            if (piece == null)
            {
                Debug.LogError("piece is null");
                return;
            }
            list.Remove(piece);
        }
        
        // ********* Add ***********
        public void Add(Piece piece)
        {
            list.Add(piece);
        }

        public List<Piece> List
        {
            get
            {
                return list;
            }
        }
    }
}