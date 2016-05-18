using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ToBeFree
{
	public class GameManager : MonoSingleton<GameManager> {

		protected GameManager() {} // can't use the constructor

		void Start() {

            Effect effect = new Effect(eType.CURE, "HP");
            Item item = new Item("cure", effect,
                eStartTime.NOW, eDuration.ONCE,
                1, 1, 10);

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
            Event event_move = new Event("Move", "A", "AGI", "move strength test, A city", result_agility, false, null);
            Event event_Inspection = new Event("Inspection", "A", "OBS", "Inspection agility test, A city", result_observation, false, null);
            Event event_work = new Event("Work", "A", "STR", "police agility test, A city", result_strength, false, null);
            EventManager.Instance.EveryEvents.Add(event_move);
            EventManager.Instance.EveryEvents.Add(event_Inspection);
            EventManager.Instance.EveryEvents.Add(event_work);

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
			Probability[] regionProbs = new Probability[7] { 
				regionProbforWork, regionProbforMove, regionProbforInfo, regionProbforBroker, 
				regionProbforInspection, regionProbforTaken, regionProbforEscape };

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
			Probability statProbforEscape = new Probability ("Escape", statProbDataList);
			Probability[] statProbs = new Probability[7] {
                statProbforMove,
                statProbforWork,
				statProbforInfo,
				statProbforBroker, 
				statProbforInspection, 
				statProbforTaken, 
				statProbforEscape };

			ProbabilityManager.Instance.Init(regionProbs, statProbs);

            Character character = new Character("Chris", new Stat(),
                                    cityA, 5, 3, 0, 5, 5, new List<Item>());

            // set character's start items.
            character.AddItem(item);
            // item use test
            character.UseItem(character, 0, effect);

            // init time calendar.

            // init CityGraph and put polices in big cities.
            CityGraph.Instance.Init();

            // check current global event and quest's end time and apply the result

            // activate global event

            // put pieces in one of random cities (police, broker, information, quest)
            CityGraph.Instance.PutRandomPolicesPerWeek(character.CurCity);
            
            
            // await for the event command
            string command = null;

            command = "Work";

            // if selected event is not move,
            // check polices in current city and activate police events.
            if(command != "Move")
            {
                foreach(Piece piece in character.CurCity.PieceList)
                {
                    if(piece is Police)
                    {
                        Police police = piece as Police;
                        character.Inspect();
                    }
                }
            }

            // activate selected event
            if (command == "Move")
            {
                character.PrintMovableCity();
                character.MoveTo(cityB);
            }
		}
	}
}
