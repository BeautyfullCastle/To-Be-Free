using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class GameManager : MonoSingleton<GameManager>
	{
		public enum GameState
		{
			Init, StartWeek, Detention, Mongolia, Escape,
			StartDay,
			Act,
			Night,
			NORTHKOREA
		}
		
		public UILabel stateLabel;
		public Camera directingCam;
		public Camera worldCam;
		public GameObject commandUIObj;
		public GameObject shopUIObj;
		public UIEventManager uiEventManager;
		public GameObject uiSetting;
		public IconCity[] iconCities;
		public UIGrid commandPopupGrid;
		public GameObject IconPieceObj;

		private Character character;
		private Action action;
		private Action inspectAction;

		private GameState state;
		private bool activateAbnormal;
		private UICommand[] commands;
		private bool moveTest;
		private bool readyToMove;

		private bool isActStart = false;

		private BezierCurveList curves;

		// can't use the constructor
		private GameManager()
		{
		}

		private void Init()
		{
			CityManager.Instance.Init();
			iconCities = GameObject.FindObjectsOfType<IconCity>();
			foreach(IconCity iconCity in iconCities)
			{
				iconCity.Init();
			}

			CityManager.Instance.InitList();
			foreach (IconCity iconCity in iconCities)
			{
				iconCity.InitNeighbors();
			}
			
			EventManager.Instance.Init();
			ResultManager.Instance.Init();
			QuestManager.Instance.Init();

			this.State = GameState.Init;
			commands = commandUIObj.GetComponentsInChildren<UICommand>();
			commandUIObj.SetActive(false);
			shopUIObj.SetActive(false);

			TimeTable.Instance.NotifyEveryWeek += WeekIsGone;
			TimeTable.Instance.NotifyEveryday += DayIsGone;
			
			uiSetting.SetActive(false);

			curves = GameObject.FindObjectOfType<BezierCurveList>();
		}
		
		public void ClickCity(IconCity city)
		{
			if (readyToMove == false)
				return;

			character.NextCity = city.City;

			readyToMove = false;
			isActStart = true;

			foreach (IconCity c in iconCities)
			{
				c.GetComponent<TweenAlpha>().value = 1;
				c.GetComponent<TweenAlpha>().enabled = false;
				c.GetComponent<UIButton>().isEnabled = true;
			}
		}

		private eCommand curCommandType = eCommand.NULL;

		public void ClickCommand(string command)
		{ 
			eCommand commandType = EnumConvert<eCommand>.ToEnum(command);

			commandPopupGrid.transform.DestroyChildren();

			if (curCommandType == commandType)
			{
				curCommandType = eCommand.NULL;
				return;
			}
			
			switch (commandType)
			{
				case eCommand.MOVE:
					action = new Move();

					InstantiatePopup("Normal Move", eEventAction.MOVE);
					InstantiatePopup("Bus", eEventAction.MOVE_BUS);
					
					break;

				case eCommand.WORK:
					action = new Work();

					InstantiatePopup("Normal Work", eEventAction.WORK);

					break;
				case eCommand.REST:
					action = new Rest();

					InstantiatePopup("Normal Rest", eEventAction.REST);
					InstantiatePopup("Hide Rest", eEventAction.HIDE);

					break;
				case eCommand.SHOP:
					action = new EnterToShop();
					break;
				case eCommand.INVESTIGATION:
					action = new InfoAction();

					InstantiatePopup("City Investigation", eEventAction.INVESTIGATION_CITY);
					InstantiatePopup("Broker Investigation", eEventAction.INVESTIGATION_BROKER);
					InstantiatePopup("Police Investigation", eEventAction.INVESTIGATION_POLICE);
					InstantiatePopup("Gathering Investigation", eEventAction.GATHERING);

					break;
					// broker and quest command
				case eCommand.BROKER:
					InstantiatePopup("Broker", eEventAction.BROKER);
					InstantiatePopup("Quest", eEventAction.QUEST);

					break;
			}

			character.CanAction[(int)commandType] = false;
			curCommandType = commandType;
		}

		private void InstantiatePopup(string name, eEventAction actionType)
		{
			GameObject prefab = Resources.Load("Command Popup") as GameObject;
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.transform.SetParent(commandPopupGrid.transform);
			obj.transform.localScale = Vector3.one;
			UICommandPopup popup = obj.GetComponent<UICommandPopup>();
			popup.nameLabel.text = name;
			popup.actionType = actionType;

			if (actionType == (actionType & (eEventAction.MOVE | eEventAction.MOVE_BUS | eEventAction.BROKER | eEventAction.QUEST)))
			{
				popup.requiredTimeButtons[1].gameObject.SetActive(false);
				popup.requiredTimeButtons[2].gameObject.SetActive(false);
			}

			commandPopupGrid.Reposition();
		}

		public void ClickCommandRequiredTime(eEventAction actionType, string requiredTime)
		{
			if (actionType == eEventAction.BROKER)
			{
				action = new BrokerAction();
			}
			else if (actionType == eEventAction.QUEST)
			{
				action = new QuestAction();
			}
			
			int iRequiredTime = int.Parse(requiredTime) - 1;

			action.RequiredTime = iRequiredTime;
			action.ActionName = actionType;
			
			curCommandType = eCommand.NULL;
			commandPopupGrid.transform.DestroyChildren();

			
			if (actionType != (actionType & (eEventAction.MOVE | eEventAction.MOVE_BUS)))
			{
				isActStart = true;
			}
			else
			{
				readyToMove = true;

				// set the button disabled every city.
				foreach (IconCity c in iconCities)
				{
					c.GetComponent<TweenAlpha>().value = 1;
					c.GetComponent<TweenAlpha>().enabled = false;
					c.GetComponent<UIButton>().isEnabled = false;
				}

				// set the cities abled and twinkle only you can go.
				List<City> cities = CityManager.Instance.FindCitiesByDistance(Character.CurCity, character.RemainAP);
				foreach (City city in cities)
				{
					IconCity iconCity = Array.Find<IconCity>(iconCities, x => x.City == city);
					iconCity.GetComponent<TweenAlpha>().enabled = true;
					iconCity.GetComponent<UIButton>().isEnabled = true;
					Debug.Log("City you can go : " + city.Name);
				}
			}
		}
		
		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				if(Time.timeScale >= 0f)
					Time.timeScale -= .2f;

				NGUIDebug.Log("Time Scale : " + Time.timeScale);
			}

			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				if(Time.timeScale <= 3f)
					Time.timeScale += .2f;

				NGUIDebug.Log("Time Scale : " + Time.timeScale);
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				uiEventManager.OnClickOK();
			}

			if(Input.GetKeyDown(KeyCode.Escape))
			{
				uiSetting.SetActive(!uiSetting.activeSelf);
				int mask = 32; // UI Layer Num : 2 ^ 5
				if (uiSetting.activeSelf)
					mask = 256; // Setting Layer Num : 2 ^ 8
				GameObject.Find("UI Camera").GetComponent<UICamera>().eventReceiverMask = mask;
			}

