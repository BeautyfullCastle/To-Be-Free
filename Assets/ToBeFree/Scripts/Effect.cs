using System;
using UnityEngine;

namespace ToBeFree
{
    public enum eSubjectType
    {
        STAT, DICE,
        INFO, POLICE, CHARACTER,
        ITEM, MONEY, FOOD,
        ACTINGPOWER, ABNORMAL,
        COMMAND,
        EVENT, QUEST,
        ROOT,
        BROKER,
        NULL
    }

    public enum eVerbType
    {
        NONE,
        ADD, DEL, MOVE,
        DEACTIVE,
        SKIP, LOAD, OPEN,
        REROLL,
        IN,
        SET,
        NULL,
        CANCEL
    }

    // ¸ñÀû¾î
    public enum eObjectType
    {
        NONE,
        HP, MENTAL, HP_MENTAL,
        FAR_CLOSE, CLOSE_FAR, RAND_RAND, RAND_CLOSE, CLOSE_CLOSE,
        CLOSE, FAR, RAND, 
        ALL, TAG, INDEX, SELECT, SELECT_ALL, SELECT_TAG,
        WORK, MOVE, REST, REST_CURE, SPECIAL, INFO, FOOD, SHOP,
        STRENGTH, AGILITY, OBSERVATION, BARGAIN, PATIENCE, LUCK,
        SOUTHEAST_ASIA, MONGOLIA,
        SPECIFIC, RAND_3,
        SUCCESSNUM,
        INVEN,
        DETENTION,
        NULL
    }

    public class Effect
    {
        private eSubjectType subjectType;
        private eVerbType verbType;
        private eObjectType objectType;
        private int prevAmount;

        public Effect(eSubjectType subjectType, eVerbType verbType = eVerbType.NONE, eObjectType objectType = eObjectType.NONE)
        {
            this.subjectType = subjectType;
            this.verbType = verbType;
            this.objectType = objectType;
        }

        public Effect(Effect effect) : this(effect.subjectType, effect.verbType, effect.objectType)
        {
        }
        
