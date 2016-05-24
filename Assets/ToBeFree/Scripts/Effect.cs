using UnityEngine;
using System.Collections;

namespace ToBeFree
{
    public enum eType
    {
        FOOD, INVEN, CURE, STAT, REROLL, DICE, DICESPOT, INFO, POLICE, CHARACTER, ITEM, MONEY, ACTINGPOWER
    }

    public enum eCureType
    {
        HP, MENTAL, BOTH
    }

    public enum eCommandType
    {
        MOVE, REST, SHOP, POLICE, INFO, BROKER, ESCAPE
    }

    public enum eDistanceType
    {
        RANDOM, SELECT, FAR_CLOSE, CLOSE_FAR
    }

    public enum eChangeType
    {
        MOVE, ADD, DELETE
    }

    public enum eSelectType
    {
        SELECT, RANDOM
    }

    public class Effect
    {
        private string bigType;
        private string detailType;
        private string middleType;

        public Effect(string bigType, string detailType)
        {
            this.bigType = bigType;
            this.detailType = detailType;
        }

        public Effect(Effect effect) : this(effect.bigType, effect.detailType)
        {

        }

        public bool Activate(Character character, int amount)
        {
            switch (bigType)
            {
                case "FOOD":
                    character.FOOD += amount;
                    break;
                case "INVEN":
                    for (int i = 0; i < amount; ++i)
                    {
                        character.Inven.AddSlot();
                    }
                    break;
                case "CURE":
                    if (detailType == eCureType.BOTH.ToString() || detailType == eCureType.HP.ToString())
                    {
                        Debug.Log("HP = " + character.HP + " + " + amount);
                        character.HP += amount;
                    }
                    if (detailType == eCureType.BOTH.ToString() || detailType == eCureType.MENTAL.ToString())
                    {
                        Debug.Log("MENTAL = " + character.MENTAL + " + " + amount);
                        character.MENTAL += amount;
                    }
                    break;
                case "STAT":
                    if(detailType == "STR")
                    {
                        character.Stat.Strength++;
                    }
                    //...
                    break;
                case "INFORM":
                case "POLICE":
                    if (middleType == "MOVE")
                    {
                        if (detailType == "RAND TO RAND")
                        {
                            Piece piece = PieceManager.Instance.GetRand(bigType);
                            PieceManager.Instance.Delete(piece, bigType);
                            piece.City.PieceList.Remove(piece);
                            CityGraph.Instance.PutRandomPiece(piece, character.CurCity);
                        }
                        if (detailType == "RAND TO CLOSE")
                        {
                            Piece piece = PieceManager.Instance.GetRand(bigType);
                            PieceManager.Instance.Delete(piece, bigType);
                            piece.City.PieceList.Remove(piece);
                            CityGraph.Instance.PutRandomPieceByDistance(piece, character.CurCity, 0);
                        }
                        if (detailType == "FAR TO CLOSE")
                        {
                            Piece piece = PieceManager.Instance.GetLast(bigType);
                            PieceManager.Instance.Delete(piece, bigType);
                            piece.City.PieceList.Remove(piece);
                            CityGraph.Instance.PutRandomPieceByDistance(piece, character.CurCity, 0);
                        }
                        if (detailType == "CLOSE TO FAR")
                        {
                            Piece piece = PieceManager.Instance.GetFirst(bigType);
                            PieceManager.Instance.Delete(piece, bigType);
                            piece.City.PieceList.Remove(piece);

                            System.Random r = new System.Random();
                            int randDistance = r.Next(piece.City.Distance, piece.City.Distance + amount);
                            CityGraph.Instance.PutRandomPieceByDistance(piece, character.CurCity, randDistance);
                        }
                    }
                    if(middleType == "DEL")
                    {
                        if(detailType == "RAND")
                        {
                            
                        }
                        if(detailType == "FAR")
                        {

                        }
                        if(detailType == "CLOSE") {

                        }
                    }
                    if(middleType == "ADD")
                    {
                        if(detailType == "RAND")
                        {

                        }
                        if(detailType == "CLOSE")
                        {

                        }
                    }
                    // for infrom only
                    if(middleType == "CHARACTER")
                    {
                        if(detailType == "ADD")
                        {

                        }
                        if(detailType == "DEL")
                        {

                        }
                    }
                    break;
                case "ITEM":
                    if(middleType == "ADD")
                    {
                        if (detailType == "ALL")
                        {
                            // Item item = inventorymanager.getrand(detailType);
                            // character.Inven.AddItem(item);
                        }
                        if (detailType == "TAG")
                        {
                        }
                        if (detailType == "INDEX") {
                            // Item item = invenManager.get(amount);
                        }
                        if (detailType == "ALL SELECT") { }
                        if (detailType == "TAG SELECT") { }
                    }
                    if(middleType == "DEL")
                    {

                    }
                    break;
                case "MONEY":
                    if (middleType == "SPECIFIC")
                    {
                    }
                    if (middleType == "RAND ?")
                    {
                    }
                    break;
                case "PLAYER":
                    if(middleType == "MOVE")
                    {

                    }
                    break;
                case "ACTION POWER":

                    break;
                       

                default:
                    break;
            }
            return false;
        }

        public string DetailType { get { return detailType; } }

        public string BigType
        {
            get
            {
                return bigType;
            }
        }

        public string DetailType1
        {
            get
            {
                return detailType;
            }

            set
            {
                detailType = value;
            }
        }
    }
}