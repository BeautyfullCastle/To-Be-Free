using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
	public class GameManager : MonoSingleton<GameManager>
	{
		public enum eSceneState
		{
			Logo=0, Main, CharacterSelect, InGame, Ending
		}

		public enum GameState
		{
			Main = 0, CharacterSelect, InGame, Init, StartWeek, Detention, Mongolia, Escape,
			StartDay,
			Act,
			Night,
			NORTHKOREA
		}

		[SerializeField]
		private int increasingSpecialEventProbability;
		[SerializeField]
		private int policeTurnDays;

		[SerializeField]
		private GameState state;

		public float moveTimeSpeed;

		public UILabel stateLabel;
		public Camera worldCam;
		public UICamera uiCamera;
		public GameObject worldObj;
		public GameObject commandUIObj;
		public GameObject shopUIObj;
		public UIInventory uiInventory;
		public UIEventManager uiEventManager;
		public UIBuffManager uiBuffManager;
		public UIQuestManager uiQuestManager;
		public UITipManager uiTipManager;
		public UICharacterSelect uiCharacterSelect;
		public UIEndingManager endingManager;
		public UICharacter uiCharacter;
		public GameObject optionObj;
		public GameObject tipObj;
		public UICaution uiCaution;
		public LanguageSelection languageSelection;
		public UIGrid commandPopupGrid;
		public GameObject IconPieceObj;
		public GameObject diceObj;
		public GameObject menuObj;
		public BlackFader blackFader;

		[SerializeField]
		private TweenAlpha tweenMainNew;
		[SerializeField]
		private TweenAlpha tweenMainContinue;

		public GameObject[] scenes;

		[HideInInspector]
		public IconCity[] iconCities;
		[HideInInspector]
		public bool revealPoliceNum = false;
		[HideInInspector]
		public IconCity ClickedIconCity = null;

		private TweenAlpha lightSpriteTweenAlpha;
		private Character character;
		private Action action;
		private Action inspectAction;
		
		private BezierCurveList curves;
		private UICommand[] commands;
		
		private bool readyToMove;
		private bool isActStart = false;
		
		private eCommand curCommandType = eCommand.NULL;

		// for test
		private bool activateAbnormal;
		private bool moveTest;
		
		private bool isNew = false;
		private bool bClickedStart = false;
		private bool bWantToSeeTutorial = true;

		// don't use.
		private Camera directingCam;
		

		// can't use the constructor
		private GameManager()
		{
		}

		private void Init()
		{
			ItemManager.Instance.Init();
			EventManager.Instance.Init();
			CharacterManager.Instance.Init();

			TipManager.Instance.Init();
			
			SelectManager.Instance.Init();
			ResultManager.Instance.Init();
			QuestManager.Instance.Init();
			AbnormalConditionManager.Instance.Init();
		}

		private void InitInGame()
		{
			DiceTester.Instance.Init();
			CityManager.Instance.Init();

			this.State = GameState.Init;

			if (iconCities != null)
			{
				iconCities = GameObject.FindObjectsOfType<IconCity>();
				foreach (IconCity iconCity in iconCities)
				{
					iconCity.Init();
				}

				CityManager.Instance.InitList();
				foreach (IconCity iconCity in iconCities)
				{
					iconCity.InitNeighbors();
				}
			}

			commands = commandUIObj.GetComponentsInChildren<UICommand>();
			foreach (UICommand command in commands)
			{
				command.GetComponent<UIButton>().isEnabled = false;
			}

			TimeTable.Instance.Init();
			TimeTable.Instance.NotifyEveryWeek += WeekIsGone;
			TimeTable.Instance.NotifyEveryday += DayIsGone;
			ResetUI();

			CrackDown.Instance.Reset();

			optionObj.SetActive(false);

			curves = GameObject.FindObjectOfType<BezierCurveList>();
			if (lightSpriteTweenAlpha == null)
			{
				lightSpriteTweenAlpha = GameObject.Find("Light Sprite").GetComponent<TweenAlpha>();
			}
			else
			{
				lightSpriteTweenAlpha.GetComponent<UISprite>().alpha = lightSpriteTweenAlpha.from;
			}


			inspectAction = new Inspect();
		}

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
				c.SetEnable(false);
			}

			ClickedIconCity = city;
		}

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
					TipManager.Instance.Show(eTipTiming.Move);
					InstantiatePopup("Normal Move", eEventAction.MOVE);
					UICommandPopup busPopup = InstantiatePopup("Bus", eEventAction.MOVE_BUS);
					List<City> busCityList = CityManager.Instance.GetCityList(eWay.HIGHWAY);
					bool isHighway = busCityList.Contains(character.CurCity);
					bool buttonEnabled = (isHighway) && (character.Stat.Money >= 4);
					busPopup.moveButton.isEnabled = buttonEnabled;
					foreach(UISprite sprite in busPopup.moveButton.GetComponentsInChildren<UISprite>())
					{
						sprite.color = busPopup.moveButton.GetComponent<TweenColor>().to;
					}
					break;

				case eCommand.WORK:
					action = new Work();
					TipManager.Instance.Show(eTipTiming.Work);
					InstantiatePopup("Normal Work", eEventAction.WORK);

					break;
				case eCommand.REST:
					action = new Rest();
					TipManager.Instance.Show(eTipTiming.Rest);
					InstantiatePopup("Normal Rest", eEventAction.REST);
					InstantiatePopup("Hide Rest", eEventAction.HIDE);

					break;
				case eCommand.INVESTIGATION:
					action = new Investigation();
					TipManager.Instance.Show(eTipTiming.Investigation);
					UICommandPopup[] investigationPopups = new UICommandPopup[3];
					UICommandPopup gatheringPopup = null;

					investigationPopups[0] = InstantiatePopup("City Investigation", eEventAction.INVESTIGATION_CITY);
					investigationPopups[1] = InstantiatePopup("Broker Investigation", eEventAction.INVESTIGATION_BROKER);
					investigationPopups[2] = InstantiatePopup("Police Investigation", eEventAction.INVESTIGATION_POLICE);
					gatheringPopup = InstantiatePopup("Gathering Investigation", eEventAction.GATHERING);

					bool isMountain = (character.CurCity.Type == eNodeType.MOUNTAIN);

					foreach (UICommandPopup popup in investigationPopups)
					{
						foreach (UIButton button in popup.requiredTimeButtons)
						{
							if(isMountain == true)
							{
								button.isEnabled = false;
							}
						}
					}
					foreach (UIButton button in gatheringPopup.requiredTimeButtons)
					{
						if(isMountain == false)
						{
							button.isEnabled = false;
						}
					}
					break;
				case eCommand.SHOP:
					action = new EnterToShop();
					character.CanAction[(int)commandType] = false;
					action.RequiredTime = 1;
					isActStart = true;
					break;
				case eCommand.BROKER:
					action = new BrokerAction();
					character.CanAction[(int)commandType] = false;
					action.RequiredTime = 1;
					isActStart = true;
					break;
				case eCommand.QUEST:
					action = new QuestAction();
					character.CanAction[(int)commandType] = false;
					action.RequiredTime = 1;
					isActStart = true;
					break;
				case eCommand.ABILITY:
					action = new AbilityAction();
					character.CanAction[(int)commandType] = false;
					action.RequiredTime = 1;
					isActStart = true;
					break;
			}

			curCommandType = commandType;
		}

		private UICommandPopup InstantiatePopup(string name, eEventAction actionType)
		{
			// set the button disabled every city.
			foreach (IconCity c in iconCities)
			{
				c.SetEnable(false);
			}

			GameObject prefab = Resources.Load("Command Popup") as GameObject;
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.transform.SetParent(commandPopupGrid.transform);
			obj.transform.localScale = Vector3.one;
			UICommandPopup popup = obj.GetComponent<UICommandPopup>();
			popup.nameLabel.text = name;
			popup.actionType = actionType;

			if (actionType == (actionType & (eEventAction.MOVE | eEventAction.MOVE_BUS )))
			{
				popup.requiredTimeButtons[0].gameObject.SetActive(false);
				popup.requiredTimeButtons[1].gameObject.SetActive(false);
				popup.requiredTimeButtons[2].gameObject.SetActive(false);
			}
			else
			{
				popup.moveButton.gameObject.SetActive(false);
				popup.requiredTimeButtons[1].isEnabled = character.RemainAP >= 2;
				popup.requiredTimeButtons[2].isEnabled = character.RemainAP >= 3;
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

				int distance = character.RemainAP;
				eWay way = eWay.ENTIREWAY;
				if(actionType == eEventAction.MOVE_BUS)
				{
					distance = 1;
					way = eWay.HIGHWAY;
				}

				foreach (IconCity iconCity in this.iconCities)
				{
					iconCity.SetEnable(false);
				}
				// set the cities enabled and twinkle only you can go.
				List<City> cities = CityManager.Instance.FindCitiesByDistance(Character.CurCity, distance, way);
				foreach (City city in cities)
				{
					IconCity iconCity = Array.Find<IconCity>(iconCities, x => x.City == city);
					iconCity.SetEnable(true, city.Distance);
				}
			}
		}

		public void ChangeState(GameState state)
		{
			if (this.state != state)
				this.state = state;
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				ExitSetting();
			}

