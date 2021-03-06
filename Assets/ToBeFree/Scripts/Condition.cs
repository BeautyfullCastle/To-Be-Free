﻿using System;

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

		public bool CheckCondition(Character character, int PastDays)
		{
			int left = -99;
			if (subjectType == eSubjectType.DDAY)
			{
				left = PastDays;
				return Compare(left, amount, comparisonOperator);
			}
			else
			{
				return CheckCondition(character);
			}
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
				return character.Inven.Exist(ItemManager.Instance.GetByIndex(amount));
			}
			else if(subjectType == eSubjectType.INFO)
			{
				left = character.Stat.InfoNum;
			}
			else if(subjectType == eSubjectType.SATIETY)
			{
				left = character.Stat.Satiety;
			}
			else if(subjectType == eSubjectType.FOOD)
			{
				left = character.Inven.FindAll(EnumConvert<ItemTag>.ToEnum(EnumConvert<eSubjectType>.ToString(subjectType))).Count;
			}
			else if(subjectType == eSubjectType.NULL)
			{
				return true;
			}
			else if(subjectType == eSubjectType.EVENT)
			{
				return false;
			}
			return Compare(left, amount, comparisonOperator);
		}

		public static bool Compare(int left, int right, string comparisonOp)
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