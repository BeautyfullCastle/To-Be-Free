using System;

namespace ToBeFree
{
    public class Condition
    {
        private string spawnType;
        private string comparisonOperator;
        private int amount;

        public Condition(string spawnType, string comparisonOperator, int amount)
        {
            this.spawnType = spawnType;
            this.comparisonOperator = comparisonOperator;
            this.amount = amount;
        }

        public bool CheckCondition(Character character)
        {
            //if (string.IsNullOrEmpty(detailType))
            //{
            //    if (string.IsNullOrEmpty(middleType))
            //    {
                    // only big
                    if (spawnType == "MENTAL")
                    {
                        return Compare(character.Stat.MENTAL, amount, "<=");
                    }
            //    }
            //    else
            //    {
            //        // big and middle
            //    }
            //}
            //else
            //{
            //    // big and middle and detail
            //}

            throw new Exception(spawnType);// + " or " + middleType + " or " + detailType + " is not right.");
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