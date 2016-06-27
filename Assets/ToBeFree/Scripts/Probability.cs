using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class Probability
    {
        protected int[] valueList; // Key : dataType, Value : dataValue

        protected Probability() { }

        private void CalculateTotalProb(int[] values)
        {
            int totalProb = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                totalProb += values[i];
            }
            if (totalProb != 100)
            {
                Debug.LogError("RegionProbability is not 100%.");
                return;
            }
        }

        public int CheckAddedAllProbValues()
        {
            int addedProbValue = 0;
            foreach (int val in valueList)
            {
                addedProbValue += val;
            }
            return addedProbValue;
        }

        public void ResetProbValues(Dictionary<int, List<Event>> eventListDic)
        {
            for (int index = 0; index < valueList.Length; ++index)
            {
                if (eventListDic[index].Count == 0) // error
                {
                    valueList[index] = 0;
                }
            }
        }

        public int[] DataList
        {
            get
            {
                return valueList;
            }

            protected set
            {
                valueList = value;
            }
        }
    }

    public class RegionProbability : Probability
    {
        public RegionProbability(int[] valueList)
        {
            this.valueList = valueList;
        }

        public RegionProbability DeepCopy()
        {
            RegionProbability prob = (RegionProbability)this.MemberwiseClone();
            prob.DataList = (int[])DataList.Clone();

            return prob;
        }

    }

    public class StatProbability : Probability
    {
        private eEventAction actionType;

        public StatProbability(eEventAction actionType, int[] valueList)
        {
            this.actionType = actionType;
            this.valueList = valueList;
        }

        public StatProbability DeepCopy()
        {
            StatProbability prob = (StatProbability)this.MemberwiseClone();
            prob.ActionType = ActionType;
            prob.DataList = (int[])DataList.Clone();

            return prob;
        }

        public eEventAction ActionType
        {
            get
            {
                return actionType;
            }

            private set
            {
                actionType = value;
            }
        }
    }
}