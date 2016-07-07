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
            yield return (ShowStateLabel("Init State", 1f));

            Inventory inven = new Inventory(3);
            character = new Character("Chris", new Stat(), CityManager.Instance.Find(eCity.YANBIAN), 5, 3, 0, 5, 5, inven);
            character.MoveTo(character.CurCity);

            inspectAction = new Inspect();

            yield return (ShowStateLabel("Adding Polices to Big cities.", 1f));

            // add polices in big cities.
            List<City> bigCityList = CityManager.Instance.FindCitiesBySize(eCitySize.BIG);
            List<Transform> bigCityTransformList = new List<Transform>();
            foreach (City city in bigCityList)
            {
                PieceManager.Instance.Add(new Police(city, eSubjectType.POLICE));
                bigCityTransformList.Add(GameObject.Find(city.Name.ToString()).transform);
                NGUIDebug.Log("Add Big city : " + city.Name.ToString());
            }

            yield return (MoveDirectingCam(bigCityTransformList, 1f));
            

            yield return null;


            // Excute
            //Instance_NotifyEveryWeek();

            //TimeTable.Instance.NotifyEveryWeek += Instance_NotifyEveryWeek;
            //TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;

            this.State = GameState.StartWeek;

            yield return null;

            // Exit
            NextState();
        }
        
        IEnumerator StartWeekState()
        {
            // Enter
            yield return (ShowStateLabel("Start Week State", 1f));
            

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
            yield return (ShowStateLabel("Start Day State", 1f));
            
            this.State = GameState.Act;
            
            // Exit
            NextState();
        }

        IEnumerator ActState()
        {
            // Enter
            yield return (ShowStateLabel("Act State", 1f));
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

            // activate selected event
            yield return (action.Activate(character));

            this.State = GameState.Night;

            // Exit
            NextState();
        }

        IEnumerator NightState()
        {
            // Enter
            yield return (ShowStateLabel("Night State", 1f));

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
        /*
        private void Start()
        {
            inspectAction = new Inspect();

            Effect effect_Cure_HP = new Effect(eSubjectType.CHARACTER, eVerbType.ADD, eObjectType.HP);
            Buff buff_cureHP_Now_Once = new Buff("cure hp 1", effect_Cure_HP, false, 1, eStartTime.NOW, eDuration.ONCE);
            Item cureHP_Now_Once = new Item("cure hp 1", buff_cureHP_Now_Once, 10, 1);
            Item cureBoth_RestEquip = new Item("cureBoth_RestEquip", 
                new Buff("Buff : Cure both rest equip", 
                    new Effect(eSubjectType.CHARACTER, eVerbType.ADD, eObjectType.HP_MENTAL), false, 1, eStartTime.REST, eDuration.EQUIP), 
                10, 1);
            Item addSTR_Work_Once = new Item("add str when working once", 
                new Buff("Buff : add str when working once", 
                    new Effect(eSubjectType.STAT, eVerbType.ADD, eObjectType.STRENGTH), true, 1, eStartTime.TEST, eDuration.ONCE), 
                10, 1);
            Item addSTR_Work_Equip = new Item("add str when working equip", 
                    new Buff("add str when working equip", new Effect(eSubjectType.STAT, eVerbType.ADD, eObjectType.STRENGTH), true, 1, eStartTime.TEST, eDuration.EQUIP), 
                10, 1);
            Item addAGI_Move_Once = new Item("addAGI_Move_Once", 
                    new Buff("Buff : addAGI_Move_Once", new Effect(eSubjectType.STAT, eVerbType.ADD, eObjectType.AGILITY), true, 1, eStartTime.TEST, eDuration.ONCE), 
                10, 1);
            Item delPoliceNearOnce = new Item("del police near once", 
                    new Buff("Buff : del police near once", new Effect(eSubjectType.POLICE, eVerbType.DEL, eObjectType.CLOSE), false, 1, eStartTime.NOW, eDuration.ONCE), 
                10, 1);

            
            //City cityA = new City("A", "Big", "North", new List<Item>() { cureHP_Now_Once }, 5, 7);
            //City cityB = new City("B", "Midium", "South", new List<Item>() { cureHP_Now_Once }, 3, 5);
            //City cityC = new City("C", "Small", "East", new List<Item>() { cureHP_Now_Once }, 1, 3);
            //City cityC2 = new City("C2", "Big", "East", new List<Item>() { cureHP_Now_Once }, 5, 7);
            //City cityD = new City("D", "Midium", "East", new List<Item>() { cureHP_Now_Once }, 3, 5);

            CityGraph.Instance.Add(cityA);
            CityGraph.Instance.Add(cityB);
            CityGraph.Instance.Add(cityC);
            CityGraph.Instance.Add(cityC2);
            CityGraph.Instance.Add(cityD);

            CityGraph.Instance.Link(cityA, cityB);
            CityGraph.Instance.Link(cityB, cityC);
            CityGraph.Instance.Link(cityB, cityC2);
            CityGraph.Instance.Link(cityC, cityD);
            CityGraph.Instance.Link(cityC2, cityD);


            ResultEffect[] successResultEffects = new ResultEffect[1] { new ResultEffect(0, effect_Cure_HP, null, 1) };
            ResultEffect[] failureResultEffects = new ResultEffect[1] { new ResultEffect(0, effect_Cure_HP, null, -1) };
            

            ResultScriptAndEffects success = new ResultScriptAndEffects("success", successResultEffects);
            ResultScriptAndEffects failure = new ResultScriptAndEffects("failure", failureResultEffects);
            

            //Result result_strength = new Result("STR", success, failure);
            //Result result_agility = new Result("AGI", success, failure);
            //Result result_observation = new Result("OBS", success, failure);
            //Result result_global = new Result(string.Empty, failure, null);
            //Result result_quest = new Result(string.Empty, success, failure);


            //Select select_quest = new Select(eSubjectType.CHARACTER, eObjectType.HP, ">=", 1, "select cure hp > 1", result_quest);

            //Event event_move = new Event("MOVE", "A", "AGI", "move agility test, A city", result_agility, false, null);
            //Event event_Inspection = new Event("INSPECT", "ALL", "OBS", "Inspection observation test, B city", result_observation, false, null);
            //Event event_work_A = new Event("WORK", "A", "STR", "police strength test, A city", result_strength, false, null);
            //Event event_work_B = new Event("WORK", "B", "STR", "police agility test, A city", result_strength, false, null);
            //Event event_global = new Event("GLOBAL", "ALL", string.Empty, "global event", result_global, false, null);
            //Event event_quest = new Event("QUEST", "ALL", string.Empty, "quest", result_quest, true, new Select[1] { select_quest });
            //Event event_globalquest = new Event("QUEST", "ALL", string.Empty, "quest", result_quest, false, null);

            //EffectDataList effectDataList = new EffectDataList(Application.streamingAssetsPath + "/Effect.json");
            Debug.LogWarning(EffectManager.Instance.List.Length);
            // abnormal condition : despair
            Effect effect_dice_successNum = new Effect(eSubjectType.DICE, eVerbType.NONE, eObjectType.SUCCESSNUM);
            Condition spawnCondition_despair = new Condition("MENTAL", string.Empty, string.Empty, "<=", 0);
            Buff despairBuff = new Buff("Despair buff", effect_dice_successNum, true, 6, eStartTime.TEST, eDuration.PAT_TEST_REST, false);
            AbnormalCondition despair = new Despair("despair", despairBuff, spawnCondition_despair, false, false);

            ResultEffect[] resultEffects_dice_successNum_6 = new ResultEffect[1] { new ResultEffect(2, null, despair, 6) };
            ResultScriptAndEffects failure_diceNum_6 = new ResultScriptAndEffects("dice num 6", resultEffects_dice_successNum_6);
            Result result_obs_diceNum_6 = new Result(eStat.OBSERVATION, failure_diceNum_6, failure_diceNum_6);
            //Event event_Inspection_despair = new Event("INSPECT", "A", "OBS", "Inspection observation test", result_obs_diceNum_6, false, null);

            //EventManager.Instance.EveryEvents.Add(event_move);
            //EventManager.Instance.EveryEvents.Add(event_Inspection);
            //EventManager.Instance.EveryEvents.Add(event_work_A);
            //EventManager.Instance.EveryEvents.Add(event_work_B);
            //EventManager.Instance.EveryEvents.Add(event_global);
            //EventManager.Instance.EveryEvents.Add(event_quest);
            //EventManager.Instance.EveryEvents.Add(event_globalquest);
            //EventManager.Instance.EveryEvents.Add(event_Inspection_despair);

            List<int> regionProbDataList = new List<int>();
            regionProbDataList.Add(10);
            regionProbDataList.Add(80);
            regionProbDataList.Add(10);
            Probability regionProbforMove = new Probability("MOVE", new List<int>(regionProbDataList));
            Probability regionProbforWork = new Probability("WORK", regionProbDataList);
            Probability regionProbforInfo = new Probability("INFO", regionProbDataList);
            Probability regionProbforBroker = new Probability("BROKER", regionProbDataList);
            Probability regionProbforInspection = new Probability("INSPECT", regionProbDataList);
            Probability regionProbforTaken = new Probability("TAKEN", regionProbDataList);
            Probability regionProbforEscape = new Probability("ESCAPE", regionProbDataList);
            Probability regionProbforGlobal = new Probability("GLOBAL", regionProbDataList);
            Probability regionProbforQuest = new Probability("QUEST", regionProbDataList);
            Probability[] regionProbs = new Probability[9] {
                regionProbforWork, regionProbforMove, regionProbforInfo, regionProbforBroker,
                regionProbforInspection, regionProbforTaken, regionProbforEscape, regionProbforGlobal, regionProbforQuest };

            List<int> statProbDataList = new List<int>();
            statProbDataList.Add(50);
            statProbDataList.Add(10);
            statProbDataList.Add(10);
            statProbDataList.Add(10);
            statProbDataList.Add(10);
            statProbDataList.Add(10);
            Probability statProbforMove = new Probability("MOVE", statProbDataList);
            Probability statProbforWork = new Probability("WORK", statProbDataList);
            Probability statProbforInfo = new Probability("INFO", statProbDataList);
            Probability statProbforBroker = new Probability("BROKER", statProbDataList);
            Probability statProbforInspection = new Probability("INSPECT", statProbDataList);
            Probability statProbforTaken = new Probability("TAKEN", statProbDataList);
            Probability statProbforEscape = new Probability("ESCAPE", statProbDataList);
            Probability statProbforGlobal = new Probability("GLOBAL", statProbDataList);
            Probability statProbforQuest = new Probability("QUEST", regionProbDataList);
            Probability[] statProbs = new Probability[9] {
                statProbforMove, statProbforWork, statProbforInfo, statProbforBroker, statProbforInspection,
                statProbforTaken, statProbforEscape, statProbforGlobal, statProbforQuest
            };

            ProbabilityManager.Instance.Init(regionProbs, statProbs);

            Inventory inven = new Inventory(30);
            character = new Character("Chris", new Stat(),
                                    cityA, 5, 3, 0, 5, 5, inven);

            // set character's start cureHP_Now_Onces.
            character.Inven.AddItem(new Item("food", new Buff("food buff", new Effect(eSubjectType.CHARACTER, eVerbType.ADD, eObjectType.FOOD), false, 1, eStartTime.NOW, eDuration.ONCE), 2, 1), character);
            character.Inven.AddItem(cureHP_Now_Once, character);
            character.Inven.AddItem(cureBoth_RestEquip, character);
            character.Inven.AddItem(addSTR_Work_Once, character);
            character.Inven.AddItem(addSTR_Work_Equip, character);
            character.Inven.AddItem(addAGI_Move_Once, character);
            character.Inven.AddItem(delPoliceNearOnce, character);
            //// cureHP_Now_Once use test
            //character.Inven.UseItem(character, 0, effect);

            // init time calendar.

            // init CityGraph and put polices in big cities.
            PieceManager.Instance.Init();
            Instance_NotifyEveryWeek();
            TimeTable.Instance.NotifyEveryWeek += Instance_NotifyEveryWeek;
        }
        */

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
                    pastQuestPiece.TreatPastQuests(character);
                }
            }

            yield return StartCoroutine(PutPieces());
                     
            // activate global event
            yield return StartCoroutine(EventManager.Instance.DoCommand(eEventAction.GLOBAL, character));

        }

        private IEnumerator PutPieces()
        {
            yield return StartCoroutine(ShowStateLabel("Put Pieces", 1f));

            // put pieces in one of random cities (police, information, quest)
            int distance = 2;
            List<Transform> pieceCityTransformList = new List<Transform>();
            // 1 random quest
            Quest selectedQuest = QuestManager.Instance.FindRand();
            if(CityManager.Instance == null)
            {
                Debug.LogError("CityManager.Instance is null");
            }
            City city = CityManager.Instance.FindRandCityByDistance(character.CurCity, distance);
            QuestPiece questPiece = new QuestPiece(selectedQuest, character, city, eSubjectType.QUEST);
            PieceManager.Instance.Add(questPiece);
            pieceCityTransformList.Add(GameObject.Find(questPiece.City.Name.ToString()).transform);
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


            yield return StartCoroutine(MoveDirectingCam(pieceCityTransformList, 1f));
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

        private IEnumerator MoveDirectingCam(List<Transform> transformList, float duration)
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