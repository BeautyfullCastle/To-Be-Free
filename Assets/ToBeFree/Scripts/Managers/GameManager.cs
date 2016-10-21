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
			foreach(UICommand command in commands)
			{
				command.GetComponent<UIButton>().isEnabled = false;
			}
			shopUIObj.SetActive(false);

			TimeTable.Instance.NotifyEveryWeek += WeekIsGone;
			TimeTable.Instance.NotifyEveryday += DayIsGone;
			
			uiSetting.SetActive(false);

			curves = GameObject.FindObjectOfType<BezierCurveList>();
		}

		private List<IEnumerator> coroutines;
		public void AddCoroutine(IEnumerator routine)
		{
			coroutines.Add(routine);
		}

		public bool revealPoliceNum = false;
		public IconCity ClickedIconCity = null;
		public void ClickCity(IconCity city)
		{
			// 캐릭터 도시 이동
			if(readyToMove)
			{
				character.NextCity = city.City;

				readyToMove = false;
				isActStart = true;
			}
			else
			{
				// 공안 조사 : 해당 도시의 공안 수 보여주는 이펙트
				if (revealPoliceNum == false)
				{
					revealPoliceNum = true;
				}
			}
			
			// 도시 아이콘 정상화
			foreach (IconCity c in iconCities)
			{
				c.GetComponent<TweenAlpha>().value = 1;
				c.GetComponent<TweenAlpha>().enabled = false;
				c.GetComponent<UIButton>().isEnabled = true;
			}

			ClickedIconCity = city;
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
					UICommandPopup busPopup = InstantiatePopup("Bus", eEventAction.MOVE_BUS);

					bool buttonEnabled = (character.CurCity.Type == eNodeType.BIGCITY) && (character.Stat.Money >= 4);
					foreach (UIButton button in busPopup.requiredTimeButtons)
					{
						button.isEnabled = buttonEnabled;
					}

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
				case eCommand.INVESTIGATION:
					action = new Investigation();

					UICommandPopup[] investigationPopups = new UICommandPopup[3];
					UICommandPopup gatheringPopup = null;

					investigationPopups[0] = InstantiatePopup("City Investigation", eEventAction.INVESTIGATION_CITY);
					investigationPopups[1] = InstantiatePopup("Broker Investigation", eEventAction.INVESTIGATION_BROKER);
					investigationPopups[2] = InstantiatePopup("Police Investigation", eEventAction.INVESTIGATION_POLICE);
					gatheringPopup = InstantiatePopup("Gathering Investigation", eEventAction.GATHERING);

					if(character.CurCity.Type == eNodeType.MOUNTAIN)
					{
						foreach(UICommandPopup popup in investigationPopups)
						{
							foreach(UIButton button in popup.requiredTimeButtons)
							{
								button.isEnabled = false;
							}
						}
					}
					else
					{
						foreach (UIButton button in gatheringPopup.requiredTimeButtons)
						{
							button.isEnabled = false;
						}
					}
					break;
				case eCommand.SHOP:
					action = new EnterToShop();
					character.CanAction[(int)commandType] = false;
					isActStart = true;
					break;
				case eCommand.BROKER:
					action = new BrokerAction();
					character.CanAction[(int)commandType] = false;
					isActStart = true;
					break;
				case eCommand.QUEST:
					action = new QuestAction();
					character.CanAction[(int)commandType] = false;
					isActStart = true;
					break;
				case eCommand.ABILITY:
					action = new AbilityAction();
					character.CanAction[(int)commandType] = false;
					isActStart = true;
					break;
			}

			curCommandType = commandType;
		}

		private UICommandPopup InstantiatePopup(string name, eEventAction actionType)
		{
			GameObject prefab = Resources.Load("Command Popup") as GameObject;
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.transform.SetParent(commandPopupGrid.transform);
			obj.transform.localScale = Vector3.one;
			UICommandPopup popup = obj.GetComponent<UICommandPopup>();
			popup.nameLabel.text = name;
			popup.actionType = actionType;

			if (actionType == (actionType & (eEventAction.MOVE | eEventAction.MOVE_BUS )))
			{
				popup.requiredTimeButtons[1].gameObject.SetActive(false);
				popup.requiredTimeButtons[2].gameObject.SetActive(false);
			}
			else
			{
				popup.requiredTimeButtons[1].gameObject.SetActive(character.RemainAP >= 2);
				popup.requiredTimeButtons[2].gameObject.SetActive(character.RemainAP >= 3);
			}

			commandPopupGrid.Reposition();

			return popup;
		}

		public void ClickCommandRequiredTime(eEventAction actionType, string requiredTime)
		{			
			int iRequiredTime = int.Parse(requiredTime);

			action.RequiredTime = iRequiredTime;
			action.ActionName = actionType;

			character.CanAction[(int)curCommandType] = false;
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
					c.SetEnable(false);
				}

				// set the cities enabled and twinkle only you can go.
				List<City> cities = CityManager.Instance.FindCitiesByDistance(Character.CurCity, character.RemainAP, actionType);
				foreach (City city in cities)
				{
					IconCity iconCity = Array.Find<IconCity>(iconCities, x => x.City == city);
					iconCity.SetEnable(true);
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
				if(Time.timeScale <= 5f)
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
			//yield return character.MoveTo(character.CurCity);

			//CityManager.Instance.FindNearestPathToStartCity(CityManager.Instance.Find(eCity.KUNMING), CityManager.Instance.Find(eCity.DANDONG));
			

			inspectAction = new Inspect();

			yield return (ShowStateLabel("Adding Polices to Big cities.", 0.5f));
			
			//add polices in big cities.
			List<City> bigCityList = CityManager.Instance.FindCitiesByType(eNodeType.BIGCITY);
			foreach (City city in bigCityList)
			{
				PieceManager.Instance.Add(new Police(city, eSubjectType.POLICE));
				NGUIDebug.Log("Add Big city : " + city.Name.ToString());
			}

			yield return null;

#if UNITY_EDITOR
			// for test
			//character.Stat.Agility = 0;
			//character.Stat.InfoNum = 2;
			//character.Stat.Satiety = 1;
#endif

			this.State = GameState.StartDay;

			yield return null;

			// Exit
			yield return NextState();
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
			yield return NextState();
		}

		IEnumerator StartDayState()
		{
			// Enter
			yield return (ShowStateLabel("Start Day State", 0.5f));
			
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.DAY, character);

