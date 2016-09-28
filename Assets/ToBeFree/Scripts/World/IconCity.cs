using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
	[SerializeField]
	public enum eNodeType
	{
		NULL, BIGCITY, MIDDLECITY, SMALLCITY, TOWN, MOUNTAIN
	}

	//[ExecuteInEditMode]
	public class IconCity : MonoBehaviour
	{
		public UILabel nameLabel;
		public GameObject questSprite;
		public GameObject brokerSprite;
		public GameObject policeSprite;
		public UILabel policeNumLabel;

		public eNodeType type = eNodeType.TOWN;

		private City city;
		
		public void Init()
		{
			city = new City(gameObject.name, type);						
		}

		public void InitNeighbors()
		{
			if(GetComponent<BezierPoint>() == null)
			{
				Debug.LogError("No BezierPoint in " + this.name);
				return;
			}

			List<City> neighbors = new List<City>();
			foreach (BezierCurveList curveList in CityManager.Instance.Curves)
			{
				if (curveList.Dic.ContainsKey(GetComponent<BezierPoint>()) == false)
				{
					//Debug.LogError("Not contains this point " + this.name + this.transform.parent.name);
					continue;
				}
				List<BezierPoint> points = curveList.Dic[GetComponent<BezierPoint>()];
				
				foreach (BezierPoint point in points)
				{
					neighbors.Add(point.GetComponent<IconCity>().City);
				}
			}
			city.InitNeighbors(neighbors);
		}

		void Awake()
		{
			nameLabel.text = this.name;

			PieceManager.AddPiece += PieceManager_AddPiece;
			PieceManager.DeletePiece += PieceManager_DeletePiece;
			
			policeSprite.SetActive(false);
			questSprite.SetActive(false);
			brokerSprite.SetActive(false);

			LanguageSelection.selectLanguageForUI += ChangeLanguage;

			EventDelegate.Parameter param = new EventDelegate.Parameter(this, string.Empty);
			NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), this.GetComponent<UIButton>(), "ClickCity", new EventDelegate.Parameter[] { param });
		}
		
		public void ChangeLanguage(eLanguage language)
		{
			Language.CityData[] list = null;
			if (language == eLanguage.KOREAN)
			{
				list = CityManager.Instance.KorList;				
			}
			else if (language == eLanguage.ENGLISH)
			{
				list = CityManager.Instance.EngList;
			}
			nameLabel.text = Array.Find<Language.CityData>(list, x => x.index == city.Name).name;
		}
		

		private void PieceManager_DeletePiece(Piece piece)
		{
			if (piece.City == null)
			{
				return;
			}
			if (piece.City.Name.ToString() != this.name)
			{
				return;
			}
			SetPieceSprite(piece.SubjectType, false);
		}

		private void PieceManager_AddPiece(Piece piece)
		{
			if(piece.City == null)
			{
				return;
			}
			if (piece.City.Name.ToString() != this.name)
			{
				return;
			}
			SetPieceSprite(piece.SubjectType, true);
		}

		private void SetPieceSprite(eSubjectType type, bool isExist)
		{
			if (type == eSubjectType.POLICE)
			{
				policeSprite.SetActive(isExist);
				policeNumLabel.text = PieceManager.Instance.GetNumberOfPiece(eSubjectType.POLICE, this.City).ToString();
			}
			else if (type == eSubjectType.QUEST)
			{
				questSprite.SetActive(isExist);
			}
			else if (type == eSubjectType.BROKER)
			{
				brokerSprite.SetActive(isExist);
			}
			NGUIDebug.Log(this.name + "'s " + type.ToString() + " sprite is " + isExist);
		}

		void Start()
		{
			if (type == eNodeType.BIGCITY)
			{
				GetComponent<UISprite>().color = Color.white;
				nameLabel.text = gameObject.name;
			}
			else if(type == eNodeType.MIDDLECITY)
			{
				GetComponent<UISprite>().color = new Color(1, 1, 0.8f);
				nameLabel.text = gameObject.name;
			}
			else if (type == eNodeType.SMALLCITY)
			{
				GetComponent<UISprite>().color = new Color(1, 1, 0.5f);
				nameLabel.text = type.ToString();
				gameObject.name = type.ToString();
			}
			else if (type == eNodeType.TOWN)
			{
				GetComponent<UISprite>().color = new Color(1, 0.8f, 1f);
				nameLabel.text = type.ToString();
				gameObject.name = type.ToString();
			}
			else if (type == eNodeType.MOUNTAIN)
			{
				GetComponent<UISprite>().color = new Color(0.5f, 1, 0.5f);
				nameLabel.text = type.ToString();
				gameObject.name = type.ToString();
			}
			else
			{
			}
		}

		public City City
		{
			get
			{
				return city;
			}
		}
	}
}