#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.D))
			{
				character.IsDetention = true;
			}

			if(Input.GetKeyDown(KeyCode.A))
			{
				// for the test
				//Effect effect = new Effect(eSubjectType.COMMAND, eVerbType.DEACTIVE, eObjectType.SHOP);
				//yield return effect.Activate(character, 1);
				activateAbnormal = true;
			}

			if(Input.GetKeyDown(KeyCode.P))
			{
				PieceManager.Instance.Add(new Police(CityManager.Instance.FindRand(eSubjectType.POLICE), eSubjectType.POLICE));
			}
			if (Input.GetKeyDown(KeyCode.I))
			{
				PieceManager.Instance.Add(new Information(CityManager.Instance.FindRandCityByDistance(character.CurCity, 2, eSubjectType.INFO), eSubjectType.INFO));
			}

			if(Input.GetKeyDown(KeyCode.M))
			{
				moveTest = true;                
			}
			
#endif
			// 0. parse data

			// 1. start week
			//  1.1 treat finished quest
			//  1.2 put pieces
			//      1.2.1 get quest(if have city, get piece, or just get quest)
			//      1.2.2 put other pieces
			//  1.3 get global event

			// 2. start day

			// 3. start action
			//  3.1 get command
			//  3.2 start event
			//      3.2.1 start test
			//      3.2.2 end test
			//      3.2.3 get result
			//  3.3 end event

			// 4. end action

			// 5. end day

			// 6. end week

		}

		public GameObject FindGameObject(string name)
		{
			return GameObject.Find(name);
		}

		private void DayIsGone()
		{
			state = GameState.StartDay;
		}

		private void WeekIsGone()
		{
			state = GameState.StartWeek;			
		}

		private void Start()
		{
			this.StartCoroutine(InitState());
		}

		#region State Routine

		
		IEnumerator InitState()
		{
			// Enter
			yield return (ShowStateLabel("Init State", 0.5f));

			Init();

			// character init
			CharacterManager.Instance.Init();
			character = CharacterManager.Instance.List[0];
			character.Stat.RefreshUI();
			GameObject.FindObjectOfType<UIInventory>().Change(character.Inven);
			yield return character.MoveTo(character.CurCity);

			//CityManager.Instance.FindNearestPathToStartCity(CityManager.Instance.Find(eCity.KUNMING), CityManager.Instance.Find(eCity.DANDONG));
			

			inspectAction = new Inspect();

			yield return (ShowStateLabel("Adding Polices to Big cities.", 0.5f));

			// HAVE TO CHANGE
			//add polices in big cities.
			List<City> bigCityList = CityManager.Instance.FindCitiesByType(eNodeType.BIGCITY);
			List<Transform> bigCityTransformList = new List<Transform>();
			foreach (City city in bigCityList)
			{
				PieceManager.Instance.Add(new Police(city, eSubjectType.POLICE));
				bigCityTransformList.Add(GameObject.Find(city.Name.ToString()).transform);
				NGUIDebug.Log("Add Big city : " + city.Name.ToString());
			}
			yield return (MoveDirectingCam(bigCityTransformList, 0.5f));


			yield return null;

			// for test
			//character.Stat.Agility = 0;
			//character.Stat.InfoNum = 2;
			
			// Excute

			this.State = GameState.StartWeek;

			yield return null;

			// Exit
			NextState();
		}
		
		IEnumerator StartWeekState()
		{
			// Enter
			yield return (ShowStateLabel("Start Week State", 0.5f));
			

			yield return (Instance_NotifyEveryWeek());

			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[67], character);
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[67], character);

			action = null;
			yield return null;

			// Excute
			this.State = GameState.StartDay;
			yield return null;

			// Exit
			NextState();
		}

		IEnumerator StartDayState()
		{
			// Enter
			yield return (ShowStateLabel("Start Day State", 0.5f));

			yield return character.Inven.CheckItem(eStartTime.DAY, character);

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.DAY, character);

