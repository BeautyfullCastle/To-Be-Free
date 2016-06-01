using System;

namespace ToBeFree
{
    public class Condition
    {
        private string bigType;
        private string middleType;
        private string detailType;
        private string comparisonOperator;
        private int amount;

        public Condition(string bigType, string middleType, string detailType, string comparisonOperator, int amount)
        {
            this.bigType = bigType;
            this.middleType = middleType;
            this.detailType = detailType;
            this.comparisonOperator = comparisonOperator;
            this.amount = amount;
        }

        public bool CheckCondition(Character character)
        {
            if (string.IsNullOrEmpty(detailType))
            {
                if (string.IsNullOrEmpty(middleType))
                {
                    // only big
                    if (bigType == "MENTAL")
                    {
                        return Compare(character.MENTAL, amount, "<=");
                    }
                }
                else
                {
                    // big and middle
                }
            }
            else
            {
                // big and middle and detail
            }

            throw new Exception(bigType + " or " + middleType + " or " + detailType + " is not right.");
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
    }
}