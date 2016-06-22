using System;
using UnityEngine;

namespace ToBeFree
{
    public enum eSelectLinkType
    {
        EVENT, RESULT
    }

    public class Select
    {
        private eSubjectType subjectType;
        private eObjectType objectType;
        private string comparisonOperator;
        private int compareAmount;
        private string script;
        private eSelectLinkType linkType;
        private int linkIndex;

        public Select(eSubjectType subjectType, eObjectType objectType, string comparisonOperator, int compareAmount, string script, eSelectLinkType linkType, int linkIndex)
        {
            this.subjectType = subjectType;
            this.objectType = objectType;
            this.comparisonOperator = comparisonOperator;
            this.compareAmount = compareAmount;
            this.script = script;
            this.linkType = linkType;
            this.linkIndex = linkIndex;
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
                        bool isExist = Compare(itemAmount, compareAmount, comparisonOperator);
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
                if (linkType == eSelectLinkType.RESULT)
                {
                    return ResultManager.Instance.List[linkIndex];
                }
                return null;
            }
        }

        public Event Event
        {
            get
            {
                if(linkType == eSelectLinkType.EVENT)
                {
                    return EventManager.Instance.List[linkIndex];
                }
                return null;
            }
        }
    }
}