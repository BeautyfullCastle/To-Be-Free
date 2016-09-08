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
		public GameObject informSprite;
		public GameObject questSprite;
		public GameObject brokerSprite;
		public GameObject policeSprite;
		public UILabel policeNumLabel;

		public eNodeType type = eNodeType.TOWN;

		private City city;

		public IconCity This { get { return this; } }

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
			if (FindObjectOfType<BezierCurveList>().Dic.ContainsKey(GetComponent<BezierPoint>()) == false)
			{
				Debug.LogError("Not contains this point " + this.name + this.transform.parent.name);
				return;
			}
			List<BezierPoint> points = FindObjectOfType<BezierCurveList>().Dic[GetComponent<BezierPoint>()];
			List<City> neighbors = new List<City>();
			foreach (BezierPoint point in points)
			{
				neighbors.Add(point.GetComponent<IconCity>().City);
			}
			city.InitNeighbors(neighbors);
		}

		void Awake()
		{
			nameLabel.text = this.name;

			PieceManager.AddPiece += PieceManager_AddPiece;
			PieceManager.DeletePiece += PieceManager_DeletePiece;

			informSprite.SetActive(false);
			policeSprite.SetActive(false);
			questSprite.SetActive(false);
			brokerSprite.SetActive(false);

			LanguageSelection.selectLanguageForUI += ChangeLanguage;

		}

		//void Start()
		//{
		//	UIButton m_BtnTest = GetComponent<UIButton>();

		//	EventDelegate.Parameter param = new EventDelegate.Parameter();

		//	// 파라메타로 지정할 오브젝트가 있는 컴포넌트
		//	param.obj = gameObject.GetComponent<Transform>();

		//	// 해당 오브젝트의 변수명
		//	param.field = "name";

		//	// 이벤트 등록 및 파라메타 추가
		//	EventDelegate onClick = new EventDelegate(GameObject.FindObjectOfType<GameManager>(), "ClickCity");

		//	onClick.parameters[0] = param;

		//	EventDelegate.Add(m_BtnTest.onClick, onClick);
		//}

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
			nameLabel.text = Array.Find<Language.CityData>(CityManager.Instance.KorList, x => x.index == city.Name).name;
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
			else if (type == eSubjectType.INFO)
			{
				informSprite.SetActive(isExist);
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