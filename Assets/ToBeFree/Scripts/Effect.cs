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

                            piece.City = CityGraph.Instance.FindRand();
                        }
                        if (detailType == "RAND TO CLOSE")
                        {
                            Piece piece = PieceManager.Instance.GetRand(bigType);
                            
                            piece.City = CityGraph.Instance.FindRandCityByDistance(character.CurCity, amount);
                        }
                        if (detailType == "FAR TO CLOSE")
                        {
                            Piece piece = PieceManager.Instance.GetLast(bigType);

                            piece.City = CityGraph.Instance.FindRandCityByDistance(character.CurCity, amount);
                        }
                        if (detailType == "CLOSE TO FAR")
                        {
                            Piece piece = PieceManager.Instance.GetFirst(bigType);

                            System.Random r = new System.Random();
                            int randDistance = r.Next(piece.City.Distance, piece.City.Distance + amount);
                            piece.City = CityGraph.Instance.FindRandCityByDistance(character.CurCity, randDistance);
                        }
                    }
                    if(middleType == "DEL")
                    {
                        Piece piece = null;
                        if (detailType == "RAND")
                        {
                            piece = PieceManager.Instance.GetRand(bigType);
                        }
                        if(detailType == "FAR")
                        {
                            piece = PieceManager.Instance.GetLast(bigType);
                        }
                        if(detailType == "CLOSE") {
                            piece = PieceManager.Instance.GetFirst(bigType);
                        }
                        PieceManager.Instance.Delete(piece);
                    }
                    if(middleType == "ADD")
                    {
                        if(detailType == "RAND")
                        {
                            City city = CityGraph.Instance.FindRand();
                            Piece piece = PieceManager.Instance.Add(city, bigType);
                        }
                        if(detailType == "CLOSE")
                        {
                            City city = CityGraph.Instance.FindRandCityByDistance(character.CurCity, amount);
                            Piece piece = PieceManager.Instance.Add(city, bigType);
                        }
                    }
                    // for infrom only
                    if(middleType == "CHARACTER")
                    {
                        if(detailType == "ADD")
                        {
                            character.CurInfoNum++;
                        }
                        if(detailType == "DEL")
                        {
                            character.CurInfoNum--;
                        }
                    }
                    break;
                case "ITEM":
                    if(middleType == "ADD")
                    {
                        Item item = null;
                        if (detailType == "ALL")
                        {
                            item = ItemManager.Instance.GetRand();
                        }
                        else if (detailType == "TAG")
                        {
                            item = ItemManager.Instance.GetTagRand(amount);
                        }
                        else if (detailType == "INDEX")
                        {
                            // Item item = invenManager.get(amount);
                            ItemManager.Instance.GetByIndex(amount);
                        }
                        else if (detailType == "ALL SELECT") { }
                        else if (detailType == "TAG SELECT") { }
                        else
                        {
                            throw new System.Exception("detail type is not right.");
                        }

                        if(item==null)
                        {
                            throw new System.Exception("item is null");
                        }
                        character.Inven.AddItem(item);
                    }
                    if(middleType == "DEL")
                    {
                        Item item = null;
                        if (detailType == "ALL")
                        {
                            item = character.Inven.GetRand();
                        }
                        else if (detailType == "TAG")
                        {
                            item = character.Inven.GetTagRand(amount);
                        }
                        else if (detailType == "INDEX")
                        {
                            item = ItemManager.Instance.GetByIndex(amount);
                        }
                        else if (detailType == "SELECT") { }
                        else
                        {
                            throw new System.Exception("detail type is not right.");
                        }

                        if (item == null)
                        {
                            throw new System.Exception("item is null");
                        }
                        character.Inven.DeleteItem(item);
                    }
                    break;
                case "MONEY":
                    if (middleType == "SPECIFIC")
                    {
                        character.CurMoney += amount;
                    }
                    // can add more : RAND ?
                    if (middleType == "RAND 3")
                    {
                        int middleMoney = 3;
                        System.Random r = new System.Random();
                        int money = r.Next(-middleMoney, middleMoney) + amount;
                        character.CurMoney += money;
                    }
                    break;
                case "CHARACTER":
                    if(middleType == "MOVE")
                    {
                        if(detailType == "CLOSE")
                        {
                            character.CurCity = CityGraph.Instance.FindRandCityByDistance(character.CurCity, amount);
                        }
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