#if UNITY_EDITOR
			// for test
			//PieceManager.Instance.Add(new Broker(character.CurCity, eSubjectType.BROKER));
			//PieceManager.Instance.Add(new QuestPiece(QuestManager.Instance.List[1], character, character.CurCity, eSubjectType.QUEST));
			
			Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
			yield return police.Move();
#endif
			/*
			 * 행동 시
			특수 이벤트 나올지 검사
			(특수 이벤트만 있는 행동은 100%)
			- 안나오면 해당 행동 일반 이벤트 실행 및 특수 이벤트 확률 증가
			- 나오면 5개의 이벤트 중 하나 골라서 해당 행동의 특수 이벤트 불러온 후 특수이벤트 확률 0으로 만듦

			해당 행동 시작하면 특수 이벤트 / 일반 이벤트 / 스킵 중 하나 실행

			이벤트 발생 확률 하루마다 증가
			이벤트 확률 / 5

			5개의 이벤트
			- 하루 시작 1
			- (공안)
			- 행동 3
			- 끝 1
			*/

			character.AddSpecialEventProbability();


			// 하루 시작 이벤트
			if(character.CheckSpecialEvent())
				yield return EventManager.Instance.ActivateEvent(EventManager.Instance.Find(eEventAction.START), character);

			if (character.IsDetention)
			{
				this.State = GameState.Detention;                
			}
			else
			{
				this.State = GameState.Act;
			}

			// Exit
			NextState();
		}

		IEnumerator ActState()
		{
			// Enter
			yield return (ShowStateLabel("Act State", 0.5f));

			commandUIObj.SetActive(true);
			foreach(UICommand command in commands)
			{
				command.SetActiveCommands();
			}
			commandUIObj.GetComponent<UIGrid>().Reposition();
			
			while(isActStart==false)
			{
				yield return null;
			}
			commandUIObj.SetActive(false);
			isActStart = false;
			
			// 공안 이벤트
			// if selected event is not move,
			// check polices in current city and activate police events.
			if (action is Move == false)
			{
				yield return inspectAction.Activate(character);
			}

			// 행동 이벤트
			if (character.IsDetention == false && character.IsActionSkip == false)
			{
				// activate selected event
				yield return action.Activate(character);
			}
			else if (character.IsActionSkip == true)
			{
				character.IsActionSkip = false;
			}
			
			if(character.RemainAP <= 0)
			{
				this.State = GameState.Night;
			}
			else
			{
				this.State = GameState.Act;
			}
			
			// Exit
			NextState();
		}

		IEnumerator DetentionState()
		{
			// Enter
			yield return (ShowStateLabel("Detention State", 0.5f));

			action = new DetentionAction();
			yield return action.Activate(character);
			
			this.State = GameState.Night;

			// Exit
			NextState();
		}

		IEnumerator NightState()
		{
			// after daytime // Temporary
			yield return BuffManager.Instance.CheckDuration(character);

			// Enter
			yield return (ShowStateLabel("Night State", 0.5f));
			
			yield return Instance_NotifyEveryNight();

			#region editor test
#if UNITY_EDITOR
			// for the test
			if (activateAbnormal)
			{
				//foreach(AbnormalCondition ab in AbnormalConditionManager.Instance.List)
				//    yield return ab.Activate(character);

				yield return AbnormalConditionManager.Instance.List[5].Activate(character);
				activateAbnormal = false;
			}

			if (moveTest)
			{
				character.Stat.Agility = 0;
				yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[5], character);
				moveTest = false;
			}

			yield return AbnormalConditionManager.Instance.List[1].Activate(character);
			yield return AbnormalConditionManager.Instance.List[2].Activate(character);

			character.Stat.HP = 1;

			// select test
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[13], character);

			//character.Stat.Observation = 0;
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[89], character);

			// open mongol event
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[51], character);

			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[0], character);
