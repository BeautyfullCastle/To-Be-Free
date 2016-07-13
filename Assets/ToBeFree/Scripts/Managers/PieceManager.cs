﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class PieceManager : Singleton<PieceManager>
    {
        private List<Piece> list;

        public delegate void AddDeletePieceHandler(Piece piece);
        public static event AddDeletePieceHandler AddPiece;
        public static event AddDeletePieceHandler DeletePiece;


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
            return list.Find(x => x.SubjectType == subjectType);
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
            DeletePiece(piece);
        }
        
        // ********* Add ***********
        public void Add(Piece piece)
        {
            list.Add(piece);
            AddPiece(piece);
        }

        public List<Piece> List
        {
            get
            {
                return list;
            }
        }

        public QuestPiece Find(Quest quest)
        {
            foreach(Piece piece in list)
            {
                if(piece.SubjectType != eSubjectType.QUEST)
                {
                    continue;
                }
                QuestPiece questPiece = piece as QuestPiece;
                if(questPiece.CurQuest == quest)
                {
                    return questPiece;
                }
            }
            return null;
        }

        public IEnumerator Move(Piece piece, City city)
        {
            Delete(piece);
            yield return piece.MoveCity(city);
            Add(piece);

        }
    }
}