        public override string ToString()
        {
            return EnumConvert<eSubjectType>.ToString(subjectType)
                + " " + EnumConvert<eVerbType>.ToString(verbType)
                + " " + EnumConvert<eObjectType>.ToString(ObjectType);
        }
        public bool Activate(Character character, int amount)
        {

            switch (subjectType)
            {
                case eSubjectType.CHARACTER:
                    if (verbType == eVerbType.ADD)
                    {
                        if (objectType == eObjectType.HP_MENTAL || objectType == eObjectType.HP)
                        {
                            Debug.Log("Cure HP");
                            character.Stat.HP += amount;
                        }
                        if (objectType == eObjectType.HP_MENTAL || objectType == eObjectType.MENTAL)
                        {
                            Debug.Log("Cure Mental");
                            character.Stat.MENTAL += amount;
                        }
                        if (objectType == eObjectType.INFO)
                        {
                            character.Stat.InfoNum++;
                        }
                        if (objectType == eObjectType.FOOD)
                        {
                            character.Stat.FOOD += amount;
                        }
                        if (objectType == eObjectType.INVEN)
                        {
                            for (int i = 0; i < amount; ++i)
                            {
                                character.Inven.AddSlot();
                            }
                        }
                    }
                    if (verbType == eVerbType.DEL)
                    {
                        if (objectType == eObjectType.INFO)
                        {
                            character.Stat.InfoNum--;
                        }
                    }
                    if (verbType == eVerbType.MOVE)
                    {
                        if (objectType == eObjectType.CLOSE)
                        {
                            character.CurCity = CityManager.Instance.FindRandCityByDistance(character.CurCity, amount);
                        }
                    }
                    if(verbType == eVerbType.IN)
                    {
                        if (objectType == eObjectType.DETENTION) { }
                    }
                    break;
                    
                case eSubjectType.STAT:
                    if (verbType == eVerbType.ADD)
                    {
                        if (objectType == eObjectType.STRENGTH || objectType == eObjectType.ALL)
                        {
                            character.Stat.Strength += amount;
                            Debug.Log("effect activate strength : " + character.Stat.Strength);
                        }
                        if (objectType == eObjectType.AGILITY || objectType == eObjectType.ALL)
                        {
                            character.Stat.Agility += amount;
                            Debug.Log("effect activate agility : " + character.Stat.Agility);
                        }
                        if (objectType == eObjectType.OBSERVATION || objectType == eObjectType.ALL)
                        {
                            character.Stat.Observation += amount;
                            Debug.Log("effect activate observation : " + character.Stat.Observation);
                        }
                        if (objectType == eObjectType.BARGAIN || objectType == eObjectType.ALL)
                        {
                            character.Stat.Bargain += amount;
                            Debug.Log("effect activate bargain : " + character.Stat.Bargain);
                        }
                        if (objectType == eObjectType.PATIENCE || objectType == eObjectType.ALL)
                        {
                            character.Stat.Patience += amount;
                            Debug.Log("effect activate patience : " + character.Stat.Patience);
                        }
                        if (objectType == eObjectType.LUCK || objectType == eObjectType.ALL)
                        {
                            character.Stat.Luck += amount;
                            Debug.Log("effect activate luck : " + character.Stat.Luck);
                        }
                    }
                    break;
                    
                case eSubjectType.INFO:
                case eSubjectType.POLICE:
                case eSubjectType.BROKER:
                    if (verbType == eVerbType.MOVE)
                    {
                        if (objectType == eObjectType.RAND_RAND)
                        {
                            Piece piece = PieceManager.Instance.FindRand(subjectType);

                            piece.MoveCity(CityManager.Instance.FindRand());
                        }
                        if (objectType == eObjectType.RAND_CLOSE)
                        {
                            Piece piece = PieceManager.Instance.FindRand(subjectType);

                            piece.MoveCity(CityManager.Instance.FindRandCityByDistance(character.CurCity, amount));
                        }
                        if (objectType == eObjectType.FAR_CLOSE)
                        {
                            Piece piece = PieceManager.Instance.GetLast(subjectType);

                            piece.MoveCity(CityManager.Instance.FindRandCityByDistance(character.CurCity, amount));
                        }
                        if (objectType == eObjectType.CLOSE_FAR)
                        {
                            Piece piece = PieceManager.Instance.GetFirst(subjectType);

                            System.Random r = new System.Random();
                            int randDistance = r.Next(piece.City.Distance, piece.City.Distance + amount);
                            piece.MoveCity(CityManager.Instance.FindRandCityByDistance(character.CurCity, randDistance));
                        }
                    }
                    if (verbType == eVerbType.DEL)
                    {
                        Piece piece = null;
                        if (objectType == eObjectType.RAND)
                        {
                            piece = PieceManager.Instance.FindRand(subjectType);
                        }
                        if (objectType == eObjectType.FAR)
                        {
                            piece = PieceManager.Instance.GetLast(subjectType);
                        }
                        if (objectType == eObjectType.CLOSE)
                        {
                            piece = PieceManager.Instance.GetFirst(subjectType);
                        }
                        PieceManager.Instance.Delete(piece);
                    }
                    if (verbType == eVerbType.ADD)
                    {
                        if (objectType == eObjectType.RAND)
                        {
                            City city = CityManager.Instance.FindRand();
                            PieceManager.Instance.Add(new Piece(city, subjectType));
                        }
                        if (objectType == eObjectType.CLOSE)
                        {
                            City city = CityManager.Instance.FindRandCityByDistance(character.CurCity, amount);
                            PieceManager.Instance.Add(new Piece(city, subjectType));
                        }
                    }
                    break;
                    
                case eSubjectType.ITEM:
                    if (verbType == eVerbType.ADD)
                    {
                        Item item = null;
                        if (objectType == eObjectType.ALL)
                        {
                            item = ItemManager.Instance.GetRand();
                        }
                        else if (objectType == eObjectType.TAG)
                        {
                            item = ItemManager.Instance.GetTagRand(amount);
                        }
                        else if (objectType == eObjectType.INDEX)
                        {
                            // Item item = invenManager.get(amount);
                            ItemManager.Instance.GetByIndex(amount);
                        }
                        else if (objectType == eObjectType.SELECT_ALL) { }
                        else if (objectType == eObjectType.SELECT_TAG) { }
                        else
                        {
                            throw new System.Exception("detail type is not right.");
                        }

                        if (item == null)
                        {
                            throw new System.Exception("item is null");
                        }
                        character.Inven.AddItem(item, character);
                    }
                    if (verbType == eVerbType.DEL)
                    {
                        Item item = null;
                        if (objectType == eObjectType.ALL)
                        {
                            item = character.Inven.GetRand();
                        }
                        else if (objectType == eObjectType.TAG)
                        {
                            item = character.Inven.GetTagRand(amount);
                        }
                        else if (objectType == eObjectType.INDEX)
                        {
                            item = ItemManager.Instance.GetByIndex(amount);
                        }
                        else if (objectType == eObjectType.SELECT) { }
                        else
                        {
                            throw new System.Exception("detail type is not right.");
                        }

                        if (item == null)
                        {
                            throw new System.Exception("item is null");
                        }
                        character.Inven.Delete(item, character);
                    }
                    break;
                    
                case eSubjectType.MONEY:
                    if (verbType == eVerbType.ADD)
                    {
                        if (objectType == eObjectType.SPECIFIC)
                        {
                            character.Stat.Money += amount;
                        }
                        // can add more : RAND ?
                        if (objectType == eObjectType.RAND_3)
                        {
                            int middleMoney = 3;
                            System.Random r = new System.Random();
                            int money = r.Next(-middleMoney, middleMoney) + amount;
                            character.Stat.Money += money;
                        }
                    }
                    break;
                    
                case eSubjectType.COMMAND:
                    if (verbType == eVerbType.DEACTIVE)
                    {
                        if (objectType == eObjectType.WORK) { }
                        else if (objectType == eObjectType.MOVE) { }
                        else if (objectType == eObjectType.REST) { }
                        else if (objectType == eObjectType.SPECIAL) { } // other commands.
                    }
                    break;
                    
                case eSubjectType.DICE:
                    if (verbType == eVerbType.SET)
                    {
                        if (objectType == eObjectType.SUCCESSNUM)
                        {
                            if (!(amount == 4 || amount == 6))
                            {
                                throw new System.Exception("Input Dice success num is not 4 or 6.");
                            }
                            Debug.Log("Effect " + subjectType + " " + verbType + " " + amount + " activated.");
                            prevAmount = DiceTester.Instance.MinSuccessNum;
                            DiceTester.Instance.MinSuccessNum = amount;
                        }
                    }
                    break;
                    
                case eSubjectType.EVENT:
                    if (verbType == eVerbType.SKIP)
                    {
                        if (objectType == eObjectType.WORK) { }
                        else if (objectType == eObjectType.MOVE) { }
                        else if (objectType == eObjectType.INFO) { }
                        // don't eat food when the time to eat
                        else if (objectType == eObjectType.FOOD) { }
                        // can't cure when rest event activated
                        else if (objectType == eObjectType.REST_CURE) { }
                        // skip entering the action
                        else if (objectType == eObjectType.ALL) { }
                    }
                    else if(verbType == eVerbType.LOAD) { }
                    break;
                case eSubjectType.QUEST:
                    // load quest
                    if(verbType == eVerbType.LOAD)
                    {

                    }
                    break;
                case eSubjectType.ABNORMAL:
                    if (verbType == eVerbType.ADD) { }
                    break;
                case eSubjectType.ROOT:
                    if(verbType == eVerbType.OPEN)
                    {
                        if (objectType == eObjectType.MONGOLIA) { }
                        if (objectType == eObjectType.SOUTHEAST_ASIA) { }
                    }
                    break;

                default:
                    break;
            }
            return false;
        }

