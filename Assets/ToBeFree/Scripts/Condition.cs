using System;

namespace ToBeFree
{
    public class Condition
    {
        private eSubjectType subjectType;
        private string comparisonOperator;
        private int amount;

        public Condition(eSubjectType subjectType, string comparisonOperator, int amount)
        {
            this.subjectType = subjectType;
            this.comparisonOperator = comparisonOperator;
            this.amount = amount;
        }

        public bool CheckCondition(Character character)
        {
            int left = -99;
            if (subjectType == eSubjectType.MONEY)
            {
                left = character.Stat.Money;
            }
            else if(subjectType == eSubjectType.ITEM)
            {
                return character.Inven.Exist(ItemManager.Instance.List[amount]);
            }
            else if(subjectType == eSubjectType.INFO)
            {
                left = character.Stat.InfoNum;
            }
            else if(subjectType == eSubjectType.NULL)
            {
                return true;
            }
            else if(subjectType == eSubjectType.MENTAL)
            {
                left = character.Stat.MENTAL;
            }
            return Compare(left, amount, comparisonOperator);
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