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
		public static event AddDeletePieceHandler AddPiece = delegate { };
		public static event AddDeletePieceHandler DeletePiece = delegate { };


		public PieceManager()
		{
			list = new List<Piece>();
		}

		public void Reset()
		{
			foreach(Piece piece in list)
			{
				if (piece.IconPiece == null)
					continue;

				GameObject.Destroy(piece.IconPiece.gameObject);
			}
			list.Clear();
		}

		public void Save(List<PieceSaveData> pieceList)
		{
			for(int i=0; i<list.Count; ++i)
			{
				PieceSaveData data = new PieceSaveData();
				Piece piece = list[i];
				if (piece == null)
				{
					continue;
				}
				data.type = EnumConvert<eSubjectType>.ToString(piece.SubjectType);
				data.cityIndex = piece.City.Index;
				if (piece.SubjectType == eSubjectType.POLICE)
				{
					Police policePiece = piece as Police;
					data.power = policePiece.Power;
					data.movement = policePiece.Movement;
				}
				else if(piece.SubjectType == eSubjectType.QUEST)
				{
					QuestPiece questPiece = piece as QuestPiece;
				}

				pieceList.Add(data);
			}
		}

		public void Load(List<PieceSaveData> pieceList)
		{
			for (int i = 0; i < pieceList.Count; ++i)
			{
				eSubjectType type = EnumConvert<eSubjectType>.ToEnum(pieceList[i].type);
				City city = CityManager.Instance.GetbyIndex(pieceList[i].cityIndex);
				if (type == eSubjectType.POLICE)
				{
					Police police = new Police(city, type, pieceList[i].power, pieceList[i].movement);
					this.list.Add(police);
				}
				else if (type == eSubjectType.QUEST)
				{
					QuestPiece piece = new QuestPiece(city, type);
					this.list.Add(piece);
				}
				else if(type == eSubjectType.BROKER)
				{
					Broker broker = new Broker(city, type);
					this.list.Add(broker);
				}
			}
		}

		public Piece Find(eSubjectType type, City city)
		{
			if (city == null)
				return null;

			Predicate<Piece> match = (x => x.City.Index == city.Index && x.SubjectType == type );
			if (list.Exists(match))
			{
				return list.Find(match);
			}
			return null;
		}

		public Piece FindRand(eSubjectType type)
		{
			List<Piece> specificTypelist = FindAll(type);
			
			int index = UnityEngine.Random.Range(0, specificTypelist.Count);
			return specificTypelist[index];
		}

		public List<Piece> FindAll(eSubjectType type)
		{
			return list.FindAll(x => x.SubjectType == type);
		}

		public List<Piece> FindAll(eSubjectType type, City city)
		{
			List<Piece> pieces = list.FindAll(x => x.SubjectType == type);
			return pieces.FindAll(x => x.City == city);
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
			if (piece.IconPiece.gameObject != null)
			{
				GameManager.DestroyImmediate(piece.IconPiece.gameObject);
			}	
			DeletePiece(piece);
		}

		public void Add(eSubjectType subjectType)
		{
			Piece piece = null;
			City city = CityManager.Instance.FindRand(subjectType);
			if(subjectType == eSubjectType.POLICE)
			{
				piece = new Police(city, subjectType);
			}
			else if(subjectType == eSubjectType.BROKER)
			{
				piece = new Broker(city, subjectType);
			}
			else
			{
				Debug.LogError("Can't Add this piece type : " + EnumConvert<eSubjectType>.ToString(subjectType));
			}
		}

		// ********* Add ***********
		public void Add(Piece piece)
		{
			if (piece.City == null)
			{
				return;
			}
			list.Add(piece);
			AddPiece(piece);

			if (GameManager.Instance.Character != null)
			{
				GameManager.Instance.Character.Stat.SetViewRange();
			}
		}

		public int GetNumberOfPiece(eSubjectType pieceType, City city)
		{
			Predicate<Piece> match = (x => x.City == city && x.SubjectType == pieceType);
			if (list.Exists(match))
			{
				var findedList = list.FindAll(match);
				return findedList.Count;
			}
			return 0;
		}

		public Piece GetPieceOfCity(eSubjectType pieceType, City city)
		{
			Predicate<Piece> match = (x => x.City == city && x.SubjectType == pieceType);
			if (list.Exists(match))
			{
				return list.Find(match);
			}
			return null;
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