#if UNITY_EDITOR

			if (Input.GetKeyDown(KeyCode.Space))
			{
				uiEventManager.OnClickOK();
			}

			if (Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				if (Time.timeScale > 0f)
					Time.timeScale -= 1f;

				//NGUIDebug.Log("Time Scale : " + Time.timeScale);
			}

			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				if (Time.timeScale < 10f)
					Time.timeScale += 1f;

				//NGUIDebug.Log("Time Scale : " + Time.timeScale);
			}

			if (Input.GetKeyDown(KeyCode.D))
			{
				character.Stat.Agility = 5;
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

			if(Input.GetKeyDown(KeyCode.S))
			{
				SaveLoadManager.Instance.Save();
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

		private void ExitSetting()
		{
			if(endingManager.gameObject.activeSelf)
			{
				return;
			}
			if(menuObj.activeSelf && this.uiCaution.gameObject.activeSelf)
			{
				this.uiCaution.OnClickNo();
			}
			else
			{
				SwitchMenu(false);
				if (optionObj.activeSelf)
				{
					SwitchMenu(true);
				}
			}
		}

		private void SwitchMenu(bool isOption)
		{
			GameObject obj = null;
			if(isOption)
			{
				obj = optionObj;
			}
			else
			{
				obj = menuObj;
			}
			obj.SetActive(!obj.activeSelf);

			ChangeUICameraMask();

			// 주사위 보이는 상태에서 설정 키면 주사위가 보이므로 주사위쪽 카메라 꺼줌.
			if(diceObj.activeSelf)
			{
				DiceTester.Instance.demo.SetEnableCameras(!(menuObj.activeSelf || optionObj.activeSelf));
			}
		}

		public void ChangeUICameraMask()
		{
			bool isObjActive = menuObj.activeSelf || optionObj.activeSelf;
			int mask = 32; // UI Layer Num : 2 ^ 5
			if (isObjActive)
			{
				mask = 256; // Setting Layer Num : 2 ^ 8
			}
			else if(tipObj.activeSelf)
			{
				mask = 2048; // Tip Layer Num : 2 ^ 11
			}
			uiCamera.eventReceiverMask = mask;
		}

		private void DayIsGone()
		{
			state = GameState.StartDay;
		}

		private void WeekIsGone()
		{
			state = GameState.StartWeek;
		}

		private void Awake()
		{
			LanguageManager.Instance.Init();
		}

		private void Start()
		{
			this.StartCoroutine(LogoState());
		}

		#region State Routine

		
		IEnumerator InitState()
		{
			// Enter
			InitInGame();

			if (IsNew == false)
			{
				SaveLoadManager.Instance.Load();

				AbnormalConditionManager.Instance.Load(SaveLoadManager.Instance.data.abnormalList);
				
				PieceManager.Instance.Load(SaveLoadManager.Instance.data.pieceList);
				CityManager.Instance.Load(SaveLoadManager.Instance.data.cityList);

				character = CharacterManager.Instance.Load(SaveLoadManager.Instance.data.character);

				uiQuestManager.Load(SaveLoadManager.Instance.data.questList);

				yield return BuffManager.Instance.Load(SaveLoadManager.Instance.data.buffList);

				TimeTable.Instance.Load(SaveLoadManager.Instance.data.time);

				CrackDown.Instance.Load(SaveLoadManager.Instance.data.crackdown);

				TipManager.Instance.Load(SaveLoadManager.Instance.data.tipList);
			}
			else
			{
				TipManager.Instance.Set(!bWantToSeeTutorial);
			}

			yield return character.Init();
			shopUIObj.GetComponent<UIShop>().Init();

			this.languageSelection.Recall();

			yield return new WaitForSeconds(2f);
			yield return blackFader.Fade(false);

			if (IsNew)
			{
				//add polices in big cities.
				List<City> bigCityList = CityManager.Instance.FindCitiesByType(eNodeType.BIGCITY);
				foreach (City city in bigCityList)
				{
					if(city == character.CurCity)
					{
						PieceManager.Instance.Add(new Police(city, eSubjectType.POLICE, 1, 1));
					}
					else
					{
						PieceManager.Instance.Add(new Police(city, eSubjectType.POLICE));
					}
					Debug.Log("Add Big city : " + city.Name.ToString());
				}
				// load first main quest.
				Quest firstMainQuest = QuestManager.Instance.GetByIndex(15);
				if(firstMainQuest != null)
				{
					yield return new WaitForSeconds(3f);
					yield return QuestManager.Instance.Load(firstMainQuest);
				}
			}

#if UNITY_EDITOR
			// for test
			//character.Stat.Agility = 25;
			//character.Stat.InfoNum = 4;
			//character.Stat.HP = 1;
			//character.Stat.Satiety = 1;
			//yield return QuestManager.Instance.Load(QuestManager.Instance.GetByIndex(9));
			//yield return QuestManager.Instance.Load(QuestManager.Instance.GetByIndex(9));
			//yield return QuestManager.Instance.Load(QuestManager.Instance.GetByIndex(9));

			//yield return AbnormalConditionManager.Instance.Find("Fatigue").Activate(character);
			//character.Inven.AddItem(ItemManager.Instance.GetByIndex(31));
			//character.Inven.AddItem(ItemManager.Instance.GetByIndex(32));
			//character.Inven.AddItem(ItemManager.Instance.GetByIndex(36));
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[71], character);
#endif


			this.State = GameState.StartDay;

			// Exit
			yield return NextState();
		}

		IEnumerator LogoState()
		{
			yield return new WaitForSeconds(1f);

			yield return this.ChangeScene(eSceneState.Logo, false);

			yield return blackFader.Fade(false);

			this.State = GameState.Main;

			yield return NextState();
		}

		IEnumerator MainState()
		{
			AudioManager.Instance.ChangeBGM("MainMenu");

			tweenMainNew.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, 0f);
			tweenMainContinue.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, 0f);

			// Enter
			yield return this.ChangeScene(eSceneState.Main, true);

			this.state = GameState.Main;

			// Excute
			SaveLoadManager.Instance.Init();

			Init();


			tweenMainNew.PlayForward();
			tweenMainContinue.PlayForward();

			while (this.state == GameState.Main)
			{
				if (this.bClickedStart)
				{
					yield return uiCaution.Show(eLanguageKey.Popup_New);
					if (uiCaution.BClickYes)
					{
						yield return this.uiCaution.Show(eLanguageKey.Popup_Tutorial);
						this.bWantToSeeTutorial = this.uiCaution.BClickYes;

						this.state = GameState.CharacterSelect;
						AudioManager.Instance.Find("start_game").Play();
					}
				}
				this.bClickedStart = false;
				yield return new WaitForSecondsRealtime(0.1f);
			}
			
			worldObj.SetActive(true);
			
			// Exit
			yield return NextState();
		}

		IEnumerator CharacterSelectState()
		{
			uiCharacterSelect.Init();

			// Enter
			yield return this.ChangeScene(eSceneState.CharacterSelect);
			

			// Excute
			while (this.state == GameState.CharacterSelect)
			{
				yield return new WaitForSecondsRealtime(0.2f);
			}

			// Exit
			yield return NextState();
		}

		IEnumerator InGameState()
		{
			yield return blackFader.Fade(true);

			// Enter
			yield return this.ChangeScene(eSceneState.InGame, false);

			AudioManager.Instance.ChangeBGM("Main");
			
			// Excute
			this.state = GameState.Init;

			// Exit
			yield return NextState();
		}

		public void SelectMenuButton(string buttonName)
		{
			buttonName = buttonName.ToUpper();
			if(buttonName == "NEW")
			{
				this.isNew = true;
				this.bClickedStart = true;
			}
			else if(buttonName == "CONTINUE")
			{
				this.isNew = false;
				this.bClickedStart = true;
				this.state = GameState.InGame;
				AudioManager.Instance.Find("start_game").Play();
			}
			else if(buttonName == "MENU")
			{
				SwitchMenu(false);
			}
			else if(buttonName == "OPTION")
			{
				SwitchMenu(false);
				SwitchMenu(true);
			}
			else if(buttonName == "EXIT")
			{
				if(this.state == GameState.Main)
				{
					Application.Quit();
				}
				else
				{
					StartCoroutine(ExitFromInGame());
				}
			}
			else if(buttonName == "EXIT_SETTING")
			{
				ExitSetting();
			}
		}

		private IEnumerator ExitFromInGame()
		{
			yield return this.uiCaution.Show(eLanguageKey.Popup_EXIT);
			if(this.uiCaution.BClickYes)
			{
				tipObj.SetActive(false);
				SwitchMenu(false);
				StopAllCoroutines();

				StartCoroutine(ChangeToMain());
			}
		}

		public IEnumerator ChangeToMain()
		{
			worldObj.SetActive(false);
			
			this.state = GameState.Main;
			yield return NextState();
		}

		private void ResetUI()
		{
			TimeTable.Instance.Reset();
			CrackDown.Instance.Reset();
			BuffManager.Instance.Reset();
			PieceManager.Instance.Reset();

			uiEventManager.Reset();

			commandPopupGrid.transform.DestroyChildren();
			curCommandType = eCommand.NULL;

			DiceTester.Instance.demo.SetEnableCameras(true);

			//GC.Collect();
			//GC.WaitForPendingFinalizers();
		}

		public IEnumerator ChangeScene(eSceneState sceneState, bool fade = true)
		{
			int iSceneState = (int)sceneState;
			if(iSceneState >= scenes.Length)
			{
				Debug.LogError("scenes' length longer than sceneState : " + scenes.Length + ", " + iSceneState);
				yield break;
			}
			
			if (fade)
			{
				yield return blackFader.Fade(true);
			}
			for (int i = 0; i < scenes.Length; ++i)
			{
				scenes[i].SetActive(i == iSceneState);
			}
			if (fade)
			{
				yield return blackFader.Fade(false);
			}

			yield return null;
		}

		IEnumerator StartWeekState()
		{
			// Enter
			yield return (Instance_NotifyEveryWeek());

			yield return GameManager.Instance.ShowStateLabel(LanguageManager.Instance.Find(eLanguageKey.UI_Your_Turn), 1f);

			action = null;
			yield return null;

			// Excute
			this.State = GameState.StartDay;
			yield return null;

			// Exit
			yield return NextState();
		}

		private IEnumerator Instance_NotifyEveryWeek()
		{
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.WEEK, character);
			
			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.WEEK, character);
		}

		IEnumerator StartDayState()
		{
			// Enter
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.DAY, character);

			if (character.Stat.HP <= 0)
			{
				yield return GameManager.Instance.endingManager.StartEnding(eEnding.STARVATION);
			}

