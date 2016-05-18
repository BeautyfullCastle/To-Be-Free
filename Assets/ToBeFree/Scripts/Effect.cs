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
        private int index;
        private eType bigType;
        private string cureType;

        public string CureType { get; set; }

        public Effect(eType bigType, string cureType)
        {
            this.bigType = bigType;
            this.cureType = cureType;
        }

        public Effect(Effect effect) : this(effect.bigType, effect.cureType)
        {

        }

        public bool Activate(Character character, int amount)
        {
            switch (bigType)
            {
                case eType.FOOD:
                    character.FOOD += amount;
                    break;
                case eType.INVEN:
                    for (int i = 0; i < amount; ++i)
                    {
                        character.AddItem(null);
                    }
                    break;
                case eType.CURE:
                    if (cureType == eCureType.BOTH.ToString() || cureType == eCureType.HP.ToString())
                    {
                        Debug.Log("HP = " + character.HP + " + " + amount);
                        character.HP += amount;
                    }
                    if (cureType == eCureType.BOTH.ToString() || cureType == eCureType.MENTAL.ToString())
                    {
                        character.MENTAL += amount;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}