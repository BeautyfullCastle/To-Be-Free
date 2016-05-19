using System;
using System.Collections.Generic;

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
                    string itemType = item.Effect.BigType;
                    int itemAmount = item.Amount;
                    if (itemType == bigType && item.Effect.DetailType == detailType)
                    {
                        return Compare(itemAmount, amount, comparisonOperator);
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