        public void Deactivate(Character character)
        {
            switch (subjectType)
            {
                case eSubjectType.DICE:
                    if (objectType == eObjectType.SUCCESSNUM)
                    {
                        Debug.Log("Effect " + subjectType + " " + verbType + " " + prevAmount + " deactivated.");
                        DiceTester.Instance.MinSuccessNum = prevAmount;
                    }
                    break;
            }
        }
        
        static public eSubjectType ToSubjectType(string subjectType)
        {
            return (eSubjectType)Enum.Parse(typeof(eSubjectType), subjectType);
        }

        static public eVerbType ToVerbType(string verbType)
        {
            switch(verbType)
            {
                case "ADD":
                    return eVerbType.ADD;
                case "CANCEL":
                    return eVerbType.CANCEL;
                case "DEACTIVE":
                    return eVerbType.DEACTIVE;
                case "DEL":
                    return eVerbType.DEL;
                case "IN":
                    return eVerbType.IN;
                case "LOAD":
                    return eVerbType.LOAD;
                case "MOVE":
                    return eVerbType.MOVE;
                case "NONE":
                    return eVerbType.NONE;
                case "OPEN":
                    return eVerbType.OPEN;
                case "REROLL":
                    return eVerbType.REROLL;
                case "SET":
                    return eVerbType.SET;
                case "SKIP":
                    return eVerbType.SKIP;
            }

            return eVerbType.NULL;
        }

