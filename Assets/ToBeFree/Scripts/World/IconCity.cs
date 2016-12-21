﻿using UnityEngine;
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
		[SerializeField]
		private UISprite timerSprite;

		private City city;

		public void Init()
		{
			city = new City(gameObject.name, type, this);
		}

		public void InitNeighbors()
		{
			if (GetComponent<BezierPoint>() == null)
			{
				Debug.LogError("No BezierPoint in " + this.name);
				return;
			}

			List<City>[] neighbors = new List<City>[(int)eWay.ENTIREWAY + 1];
			foreach (BezierCurveList curveList in CityManager.Instance.Curves)
			{
				int way = (int)EnumConvert<eWay>.ToEnum(curveList.name);
				neighbors[way] = new List<City>();

				if (curveList.Dic.ContainsKey(GetComponent<BezierPoint>()) == false)
				{
					//Debug.LogError("Not contains this point " + this.name + this.transform.parent.name);
					continue;
				}
				List<BezierPoint> points = curveList.Dic[GetComponent<BezierPoint>()];

				
				foreach (BezierPoint point in points)
				{
					neighbors[way].Add(point.GetComponent<IconCity>().City);
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
			timerSprite = this.transform.FindChild("Timer Sprite").GetComponent<UISprite>();

			LanguageSelection.selectLanguage += ChangeLanguage;

			EventDelegate.Parameter param = new EventDelegate.Parameter(this, string.Empty);
			NGUIEventRegister.Instance.AddOnClickEvent(FindObjectOfType<GameManager>(), this.GetComponent<UIButton>(), "ClickCity", new EventDelegate.Parameter[] { param });
		}

		void Start()
		{
			int smallSize = 60;
			int middleSize = 85;
			int bigSize = 120;

			UISprite sprite = GetComponent<UISprite>();
			sprite.color = Color.white;

			nameLabel.text = gameObject.name;
			nameLabel.color = Color.white;
			nameLabel.depth = 4;

			if (this.gameObject.name == eNodeType.SMALLCITY.ToString() || this.gameObject.name == eNodeType.TOWN.ToString()
				|| this.gameObject.name == eNodeType.MOUNTAIN.ToString() )
			{
				sprite.width = smallSize;
				sprite.height = smallSize;
				type = EnumConvert<eNodeType>.ToEnum(this.gameObject.name);
				nameLabel.enabled = false;

				questOffset.localPosition = new Vector3(-25f, -40f, 0f);
				brokerOffset.localPosition = new Vector3(25f, -40f, 0f);
				timerSprite.transform.localPosition = new Vector3(0, smallSize / 2, 0);

				if (this.gameObject.name == eNodeType.SMALLCITY.ToString())
				{
					sprite.spriteName = "small";
				}
				else if (this.gameObject.name == eNodeType.TOWN.ToString())
				{
					sprite.spriteName = "walk";
				}
				else if (this.gameObject.name == eNodeType.MOUNTAIN.ToString())
				{
					sprite.spriteName = "mountain";
				}
			}
			else if (this.type == eNodeType.BIGCITY)
			{
				sprite.width = bigSize;
				sprite.height = bigSize;
				sprite.spriteName = "bigcity";
				timerSprite.transform.localPosition = new Vector3(0, bigSize / 2, 0);
			}
			else if (this.type == eNodeType.MIDDLECITY)
			{
				sprite.width = middleSize;
				sprite.height = middleSize;
				sprite.spriteName = "middle";
				nameLabel.fontSize = 11;
				nameLabel.transform.localPosition = new Vector3(0f, -32f);
				timerSprite.transform.localPosition = new Vector3(0, middleSize / 2, 0);
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

		public void SetEnable(bool isEnable, int time = -1)
		{
			// set the city anabled and twinkle.
			if (isEnable)
			{
				GetComponent<TweenAlpha>().ResetToBeginning();
				GetComponent<TweenAlpha>().enabled = true;

				if (time == -1)
				{
					timerSprite.enabled = false;
				}
				else
				{
					timerSprite.spriteName = "Timer " + time.ToString();
					timerSprite.enabled = true;
				}
			}
			// set the button disabled.
			else
			{
				GetComponent<TweenAlpha>().enabled = false;
				timerSprite.enabled = false;
			}
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

			if (list == null || nameLabel == null)
			{
				return;
			}
			nameLabel.text = Array.Find(list, x => x.index == city.Name).name;
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