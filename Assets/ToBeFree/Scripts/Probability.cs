using UnityEngine;
using System.Collections.Generic;

namespace ToBeFree {
    public enum eRegion {
        AREA = 0, CITY, ALL
    }
    
    public enum eStat {
        STR = 0, AGI, OBS, BAR, PAT, LUC
    }
        
    public class Probability {
        private string actionType;
        private List<int> dataList; // Key : dataType, Value : dataValue

        public Probability(string actionType, List<int> dataList) {
            this.actionType = actionType;
            this.dataList = dataList;
        }
        
        private void CalculateTotalProb(int[] values) {
            int totalProb = 0;
            for(int i=0; i<values.Length; ++i) {
                totalProb += values[i];
            }
            if(totalProb != 100) {
                Debug.LogError("RegionProbability is not 100%.");
                //return;
            }
        }
        
        public int CheckAddedAllProbValues() {
            int addedProbValue = 0;
            foreach(int val in dataList) { 
                addedProbValue += val;
            }
            return addedProbValue;
        }
        
        public void ResetProbValues(Dictionary<int, List<Event>> eventListDic) {
            
            for (int index=0; index<dataList.Count; ++index)
            {
                if (eventListDic[index].Count == 0) // error
                {
                    dataList[index] = 0;
                }
            }            
        }

        
        
        public string ActionType
        {
            get
            {
                return actionType;
            }

            set
            {
                actionType = value;
            }
        }

        public List<int> DataList
        {
            get
            {
                return dataList;
            }

            set
            {
                dataList = value;
            }
        }
    }

}