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

        private Character character;
        private Action action;
        private Action inspectAction;

        private GameState state;

        // can't use the constructor
        private GameManager()
        {
        }

        public void MoveEvent()
        {            
            NGUIDebug.Log("Command Move input");
            character.CurCity.PrintNeighbors();
        }

        public void ClickCity(string cityName)
        {
            if (!character.CurCity.IsNeighbor(cityName))
            {
                Debug.LogError("ClickCity : " + cityName + " is not neighbor.");
                return;
            }
            action = new Move();
            character.NextCity = CityManager.Instance.Find(cityName);
        }

        public void ClickCommand(string command)
        {
            switch(EnumConvert<eCommand>.ToEnum(command))
            {
                case eCommand.WORK:
                    action = new Work();
                    break;
                case eCommand.REST:
                    action = new Rest();
                    break;
                case eCommand.QUEST:
                    action = new QuestAction();
                    break;
                case eCommand.SHOP:
                    action = new EnterToShop();
                    break;
                case eCommand.INFO:
                    action = new InfoAction();
                    break;
                case eCommand.BROKER:
                    action = new BrokerAction();
                    break;
            }
        }

        private void Update()
        {

            if(Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if(Time.timeScale >= 0f)
                    Time.timeScale -= .2f;
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if(Time.timeScale <= 3f)
                    Time.timeScale += .2f;
            }

            if(Input.GetKeyDown(KeyCode.D))
            {
                character.IsDetention = true;
            }
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

        private void Awake()
        {
            this.State = GameState.Init;
            commandUIObj.SetActive(false);
            shopUIObj.SetActive(false);
            TimeTable.Instance.NotifyEveryWeek += WeekIsGone;
            TimeTable.Instance.NotifyEveryday += DayIsGone;
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

            Inventory inven = new Inventory(3);
            character = new Character("Chris", new Stat(), CityManager.Instance.Find(eCity.YANBIAN), 5, 3, 0, 5, 5, inven);

            yield return character.MoveTo(character.CurCity);

            //CityManager.Instance.FindNearestPathToStartCity(CityManager.Instance.Find(eCity.KUNMING), CityManager.Instance.Find(eCity.DANDONG));
            

            inspectAction = new Inspect();

            yield return (ShowStateLabel("Adding Polices to Big cities.", 0.5f));

            // add polices in big cities.
            List<City> bigCityList = CityManager.Instance.FindCitiesBySize(eCitySize.BIG);
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
            character.Stat.InfoNum = 2;

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

            action = null;
            while (action == null)
            {
                yield return null;
            }
            commandUIObj.SetActive(false);
            // if selected event is not move,
            // check polices in current city and activate police events.
            if (!(action is Move))
            {
                yield return (inspectAction.Activate(character));                
            }

            if (character.IsDetention == false)
            {
                // activate selected event
                yield return (action.Activate(character));
            }

            //Item
            //startime
            //eDuration
                

            
            this.State = GameState.Night;

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
            yield return character.Inven.CheckItem(eStartTime.NIGHT, character);

            yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.NIGHT, character);

            if (character.IsFull)
            {
                yield return GameManager.Instance.ShowStateLabel("Skip Food", 0.5f);
                yield break;
            }

            if (character.Stat.FOOD <= 0)
            {
                character.Stat.HP--;
            }
            else
            {
                character.Stat.FOOD--;
            }

            yield return BuffManager.Instance.DeactivateEffectByStartTime(eStartTime.NIGHT, character);
        }

        private IEnumerator Instance_NotifyEveryWeek()
        {
            yield return character.Inven.CheckItem(eStartTime.WEEK, character);

            yield return BuffManager.Instance.ActivateEffectByStartTime(eStartTime.WEEK, character);
            
            // check current quest's end time and apply the result
            List<Piece> questPieces = PieceManager.Instance.FindAll(eSubjectType.QUEST);
            if (questPieces != null && questPieces.Count > 0)
            {
                List<Piece> pastQuestPieces = questPieces.FindAll(x => x.CheckDuration());
                foreach (QuestPiece pastQuestPiece in pastQuestPieces)
                {
                    yield return pastQuestPiece.TreatPastQuests(character);
                }
            }

            yield return PutPieces();
                     
            // activate global event
            yield return EventManager.Instance.DoCommand(eEventAction.GLOBAL, character);

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

            if (selectedQuest.Event_ != null)
            {
                QuestPiece piece = PieceManager.Instance.Find(selectedQuest);
                pieceCityTransformList.Add(GameObject.Find(piece.City.Name.ToString()).transform);
            }
            

            // 2 polices
            Police randPolice = new Police(CityManager.Instance.FindRand(), eSubjectType.POLICE);
            PieceManager.Instance.Add(randPolice);
            pieceCityTransformList.Add(GameObject.Find(randPolice.City.Name.ToString()).transform);
            Police randPoliceByDistance = new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance), eSubjectType.POLICE);
            PieceManager.Instance.Add(randPoliceByDistance);
            pieceCityTransformList.Add(GameObject.Find(randPoliceByDistance.City.Name.ToString()).transform);

            // 2 informations
            Information randInfo = new Information(CityManager.Instance.FindRand(), eSubjectType.INFO);
            PieceManager.Instance.Add(randInfo);
            pieceCityTransformList.Add(GameObject.Find(randInfo.City.Name.ToString()).transform);
            Information randInfoByDistance = new Information(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance), eSubjectType.INFO);
            PieceManager.Instance.Add(randInfoByDistance);
            pieceCityTransformList.Add(GameObject.Find(randInfoByDistance.City.Name.ToString()).transform);

            yield return MoveDirectingCam(pieceCityTransformList, 0.5f);

            // temporary
            PieceManager.Instance.Add(new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, 0), eSubjectType.POLICE));
            PieceManager.Instance.Add(new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, 0), eSubjectType.POLICE));
            PieceManager.Instance.Add(new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, 0), eSubjectType.POLICE));
            PieceManager.Instance.Add(new Police(CityManager.Instance.FindRandCityByDistance(character.CurCity, 0), eSubjectType.POLICE));

            //Information randInfo2 = new Information(CityManager.Instance.FindRand(), eSubjectType.INFO);
            //yield return PieceManager.Instance.Move(randInfo, CityManager.Instance.FindRand());
            //pieceCityTransformList.Add(GameObject.Find(randInfo.City.Name.ToString()).transform);

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