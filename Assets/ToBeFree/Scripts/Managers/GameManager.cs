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
            Night
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

        public void WorkEvent()
        {
            action = new Work();
            NGUIDebug.Log("Command Work input");
        }

        public void RestEvent()
        {
            action = new Rest();
            NGUIDebug.Log("Command Rest input");
        }

        public void QuestEvent()
        {
            action = new QuestAction();
            NGUIDebug.Log("Command Quest input");
        }

        public void ShopEvent()
        {
            action = new EnterToShop();
            NGUIDebug.Log("Command Shop input");
        }

        private void Update()
        {
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

        //public T FindObjectOfType<T>(this UnityEngine.Object unityObject) where T : UnityEngine.Object
        //{
        //    return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
        //}

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
            // Enter
            yield return (ShowStateLabel("Night State", 0.5f));

            BuffManager.Instance.CheckStartTimeAndActivate(eStartTime.NIGHT, character);

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

        private void Instance_NotifyEveryday()
        {
            character.Inven.CheckItem(eStartTime.NIGHT, character);
            if (character.Stat.FOOD <= 0)
            {
                character.Stat.HP--;
            }
            else
            {
                character.Stat.FOOD--;
            }
        }

        private IEnumerator Instance_NotifyEveryWeek()
        {
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
            City city = null;
            if (selectedQuest.Event_ != null)
            {
                city = CityManager.Instance.FindRandCityByDistance(character.CurCity, distance);
                pieceCityTransformList.Add(GameObject.Find(city.Name.ToString()).transform);
            }
            QuestPiece questPiece = new QuestPiece(selectedQuest, character, city, eSubjectType.QUEST);
            uiEventManager.OpenUI();
            uiEventManager.OnChanged(eUIEventLabelType.EVENT, selectedQuest.Script);
            yield return EventManager.Instance.WaitUntilFinish();

            PieceManager.Instance.Add(questPiece);
            
            
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
        }

        public IEnumerator ShowStateLabel(string text, float duration)
        {
            stateLabel.text = text;
            TweenAlpha tweenAlpha = stateLabel.transform.GetComponent<TweenAlpha>();
            tweenAlpha.enabled = true;
            tweenAlpha.style = UITweener.Style.Once;
            tweenAlpha.duration = duration;

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
                yield return new WaitForSeconds(duration);
            }
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