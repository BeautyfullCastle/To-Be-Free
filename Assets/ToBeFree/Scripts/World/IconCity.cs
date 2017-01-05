using UnityEngine;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
	[SerializeField]
	public enum eNodeType
	{
		NULL, BIGCITY, SMALLCITY, TOWN, MOUNTAIN
	}

	//[ExecuteInEditMode]
	public class IconCity : MonoBehaviour
	{
		public UILabel nameLabel;

		public eNodeType type = eNodeType.TOWN;

		[SerializeField]
		private UIGrid policeGrid;
		[SerializeField]
		public Transform characterOffset;
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

			policeGrid = this.transform.FindChild("Police Grid").GetComponent<UIGrid>();
			characterOffset = this.transform.FindChild("Character Offset");
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

			if (this.gameObject.name == EnumConvert<eNodeType>.ToString(eNodeType.SMALLCITY) 
				|| this.gameObject.name == EnumConvert<eNodeType>.ToString(eNodeType.TOWN)
				|| this.gameObject.name == EnumConvert<eNodeType>.ToString(eNodeType.MOUNTAIN) )
			{
				sprite.width = smallSize;
				sprite.height = smallSize;
				type = EnumConvert<eNodeType>.ToEnum(this.gameObject.name);
				nameLabel.enabled = false;

				questOffset.localPosition = new Vector3(-14.8f, -48.6f);
				brokerOffset.localPosition = new Vector3(18.3f, -48.4f);
				timerSprite.transform.localPosition = new Vector3(0, 49.6f);
				
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
			else// if (this.type == eNodeType.BIGCITY)
			{
				this.type = eNodeType.BIGCITY;
				sprite.width = bigSize;
				sprite.height = bigSize;
				sprite.spriteName = "bigcity";
				questOffset.localPosition = new Vector3(-41.9f, -77.8f);
				brokerOffset.localPosition = new Vector3(41.9f, -78.2f);
				timerSprite.transform.localPosition = new Vector3(0, 76.7f);
			}
			//else if (this.type == eNodeType.MIDDLECITY)
			//{
			//	sprite.width = middleSize;
			//	sprite.height = middleSize;
			//	sprite.spriteName = "middle";
			//	nameLabel.fontSize = 11;
			//	nameLabel.transform.localPosition = new Vector3(0f, -32f);
			//	questOffset.localPosition = new Vector3(-23.6f, -60.2f);
			//	brokerOffset.localPosition = new Vector3(24.8f, -60.5f);
			//	timerSprite.transform.localPosition = new Vector3(0, 59.7f);
			//}
		}

		public void PutPiece(IconPiece iconPiece)
		{
			if(iconPiece.subjectType == eSubjectType.POLICE)
			{
				policeGrid.AddChild(iconPiece.transform);
				policeGrid.enabled = true;
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
			UIButton uiButton = GetComponent<UIButton>();
			if (uiButton == null)
				return;

			TweenAlpha tweenAlpha = GetComponent<TweenAlpha>();
			if (tweenAlpha == null)
				return;
			
			uiButton.enabled = isEnable;
			tweenAlpha.enabled = isEnable;

			if (isEnable)
			{
				//uiButton.ResetDefaultColor();
				tweenAlpha.ResetToBeginning();
			}
			else
			{
				Color color = uiButton.defaultColor;
				color.a = tweenAlpha.to;
				uiButton.defaultColor = color;
			}

			if (timerSprite == null)
				return;

			timerSprite.enabled = isEnable;
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