#if UNITY_EDITOR
			// for test
			//PieceManager.Instance.Add(new Broker(character.CurCity, eSubjectType.BROKER));
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[26], character);
			//yield return QuestManager.Instance.Load(QuestManager.Instance.List[1], character);
			//Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;
			//yield return police.Move();
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

			// 집중 단속 시 모든 공안 이동력만큼 움직이기
			yield return CrackDown.Instance.MoveEveryPolice();

			// 스페셜 이벤트 확률 증가
			character.AddSpecialEventProbability();


			// 하루 시작 이벤트
			if (character.IsDetention == false)
			{
				if (character.CheckSpecialEvent())
				{
					yield return EventManager.Instance.ActivateEvent(EventManager.Instance.Find(eEventAction.START), character);
				}
			}

			if (character.IsDetention)
			{
				this.State = GameState.Detention;
			}
			else
			{
				this.State = GameState.Act;
			}

			// Exit
			yield return NextState();
		}

		IEnumerator ActState()
		{
			// Enter
			yield return (ShowStateLabel("Act State", 0.5f));
			foreach (UICommand command in commands)
			{
				command.SetActiveCommands();
			}
			commandUIObj.GetComponent<UIGrid>().Reposition();
			
			while(isActStart==false)
			{
				yield return null;
			}
			foreach (UICommand command in commands)
			{
				command.GetComponent<UIButton>().isEnabled = false;
			}
			isActStart = false;
			
			// 공안 이벤트
			// if selected event is not move,
			// check polices in current city and activate police events.
			if (action is Move == false && action.ActionName != eEventAction.HIDE)
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
			yield return NextState();
		}

		IEnumerator DetentionState()
		{
			// Enter
			yield return (ShowStateLabel("Detention State", 0.5f));

			action = new DetentionAction();
			yield return action.Activate(character);
			
			this.State = GameState.Night;

			// Exit
			yield return NextState();
		}

		IEnumerator NightState()
		{
			// 하루 끝 이벤트
			if (character.CheckSpecialEvent() && character.IsDetention == false)
			{
				yield return TimeTable.Instance.SpendTime(1, eSpendTime.RAND);
				yield return EventManager.Instance.ActivateEvent(EventManager.Instance.Find(eEventAction.END), character);
				yield return TimeTable.Instance.SpendRemainTime();
			}
			else
			{
				yield return TimeTable.Instance.SpendTime(1, eSpendTime.END);
			}

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

			//yield return AbnormalConditionManager.Instance.List[1].Activate(character);
			//yield return AbnormalConditionManager.Instance.List[2].Activate(character);

			//character.Stat.HP = 1;

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

			// after daytime // Temporary
			yield return BuffManager.Instance.CheckDuration(character);

			while (state == GameState.Night)
			{
				yield return null;
			}

			// Exit
			yield return NextState();
		}

		#endregion

		protected IEnumerator NextState()
		{
			string methodName = State.ToString() + "State";
			System.Reflection.MethodInfo info = GetType().GetMethod(methodName,
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			yield return ((IEnumerator)info.Invoke(this, null));
		}

		private IEnumerator Instance_NotifyEveryNight()
		{
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.NIGHT, character);

			// 구금 상태일 때 밤단계 탈출 시도를 위한.
			if(character.IsDetention == true && action is DetentionAction)
			{
				yield return action.Activate(character);
			}

			character.Reset();


			if (character.IsFull)
			{
				yield return GameManager.Instance.ShowStateLabel("Skip Food", 0.5f);
			}
			else
			{
				if (character.Stat.Satiety <= 0)
				{
					character.Stat.HP -= 2;
				}
				else
				{
					character.Stat.Satiety--;
				}
			}

			yield return AbnormalConditionManager.Instance.ActiveByCondition();

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

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.NIGHT, character);
		}

		private IEnumerator Instance_NotifyEveryWeek()
		{
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.WEEK, character);

			yield return CrackDown.Instance.Check();

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.WEEK, character);
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
			private set
			{
				state = value;
			}
		}
	}
}
 