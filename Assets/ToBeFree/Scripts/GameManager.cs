using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree
{
	public class GameManager : MonoSingleton<GameManager> {

        private Character character;
        private string command;

        protected GameManager() {} // can't use the constructor

        void Update()
        {
            // await for the event command
            if (Input.GetKeyDown(KeyCode.A))
            {
                command = "Work";
                Debug.Log("Command Work input");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                command = "Move";
                Debug.Log("Command Move input");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                command = "Quest";
                Debug.Log("Command Quest input");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                // if selected event is not move,
                // check polices in current city and activate police events.
                if (command != "Move")
                {
                    foreach (Piece piece in character.CurCity.PieceList)
                    {
                        if (piece is Police)
                        {
                            Police police = piece as Police;
                            character.Inspect();
                        }
                    }
                }

                // activate selected event
                if (command == "Work")
                {
                    character.Work();
                }
                if (command == "Move")
                {
                    //character.PrintMovableCity();
                    character.MoveTo(CityGraph.Instance.Find("B"));
                }
                if (command == "Quest")
                {
                    List<Piece> quests = character.CurCity.PieceList.FindAll(x => x is Quest);
                    if(quests.Count >= 2)
                    {
                        // have to handle this.
                    }
                    else if(quests == null || quests.Count == 0)
                    {
                        Debug.LogError("No Quest in this city.");
                    }
                    else
                    {
                        Quest quest = quests[0] as Quest;
                        if(EventManager.Instance.ActivateEvent(quest.CurEvent, character))
                        {
                            character.CurCity.PieceList.Remove(quests[0]);
                        }
                    }
                }


                TimeTable.Instance.DayIsGone();
            }
        }
		void Start() {

            Effect effect = new Effect("CURE", "HP");
            Item item = new Item("cure hp 1", effect,
                eStartTime.NOW, eDuration.ONCE,
                1, 10, 10);

            City cityA = new City("A", "Big", "North", new List<Item>() { item } );
			City cityB = new City("B", "Midium", "South", new List<Item>() { item });
			City cityC = new City("C", "Small", "East", new List<Item>() { item });
            City cityC2 = new City("C2", "Big", "East", new List<Item>() { item });
            City cityD = new City("D", "Midium", "East", new List<Item>() { item });

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


            ResultEffect[] successResultEffects = new ResultEffect[1] { new ResultEffect(0, effect, 1) };
            ResultEffect[] failureResultEffects = new ResultEffect[1] { new ResultEffect(0, effect, -1) };

            ResultScriptAndEffects success = new ResultScriptAndEffects("success", successResultEffects);
            ResultScriptAndEffects failure = new ResultScriptAndEffects("failure", failureResultEffects);

            Result result_strength = new Result("STR", success, failure);
            Result result_agility = new Result("AGI", success, failure);
            Result result_observation = new Result("OBS", success, failure);
            Result result_global = new Result(string.Empty, failure, null);
            Result result_quest = new Result(string.Empty, success, failure);

            Select select_quest = new Select("CURE", "HP", ">=", 1, "select cure hp > 1", result_quest);

            Event event_move = new Event("Move", "A", "AGI", "move strength test, A city", result_agility, false, null);
            Event event_Inspection = new Event("Inspection", "A", "OBS", "Inspection agility test, A city", result_observation, false, null);
            Event event_work_A = new Event("Work", "A", "STR", "police agility test, A city", result_strength, false, null);
            Event event_work_B = new Event("Work", "B", "STR", "police agility test, A city", result_strength, false, null);
            Event event_global = new Event("Global", "ALL", string.Empty, "global event", result_global, false, null);
            Event event_quest = new Event("Quest", "ALL", string.Empty, "quest", result_quest, true, new Select[1] { select_quest });
            Event event_globalquest = new Event("Quest", "ALL", string.Empty, "quest", result_quest, false, null);

            EventManager.Instance.EveryEvents.Add(event_move);
            EventManager.Instance.EveryEvents.Add(event_Inspection);
            EventManager.Instance.EveryEvents.Add(event_work_A);
            EventManager.Instance.EveryEvents.Add(event_work_B);
            EventManager.Instance.EveryEvents.Add(event_global);
            EventManager.Instance.EveryEvents.Add(event_quest);
            EventManager.Instance.EveryEvents.Add(event_globalquest);

            List<int> regionProbDataList = new List<int> ();
			regionProbDataList.Add (10);
			regionProbDataList.Add (80);
			regionProbDataList.Add (10);
			Probability regionProbforMove = new Probability("Move", new List<int>(regionProbDataList));
			Probability regionProbforWork = new Probability("Work", regionProbDataList);
			Probability regionProbforInfo = new Probability("Info", regionProbDataList);
			Probability regionProbforBroker = new Probability("Broker", regionProbDataList);
			Probability regionProbforInspection = new Probability("Inspection", regionProbDataList);
			Probability regionProbforTaken = new Probability("Taken", regionProbDataList);
			Probability regionProbforEscape = new Probability ("Escape", regionProbDataList);
            Probability regionProbforGlobal = new Probability("Global", regionProbDataList);
            Probability regionProbforQuest = new Probability("Quest", regionProbDataList);
            Probability[] regionProbs = new Probability[9] { 
				regionProbforWork, regionProbforMove, regionProbforInfo, regionProbforBroker, 
				regionProbforInspection, regionProbforTaken, regionProbforEscape, regionProbforGlobal, regionProbforQuest };

			List<int> statProbDataList = new List<int> ();
			statProbDataList.Add (50);
			statProbDataList.Add (10);
			statProbDataList.Add (10);
			statProbDataList.Add (10);
			statProbDataList.Add (10);
			statProbDataList.Add (10);
			Probability statProbforMove = new Probability ("Move", statProbDataList);
			Probability statProbforWork = new Probability ("Work", statProbDataList);
			Probability statProbforInfo = new Probability ("Info", statProbDataList);
			Probability statProbforBroker = new Probability ("Broker", statProbDataList);
			Probability statProbforInspection = new Probability ("Inspection", statProbDataList);
			Probability statProbforTaken = new Probability ("Taken", statProbDataList);
            Probability statProbforEscape = new Probability("Escape", statProbDataList);
            Probability statProbforGlobal = new Probability ("Global", statProbDataList);
            Probability statProbforQuest = new Probability("Quest", regionProbDataList);
            Probability[] statProbs = new Probability[9] {
                statProbforMove, statProbforWork, statProbforInfo, statProbforBroker, statProbforInspection, 
				statProbforTaken, statProbforEscape, statProbforGlobal, statProbforQuest
            };

			ProbabilityManager.Instance.Init(regionProbs, statProbs);

            Inventory inven = new Inventory(3);
            character = new Character("Chris", new Stat(),
                                    cityA, 5, 3, 0, 5, 5, inven);

            // set character's start items.
            character.Inven.AddItem(item);
            // item use test
            //character.Inven.UseItem(character, 0, effect);

            // init time calendar.

            // init CityGraph and put polices in big cities.
            CityGraph.Instance.Init();

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
            CityGraph.Instance.PutRandomPiece(new Police() as Piece, character.CurCity);
            CityGraph.Instance.PutRandomPieceByDistance(new Police() as Piece, character.CurCity, distance);
            // 2 informations
            CityGraph.Instance.PutRandomPiece(new Information() as Piece, character.CurCity);
            CityGraph.Instance.PutRandomPieceByDistance(new Information() as Piece, character.CurCity, distance);
            // 1 quest
            Event selectedEvent = EventManager.Instance.Find("Quest", character.CurCity);
            Quest quest = new Quest(selectedEvent, character);

            if (selectedEvent.ActionType == "Quest")
            {
                quest.City = CityGraph.Instance.PutRandomPieceByDistance(quest, character.CurCity, distance);
            }
        }
    }
}