#endif
			#endregion

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.DAY, character);

			TimeTable.Instance.DayIsGone();
			while(state == GameState.Night)
			{
				yield return null;
			}

			// Exit
			NextState();
		}

		#endregion

		protected void NextState()
		{
			string methodName = State.ToString() + "State";
			System.Reflection.MethodInfo info = GetType().GetMethod(methodName,
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			StartCoroutine((IEnumerator)info.Invoke(this, null));
		}

		private IEnumerator Instance_NotifyEveryNight()
		{
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.NIGHT, character);

			// 하루 끝 이벤트
			if (character.CheckSpecialEvent())
			{
				yield return EventManager.Instance.ActivateEvent(EventManager.Instance.Find(eEventAction.END), character);
			}

			character.Reset();

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.NIGHT, character);
		}

		private IEnumerator Instance_NotifyEveryWeek()
		{
			yield return character.Inven.CheckItem(eStartTime.NIGHT, character);

			yield return character.Inven.CheckItem(eStartTime.WEEK, character);

			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.WEEK, character);
			
			if (character.IsFull)
			{
				yield return GameManager.Instance.ShowStateLabel("Skip Food", 0.5f);
			}
			else
			{
				if (character.Stat.FOOD <= 0)
				{
					character.Stat.HP -= 2;
				}
				else
				{
					character.Stat.FOOD--;
				}
			}

			//check current quest's end time and apply the result
			List<Piece> questPieces = PieceManager.Instance.FindAll(eSubjectType.QUEST);
			if (questPieces != null && questPieces.Count > 0)
			{
				List<Piece> pastQuestPieces = questPieces.FindAll(x => x.CheckDuration());
				foreach (QuestPiece pastQuestPiece in pastQuestPieces)
				{
					yield return pastQuestPiece.TreatPastQuests(character);
				}
			}

			yield return CrackDown.Instance.Check();
			//yield return PutPieces();

			// activate global event
			//yield return EventManager.Instance.DoCommand(eEventAction.GLOBAL, character);

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.WEEK, character);
		}

		private IEnumerator PutPieces()
		{
			yield return ShowStateLabel("Put Pieces", 0.5f);

			// put pieces in one of random cities (police, information, quest)
			int distance = 2;
			List<Transform> pieceCityTransformList = new List<Transform>();
			// 1 random quest
			Quest selectedQuest = QuestManager.Instance.FindRand();
			if(CityManager.Instance == null)
			{
				Debug.LogError("CityManager.Instance is null");
			}
			
			yield return QuestManager.Instance.Load(selectedQuest, character);

			if (selectedQuest.ActionType == eQuestActionType.QUEST)
			{
				QuestPiece piece = PieceManager.Instance.Find(selectedQuest);
				pieceCityTransformList.Add(GameObject.Find(piece.City.Name).transform);
			}

			// 2 polices
			Police randPolice = new Police(CityManager.Instance.FindRand(eSubjectType.POLICE), eSubjectType.POLICE);
			PieceManager.Instance.Add(randPolice);
			pieceCityTransformList.Add(GameObject.Find(randPolice.City.Name).transform);
			//Police randPoliceByDistance = new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance, eSubjectType.POLICE), eSubjectType.POLICE);
			//PieceManager.Instance.Add(randPoliceByDistance);
			//pieceCityTransformList.Add(GameObject.Find(randPoliceByDistance.City.Name).transform);

			// 2 informations
			Information randInfo = new Information(CityManager.Instance.FindRand(eSubjectType.INFO), eSubjectType.INFO);
			PieceManager.Instance.Add(randInfo);
			pieceCityTransformList.Add(GameObject.Find(randInfo.City.Name).transform);
			//Information randInfoByDistance = new Information(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance, eSubjectType.INFO), eSubjectType.INFO);
			//PieceManager.Instance.Add(randInfoByDistance);
			//pieceCityTransformList.Add(GameObject.Find(randInfoByDistance.City.Name).transform);

			yield return MoveDirectingCam(pieceCityTransformList, 0.5f);

			// temporary
			//PieceManager.Instance.Add(new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, 0), eSubjectType.POLICE));
			
			//Information randInfo2 = new Information(CityManager.Instance.FindRand(), eSubjectType.INFO);
			//yield return PieceManager.Instance.Move(randInfo, CityManager.Instance.FindRand());
			//pieceCityTransformList.Add(GameObject.Find(randInfo.City.Name).transform);

			//Piece infoPiece = PieceManager.Instance.GetFirst(eSubjectType.INFO);
			//Piece infoLast = PieceManager.Instance.GetLast(eSubjectType.INFO);

		}

		public IEnumerator ShowStateLabel(string text, float duration)
		{
			stateLabel.text = text;
			TweenAlpha tweenAlpha = stateLabel.transform.GetComponent<TweenAlpha>();
			tweenAlpha.enabled = true;
			tweenAlpha.style = UITweener.Style.Once;
			tweenAlpha.duration = duration / Time.timeScale;

			tweenAlpha.PlayForward();
			while(tweenAlpha.value < 1f)
			{
				yield return null;
			}
			tweenAlpha.PlayReverse();
			while (tweenAlpha.value > 0f)
			{
				yield return null;
			}

			stateLabel.alpha = 0f;
			tweenAlpha.enabled = false;
		}

		public IEnumerator MoveDirectingCam(List<Transform> transformList, float duration)
		{
			float prevWorldCamSize = worldCam.orthographicSize;
			worldCam.orthographicSize = 10f;
			directingCam.enabled = true;
			float camZPos = directingCam.transform.position.z;
			for (int i = 0; i < transformList.Count; ++i)
			{
				directingCam.transform.parent = transformList[i];
				directingCam.transform.localPosition = new Vector3(0, 0, -200f);
				yield return new WaitForSeconds(duration / Time.timeScale);
			}
			directingCam.enabled = false;
			worldCam.orthographicSize = prevWorldCamSize;

			directingCam.transform.parent = null;
		}

		public IEnumerator MoveDirectingCam(Vector3 from, Vector3 to, float duration)
		{
			float prevWorldCamSize = worldCam.orthographicSize;
			worldCam.orthographicSize = 10f;
			
			float camZPos = directingCam.transform.position.z;

			//directingCam.transform.parent = from;
			//directingCam.transform.localPosition = new Vector3(0, 0, -200f);

			Vector3 camFrom = new Vector3(from.x, from.y, -10f);
			Vector3 camTo = new Vector3(to.x, to.y, -10f);

			directingCam.transform.position = camFrom;
			directingCam.enabled = true;

			duration = duration / Time.timeScale;
			TweenPosition.Begin(directingCam.gameObject, duration, camTo);
				
			yield return new WaitForSeconds(duration);

			//directingCam.transform.parent = to;

			directingCam.enabled = false;
			worldCam.orthographicSize = prevWorldCamSize;
		}

		public void OpenEventUI()
		{
			uiEventManager.OpenUI();
		}

		public Character Character
		{
			get
			{
				return character;
			}
		}

		public GameState State
		{
			get
			{
				return state;
			}

			set
			{
				state = value;
			}
		}
	}
}
 