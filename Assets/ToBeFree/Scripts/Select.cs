using System;
using UnityEngine;

namespace ToBeFree
{
    public class Select
    {
        private eSubjectType subjectType;
        private eObjectType objectType;
        private string comparisonOperator;
        private int amount;
        private string script;
        private Result result;

        public Select(eSubjectType subjectType, eObjectType objectType, string comparisonOperator, int amount, string script, Result result)
        {
            this.subjectType = subjectType;
            this.objectType = objectType;
            this.comparisonOperator = comparisonOperator;
            this.amount = amount;
            this.script = script;
            this.result = result;
        }

        public bool CheckCondition(Character character)
        {
            // have to fix here.
            if (subjectType == eSubjectType.CHARACTER)
            {
                if (objectType == eObjectType.HP)
                {
                    Item item = character.Inven.FindItemByType(subjectType, eVerbType.ADD, objectType);
                    if (item == null)
                    {
                        Debug.Log(subjectType + " or " + objectType + " is not exist.");
                        return false;
                    }
                    eSubjectType itemType = item.Buff.Effect.SubjectType;
                    int itemAmount = item.Buff.Amount;
                    if (itemType == subjectType && item.Buff.Effect.ObjectType == objectType)
                    {
                        bool isExist = Compare(itemAmount, amount, comparisonOperator);
                        if (isExist)
                        {
                            character.Inven.Delete(item, character);
                        }
                        return isExist;
                    }
                }
            }

            throw new Exception(subjectType + " or " + objectType + " is not right.");
        }

        private bool Compare(int left, int right, string comparisonOp)
        {
            if (comparisonOp == "<")
            {
                return left < right;
            }
            if (comparisonOp == "<=")
            {
                return left <= right;
            }
            if (comparisonOp == "==")
            {
                return left == right;
            }
            if (comparisonOp == ">=")
            {
                return left >= right;
            }
            if (comparisonOp == ">")
            {
                return left > right;
            }

            throw new Exception(comparisonOp + " is not right operator.");
        }

        public Result Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }
}