#if UNITY_EDITOR
			// for test
			//PieceManager.Instance.Add(new Broker(character.CurCity, eSubjectType.BROKER));
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[26], character);
			//for(int i = 0; i < 3; ++i) yield return QuestManager.Instance.Load(QuestManager.Instance.GetByIndex(5), character);
			//character.Stat.Satiety = 5;
			//character.Stat.SetViewRange();

			//Police police = PieceManager.Instance.FindRand(eSubjectType.POLICE) as Police;

			//UICenterOnChild scrollviewCenter = GameObject.FindObjectOfType<UICenterOnChild>();
			//scrollviewCenter.enabled = true;
			//scrollviewCenter.CenterOn(police.IconPiece.transform);
			//scrollviewCenter.enabled = false;

			//yield return police.AddStat(false);

			///character.Inven.AddItem(ItemManager.Instance.GetByIndex(12));


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

			// 하루 시작 이벤트
			//if (character.IsDetention == false)
			//{
			//	if (character.CheckSpecialEvent())
			//	{
			//		yield return EventManager.Instance.ActivateEvent(EventManager.Instance.Find(eEventAction.START), character);
			//	}
			//}

			if (character.IsDetention)
			{
				if (character.ArrestedDate == TimeTable.Instance.Day)
				{
					yield return TimeTable.Instance.SpendTime(character.TotalAP, eSpendTime.END);
					this.State = GameState.Night;
				}
				else 
				{
					this.State = GameState.Detention;
				}
				
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
			TipManager.Instance.Show(eTipTiming.Act);
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
			if (character.IsActionSkip == false)
			{
				if(character.IsDetention == false)
				{
					yield return action.Activate(character);
				}
			}
			else
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
			if (action is DetentionAction == false)
			{
				action = new DetentionAction();
				AudioManager.Instance.ChangeBGM("Detention");
			}
			yield return action.Activate(character);
			
			this.State = GameState.Night;

			// Exit
			yield return NextState();
		}

		IEnumerator NightState()
		{
			// Enter
			lightSpriteTweenAlpha.PlayForward();
			
			yield return TimeTable.Instance.SpendTime(1, eSpendTime.END);

			yield return Instance_NotifyEveryNight();

			#region editor test
#if UNITY_EDITOR
			// for the test
			if (activateAbnormal)
			{
				//foreach(AbnormalCondition ab in AbnormalConditionManager.Instance.List)
				//    yield return ab.Activate(character);

				yield return AbnormalConditionManager.Instance.GetByIndex(5).Activate(character);
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


			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[89], character);

			// 공안 만나는 이벤트
			//yield return EventManager.Instance.ActivateEvent(EventManager.Instance.List[11], character);

			// 공안 스탯 증가
			//Police p = PieceManager.Instance.Find(eSubjectType.POLICE, CityManager.Instance.Find("TUMEN")) as Police;
			//if(p != null)
			//{
			//	yield return p.AddStat(CrackDown.Instance.IsCrackDown);
			//}
			
#endif
			#endregion

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.DAY, character);

			TimeTable.Instance.DayIsGone();

			lightSpriteTweenAlpha.PlayReverse();

			// after daytime // Temporary
			yield return BuffManager.Instance.CheckDuration(character);

			

			while (state == GameState.Night)
			{
				yield return null;
			}

			yield return CrackDown.Instance.Check();

			// 집중 단속 시 모든 공안 이동력만큼 움직이기
			yield return CrackDown.Instance.MoveEveryPolice();

			// 스페셜 이벤트 확률 증가
			character.AddSpecialEventProbability();

			SaveLoadManager.Instance.Save();

			// Exit
			yield return NextState();
		}

		private IEnumerator Instance_NotifyEveryNight()
		{
			yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.NIGHT, character);

			// 구금 상태일 때 밤단계 탈출 시도를 위한.
			if (character.IsDetention == true)
			{
				if (action is DetentionAction == false)
				{
					action = new DetentionAction();
				}
				yield return action.Activate(character);
			}

			character.ResetAPandAction();

			character.Stat.Satiety--;

			yield return AbnormalConditionManager.Instance.ActiveByCondition();

			//check current quest's end time and apply the result
			yield return uiQuestManager.TreatPastQuests();

			yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.NIGHT, character);
		}

		#endregion

		protected IEnumerator NextState()
		{
			string methodName = EnumConvert<GameState>.ToString(State) + "State";
			System.Reflection.MethodInfo info = GetType().GetMethod(methodName,
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			yield return ((IEnumerator)info.Invoke(this, null));
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
			yield break;

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
			yield break;

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

		public Character Character
		{
			get
			{
				return character;
			}
			set
			{
				character = value;
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

		public bool IsNew
		{
			get
			{
				return isNew;
			}
		}

		public int IncreasingSpecialEventProbability
		{
			get
			{
				return increasingSpecialEventProbability;
			}
		}

		public int PoliceTurnDays
		{
			get
			{
				return policeTurnDays;
			}
		}
	}
}
 