        static public eObjectType ToObjectType(string objectType)
        {
            switch(objectType)
            {
                case "AGILITY":
                    return eObjectType.AGILITY;
                case "ALL":
                    return eObjectType.ALL;
                case "BARGAIN":
                    return eObjectType.BARGAIN;
                case "CLOSE":
                    return eObjectType.CLOSE;
                case "CLOSE_CLOSE":
                    return eObjectType.CLOSE_CLOSE;
                case "CLOSE_FAR":
                    return eObjectType.CLOSE_FAR;
                case "DETENTION":
                    return eObjectType.DETENTION;
                case "FAR":
                    return eObjectType.FAR;
                case "FAR_CLOSE":
                    return eObjectType.FAR_CLOSE;
                case "FOOD":
                    return eObjectType.FOOD;
                case "HP":
                    return eObjectType.HP;
                case "HP_MENTAL":
                    return eObjectType.HP_MENTAL;
                case "INDEX":
                    return eObjectType.INDEX;
                case "INFO":
                    return eObjectType.INFO;
                case "INVEN":
                    return eObjectType.INVEN;
                case "LUCK":
                    return eObjectType.LUCK;
                case "MENTAL":
                    return eObjectType.MENTAL;
                case "MONGOLIA":
                    return eObjectType.MONGOLIA;
                case "MOVE":
                    return eObjectType.MOVE;
                case "NONE":
                    return eObjectType.NONE;
                case "OBSERVATION":
                    return eObjectType.OBSERVATION;
                case "PATIENCE":
                    return eObjectType.PATIENCE;
                case "RAND":
                    return eObjectType.RAND;
                case "RAND_3":
                    return eObjectType.RAND_3;
                case "RAND_CLOSE":
                    return eObjectType.RAND_CLOSE;
                case "RAND_RAND":
                    return eObjectType.RAND_RAND;
                case "REST":
                    return eObjectType.REST;
                case "REST_CURE":
                    return eObjectType.REST_CURE;
                case "SELECT":
                    return eObjectType.SELECT;
                case "SELECT_ALL":
                    return eObjectType.SELECT_ALL;
                case "SELECT_TAG":
                    return eObjectType.SELECT_TAG;
                case "SHOP":
                    return eObjectType.SHOP;
                case "SOUTHEAST_ASIA":
                    return eObjectType.SOUTHEAST_ASIA;
                case "SPECIAL":
                    return eObjectType.SPECIAL;
                case "SPECIFIC":
                    return eObjectType.SPECIFIC;
                case "STRENGTH":
                    return eObjectType.STRENGTH;
                case "SUCCESSNUM":
                    return eObjectType.SUCCESSNUM;
                case "TAG":
                    return eObjectType.TAG;
                case "WORK":
                    return eObjectType.WORK;
            }
            return eObjectType.NULL;
        }


        public eSubjectType SubjectType { get { return subjectType; } }

        public eVerbType VerbType { get { return verbType; } }

        public eObjectType ObjectType { get { return objectType; } }
    }
}