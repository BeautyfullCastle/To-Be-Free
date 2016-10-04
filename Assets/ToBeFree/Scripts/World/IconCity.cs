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

		public eNodeType type = eNodeType.TOWN;

		[SerializeField]
		private UIGrid grid;
		[SerializeField]
		private Transform questOffset;
		[SerializeField]
		private Transform brokerOffset;

		private City city;
		
		public void Init()
		{
			city = new City(gameObject.name, type, this);						
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

			grid = this.gameObject.GetComponentInChildren<UIGrid>();
			questOffset = this.transform.FindChild("Quest Offset");
			brokerOffset = this.transform.FindChild("Broker Offset");

			LanguageSelection.selectLanguageForUI += ChangeLanguage;

			EventDelegate.Parameter param = new EventDelegate.Parameter(this, string.Empty);
			NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), this.GetComponent<UIButton>(), "ClickCity", new EventDelegate.Parameter[] { param });
		}

		void Start()
		{
			if (this.gameObject.name == eNodeType.SMALLCITY.ToString())
			{
				GetComponent<UISprite>().color = new Color(1, 1, 0.5f);
				type = EnumConvert<eNodeType>.ToEnum(this.gameObject.name);
				nameLabel.text = type.ToString();
			}
			else if (this.gameObject.name == eNodeType.TOWN.ToString())
			{
				GetComponent<UISprite>().color = new Color(1, 0.8f, 1f);
				type = EnumConvert<eNodeType>.ToEnum(this.gameObject.name);
				nameLabel.text = type.ToString();
			}
			else if (this.gameObject.name == eNodeType.MOUNTAIN.ToString())
			{
				GetComponent<UISprite>().color = new Color(0.5f, 1, 0.5f);
				type = EnumConvert<eNodeType>.ToEnum(this.gameObject.name);
				nameLabel.text = type.ToString();
			}
			else
			{
				GetComponent<UISprite>().color = Color.white;
				type = eNodeType.BIGCITY;
				nameLabel.text = gameObject.name;
			}
		}

		public void PutPiece(IconPiece iconPiece)
		{
			if(iconPiece.subjectType == eSubjectType.POLICE)
			{
				grid.AddChild(iconPiece.transform);
				grid.enabled = true;
			}
			else if(iconPiece.subjectType == eSubjectType.QUEST)
			{
				iconPiece.transform.SetParent(questOffset);
				iconPiece.transform.localPosition = Vector3.zero;
			}
			else if (iconPiece.subjectType == eSubjectType.BROKER)
			{
				iconPiece.transform.SetParent(brokerOffset);
				iconPiece.transform.localPosition = Vector3.zero;
			}
			iconPiece.transform.localScale = Vector3.one;
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
		
		public City City
		{
			get
			{
				return city;
			}
		}
	}
}