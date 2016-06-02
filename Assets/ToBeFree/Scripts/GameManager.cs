using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private Character character;
        private string command;
        private Action action;
        private Action inspectAction;

        protected GameManager()
        {
        } // can't use the constructor

        private void Update()
        {
            // await for the event command
            if (Input.GetKeyDown(KeyCode.W))
            {
                action = new Work();
                Debug.Log("Command Work input");
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                action = new Move();
                Debug.Log("Command Move input");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                action = new Rest();
                Debug.Log("Command Rest input");
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                action = new QuestAction();
                Debug.Log("Command Quest input");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                character.Inven.UseItem(character.Inven.FindItemByType("POLICE", "DEL", "CLOSE"), character);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // if selected event is not move,
                // check polices in current city and activate police events.
                if (!(action is Move))
                {
                    inspectAction.Activate(character);
                }

                // activate selected event
                action.Activate(character);

                TimeTable.Instance.DayIsGone();
            }
        }

        private void Start()
        {
            inspectAction = new Inspect();

            Effect effect = new Effect("CURE", "HP", string.Empty);

            Item cureHP_Now_Once = new Item("cure hp 1", effect, eStartTime.NOW, eDuration.ONCE, false, 1, 10, 1);
            Item cureBoth_RestEquip = new Item("cureBoth_RestEquip", new Effect("CURE", "BOTH"), eStartTime.REST, eDuration.EQUIP, false, 1, 10, 1);
            Item addSTR_Work_Once = new Item("add str when working once", new Effect("STAT", "STR"), eStartTime.WORK, eDuration.ONCE, true, 1, 10, 1);
            Item addSTR_Work_Equip = new Item("add str when working equip", new Effect("STAT", "STR"), eStartTime.WORK, eDuration.EQUIP, true, 1, 10, 1);
            Item addAGI_Move_Once = new Item("addAGI_Move_Once", new Effect("STAT", "AGI"), eStartTime.TEST, eDuration.ONCE, true, 1, 1, 1);
            Item delPoliceNearOnce = new Item("del police near once", new Effect("POLICE", "DEL", "CLOSE"), eStartTime.NOW, eDuration.ONCE, false, 1, 1, 1);

            
            City cityA = new City("A", "Big", "North", new List<Item>() { cureHP_Now_Once }, 5, 7);
            City cityB = new City("B", "Midium", "South", new List<Item>() { cureHP_Now_Once }, 3, 5);
            City cityC = new City("C", "Small", "East", new List<Item>() { cureHP_Now_Once }, 1, 3);
            City cityC2 = new City("C2", "Big", "East", new List<Item>() { cureHP_Now_Once }, 5, 7);
            City cityD = new City("D", "Midium", "East", new List<Item>() { cureHP_Now_Once }, 3, 5);

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


            ResultEffect[] successResultEffects = new ResultEffect[1] { new ResultEffect(0, effect, null, 1) };
            ResultEffect[] failureResultEffects = new ResultEffect[1] { new ResultEffect(0, effect, null, -1) };
            

            ResultScriptAndEffects success = new ResultScriptAndEffects("success", successResultEffects);
            ResultScriptAndEffects failure = new ResultScriptAndEffects("failure", failureResultEffects);
            

            Result result_strength = new Result("STR", success, failure);
            Result result_agility = new Result("AGI", success, failure);
            Result result_observation = new Result("OBS", success, failure);
            Result result_global = new Result(string.Empty, failure, null);
            Result result_quest = new Result(string.Empty, success, failure);


            Select select_quest = new Select("CURE", "HP", ">=", 1, "select cure hp > 1", result_quest);

            Event event_move = new Event("Move", "A", "AGI", "move strength test, A city", result_agility, false, null);
            Event event_Inspection = new Event("Inspect", "B", "OBS", "Inspection agility test, A city", result_observation, false, null);
            Event event_work_A = new Event("Work", "A", "STR", "police agility test, A city", result_strength, false, null);
            Event event_work_B = new Event("Work", "B", "STR", "police agility test, A city", result_strength, false, null);
            Event event_global = new Event("Global", "ALL", string.Empty, "global event", result_global, false, null);
            Event event_quest = new Event("Quest", "ALL", string.Empty, "quest", result_quest, true, new Select[1] { select_quest });
            Event event_globalquest = new Event("Quest", "ALL", string.Empty, "quest", result_quest, false, null);

            // abnormal condition : despair
            Effect effect_dice_successNum = new Effect("DICE", "SUCCESS NUM", string.Empty);
            Condition spawnCondition_despair = new Condition("MENTAL", string.Empty, string.Empty, "<=", 0);
            Buff despairBuff = new Buff("Despair buff", effect_dice_successNum, true, 6, eStartTime.TEST, eDuration.PAT_TEST_REST, false);
            AbnormalCondition despair = new Despair("despair", despairBuff, spawnCondition_despair, false, false);

            ResultEffect[] resultEffects_dice_successNum_6 = new ResultEffect[1] { new ResultEffect(2, null, despair, 6) };
            ResultScriptAndEffects failure_diceNum_6 = new ResultScriptAndEffects("dice num 6", resultEffects_dice_successNum_6);
            Result result_obs_diceNum_6 = new Result("OBS", failure_diceNum_6, failure_diceNum_6);
            Event event_Inspection_despair = new Event("Inspect", "A", "OBS", "Inspection observation test", result_obs_diceNum_6, false, null);

            EventManager.Instance.EveryEvents.Add(event_move);
            EventManager.Instance.EveryEvents.Add(event_Inspection);
            EventManager.Instance.EveryEvents.Add(event_work_A);
            EventManager.Instance.EveryEvents.Add(event_work_B);
            EventManager.Instance.EveryEvents.Add(event_global);
            EventManager.Instance.EveryEvents.Add(event_quest);
            EventManager.Instance.EveryEvents.Add(event_globalquest);
            EventManager.Instance.EveryEvents.Add(event_Inspection_despair);

            List<int> regionProbDataList = new List<int>();
            regionProbDataList.Add(10);
            regionProbDataList.Add(80);
            regionProbDataList.Add(10);
            Probability regionProbforMove = new Probability("Move", new List<int>(regionProbDataList));
            Probability regionProbforWork = new Probability("Work", regionProbDataList);
            Probability regionProbforInfo = new Probability("Info", regionProbDataList);
            Probability regionProbforBroker = new Probability("Broker", regionProbDataList);
            Probability regionProbforInspection = new Probability("Inspect", regionProbDataList);
            Probability regionProbforTaken = new Probability("Taken", regionProbDataList);
            Probability regionProbforEscape = new Probability("Escape", regionProbDataList);
            Probability regionProbforGlobal = new Probability("Global", regionProbDataList);
            Probability regionProbforQuest = new Probability("Quest", regionProbDataList);
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
            Probability statProbforMove = new Probability("Move", statProbDataList);
            Probability statProbforWork = new Probability("Work", statProbDataList);
            Probability statProbforInfo = new Probability("Info", statProbDataList);
            Probability statProbforBroker = new Probability("Broker", statProbDataList);
            Probability statProbforInspection = new Probability("Inspect", statProbDataList);
            Probability statProbforTaken = new Probability("Taken", statProbDataList);
            Probability statProbforEscape = new Probability("Escape", statProbDataList);
            Probability statProbforGlobal = new Probability("Global", statProbDataList);
            Probability statProbforQuest = new Probability("Quest", regionProbDataList);
            Probability[] statProbs = new Probability[9] {
                statProbforMove, statProbforWork, statProbforInfo, statProbforBroker, statProbforInspection,
                statProbforTaken, statProbforEscape, statProbforGlobal, statProbforQuest
            };

            ProbabilityManager.Instance.Init(regionProbs, statProbs);

            Inventory inven = new Inventory(30);
            character = new Character("Chris", new Stat(),
                                    cityA, 5, 3, 0, 5, 5, inven);

            // set character's start cureHP_Now_Onces.
            character.Inven.AddItem(cureHP_Now_Once);
            character.Inven.AddItem(cureBoth_RestEquip);
            character.Inven.AddItem(addSTR_Work_Once);
            character.Inven.AddItem(addSTR_Work_Equip);
            character.Inven.AddItem(addAGI_Move_Once);
            character.Inven.AddItem(delPoliceNearOnce);
            // cureHP_Now_Once use test
            //character.Inven.UseItem(character, 0, effect);

            // init time calendar.

            // init CityGraph and put polices in big cities.
            PieceManager.Instance.Init();
            Instance_NotifyEveryWeek();
            TimeTable.Instance.NotifyEveryWeek += Instance_NotifyEveryWeek;
        }

        private void Instance_NotifyEveryWeek()
        {
            // check current quest's end time and apply the result

            // activate global event
            Event globalEvent = EventManager.Instance.DoCommand("Global", character);

            // put pieces in one of random cities (police, information, quest)
            int distance = 0;
            // 2 polices
            PieceManager.Instance.Add(CityGraph.Instance.FindRand(), "POLICE");
            PieceManager.Instance.Add(CityGraph.Instance.FindRandCityByDistance(character.CurCity, distance), "POLICE");
            // 2 informations
            PieceManager.Instance.Add(CityGraph.Instance.FindRand(), "INFORM");
            PieceManager.Instance.Add(CityGraph.Instance.FindRandCityByDistance(character.CurCity, distance), "INFORM");
            // 1 quest
            Event selectedEvent = EventManager.Instance.Find("Quest", character.CurCity);
            PieceManager.Instance.AddQuest(CityGraph.Instance.FindRandCityByDistance(character.CurCity, distance), character, selectedEvent);
        }
    }
}