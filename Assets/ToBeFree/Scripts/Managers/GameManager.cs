using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private Character character;
        private Action action;
        private Action inspectAction;
        
        // can't use the constructor
        private GameManager()
        {
        }
         
        public void MoveEvent()
        {
            action = new Move();
            Debug.LogWarning("Command Move input");
            character.CurCity.PrintNeighbors();
        }

        public void WorkEvent()
        {
            action = new Work();
            Debug.LogWarning("Command Work input");
            ExcuteCommand();
        }

        public void RestEvent()
        {
            action = new Rest();
            Debug.LogWarning("Command Rest input");
            ExcuteCommand();
        }

        public void QuestEvent()
        {
            action = new QuestAction();
            Debug.LogWarning("Command Quest input");
            ExcuteCommand();
        }

        public void ClickCity(City city)
        {
            if (action is Move)
            {
                foreach(City neighbor in character.CurCity.NeighborList)
                {
                    if(neighbor.Name == city.Name)
                    {
                        action.Activate(character, city);
                        return;
                    }
                }
                Debug.Log("This city is not neighbor of current city.");
            }
        }

        private void Update()
        {
            // await for the event command
            if (Input.GetKeyDown(KeyCode.W))
            {
                action = new Work();
                Debug.LogWarning("Command Work input");
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                action = new Move();
                Debug.LogWarning("Command Move input");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                action = new Rest();
                Debug.LogWarning("Command Rest input");
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                action = new QuestAction();
                Debug.LogWarning("Command Quest input");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //character.Inven.UseItem(character.Inven.FindItemByType("POLICE", "DEL", "CLOSE"), character);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExcuteCommand();
            }
        }

        private void ExcuteCommand()
        {
            // if selected event is not move,
            // check polices in current city and activate police events.
            if (!(action is Move))
            {
                inspectAction.Activate(character);
            }

            // activate selected event
            if (action != null)
            {
                action.Activate(character);
            }

            TimeTable.Instance.DayIsGone();
        }

        private void Awake()
        {
            TimeTable.Instance.NotifyEveryWeek += Instance_NotifyEveryWeek;
            TimeTable.Instance.NotifyEveryday += Instance_NotifyEveryday;
            PieceManager.Instance.Init();
        }

        private void Start()
        {
            Debug.LogWarning(EffectManager.Instance.List.Length);
            Debug.LogWarning(SelectManager.Instance.List[EventManager.Instance.List[13].SelectIndexList[0]].Event.Script);

            Inventory inven = new Inventory(3);
            character = new Character("Chris", new Stat(), CityManager.Instance.Find(eCity.YANBIAN), 5, 3, 0, 5, 5, inven);

            inspectAction = new Inspect();

            Instance_NotifyEveryWeek();
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

        private void Instance_NotifyEveryWeek()
        {
            // check current quest's end time and apply the result

            // put pieces in one of random cities (police, information, quest)
            int distance = 2;
            // 2 polices
            PieceManager.Instance.Add(CityManager.Instance.FindRand(), eSubjectType.POLICE);
            PieceManager.Instance.Add(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance), eSubjectType.POLICE);
            // 2 informations
            PieceManager.Instance.Add(CityManager.Instance.FindRand(), eSubjectType.INFO);
            PieceManager.Instance.Add(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance), eSubjectType.INFO);
            // 1 quest
            //Event selectedEvent = EventManager.Instance.Find(eEventAction.QUEST, character.CurCity);
            //PieceManager.Instance.AddQuest(CityManager.Instance.FindRandCityByDistance(character.CurCity, distance), character, selectedEvent);

            // activate global event
            Event globalEvent = EventManager.Instance.DoCommand(eEventAction.GLOBAL, character);
        }

        public Character Character
        {
            get
            {
                return character;
            }
        }
    }
}