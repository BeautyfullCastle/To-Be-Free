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
        MENTAL,
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
        SUCCESS
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
        NULL,
        CANCEL
    }

    public class Effect
    {
        private eSubjectType subjectType;
        private eVerbType verbType;
        private eObjectType objectType;
        private int prevAmount;

        public delegate void SkipEventHandler(string eventType);
        public static event SkipEventHandler SkipEvent;

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
                            if(amount > 0 && character.IsFull)
                            {
                                return false;
                            }
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
                        // can't move after move event( in mongolia )
                        if (objectType == eObjectType.CANCEL)
                        {
                            character.CantMove = true;
                        }
                    }
                    if(verbType == eVerbType.IN)
                    {
                        if (objectType == eObjectType.DETENTION)
                        {
                            AbnormalConditionManager.Instance.Find("Detention").DeActivate(character);
                        }
                    }
                    
                        
                    break;
                    
                case eSubjectType.STAT:
                    if (verbType == eVerbType.ADD)
                    {
                        character.Stat.Set(objectType, amount);
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
                            Debug.LogError("detail type is not right.");
                        }

                        if (item == null)
                        {
                            Debug.LogError("item is null");
                            return false;
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
                    // later.
                    //if (verbType == eVerbType.SKIP)
                    //{
                    //    // fix. to make buff for skip event and make it succeed.
                    //    SkipEvent(EnumConvert<eObjectType>.ToString(objectType));
                    //    if (objectType == eObjectType.WORK)
                    //    {
                            
                    //    }
                    //    else if (objectType == eObjectType.MOVE) { }
                    //    else if (objectType == eObjectType.INFO) { }
                    //    // don't eat food when the time to eat
                    //    else if (objectType == eObjectType.FOOD) { }
                    //    // can't cure when rest event activated
                    //    else if (objectType == eObjectType.REST_CURE) { }
                    //    // skip entering the action
                    //    else if (objectType == eObjectType.ALL) { }
                    //}
                    if(verbType == eVerbType.SKIP)
                    {
                        if (objectType == eObjectType.FOOD)
                        {
                            character.IsFull = true;
                        }
                        // can't cure when rest event activated
                        else if (objectType == eObjectType.REST_CURE)
                        {
                            character.CantCure = true;
                        }
                    }
                    if (verbType == eVerbType.LOAD)
                    {
                        EventManager.Instance.ActivateEvent(EventManager.Instance.List[amount], character);
                    }
                    break;
                case eSubjectType.QUEST:
                    // load quest
                    if(verbType == eVerbType.LOAD)
                    {
                        int distance = 2;
                        City city = CityManager.Instance.FindRandCityByDistance(character.CurCity, distance);
                        Quest selectedQuest = QuestManager.Instance.List[amount];
                        QuestPiece questPiece = new QuestPiece(selectedQuest, character, city, eSubjectType.QUEST);
                        PieceManager.Instance.Add(questPiece);
                    }
                    if(verbType == eVerbType.SUCCESS)
                    {
                        Quest quest = QuestManager.Instance.List[amount];
                        QuestPiece piece = PieceManager.Instance.Find(quest);
                        QuestManager.Instance.ActivateResultEffects(quest.Event_.Result.Success.EffectAmounts, character);
                        PieceManager.Instance.Delete(piece);
                    }
                    break;
                case eSubjectType.ABNORMAL:
                    if (verbType == eVerbType.ADD)
                    {
                        AbnormalCondition abnormalCondition = AbnormalConditionManager.Instance.List[amount];
                        abnormalCondition.Activate(character);
                    }
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
                case eSubjectType.STAT:
                    if (verbType == eVerbType.ADD)
                    {
                        character.Stat.Restore(objectType);
                    }
                    break;
                case eSubjectType.EVENT:
                    if (verbType == eVerbType.SKIP)
                    {
                        if (objectType == eObjectType.FOOD)
                        {
                            character.IsFull = false;
                        }
                        else if (objectType == eObjectType.REST_CURE)
                        {
                            character.CantCure = false;
                        }
                    }
                    break;
                case eSubjectType.CHARACTER:
                    if (verbType == eVerbType.MOVE)
                    {
                        if (objectType == eObjectType.CANCEL)
                        {
                            character.CantMove = false;
                        }
                    }
                    break;
            }
        }
        
        public eSubjectType SubjectType { get { return subjectType; } }

        public eVerbType VerbType { get { return verbType; } }

        public eObjectType ObjectType { get { return objectType; } }
    }
}