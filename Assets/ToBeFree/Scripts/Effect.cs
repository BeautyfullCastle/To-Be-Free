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
                        character.Inven.AddItem(null);
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
                        character.MENTAL += amount;
                    }
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