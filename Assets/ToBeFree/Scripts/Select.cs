using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Select
    {
        private string bigType;
        private string detailType;
        private string comparisonOperator;
        private int amount;
        private string script;
        private Result result;
        
        public Select(string bigType, string detailType, string comparisonOperator, int amount, string script, Result result)
        {
            this.bigType = bigType;
            this.detailType = detailType;
            this.comparisonOperator = comparisonOperator;
            this.amount = amount;
            this.script = script;
            this.result = result;
        }

        public bool CheckCondition(Character character)
        {
            if(bigType == "CURE")
            {
                if(detailType == "HP")
                {
                    Item item = character.Inven.FindItemByType(bigType, detailType);
                    if(item == null)
                    {
                        Debug.Log(bigType + " or " + detailType + " is not exist.");
                        return false;
                    }
                    string itemType = item.Effect.BigType;
                    int itemAmount = item.Amount;
                    if (itemType == bigType && item.Effect.DetailType == detailType)
                    {
                        bool isExist = Compare(itemAmount, amount, comparisonOperator);
                        if(isExist)
                        {
                            character.Inven.DeleteItem(item);
                        }
                        return isExist;
                    }
                }
            }

            throw new Exception(bigType + " or " + detailType + " is not right.");
        }

        private bool Compare(int left, int right, string comparisonOp)
        {
            if(comparisonOp == "<")
            {
                return left < right;
            }
            if(comparisonOp == "<=")
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
            if(comparisonOp == ">")
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