using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree {
    public enum eActionByEvent {
        WORK = 0, MOVE, INFO, BROKER, INSPECTION, TAKEN, ESCAPE
    }
    
    public class ProbabilityManager : Singleton<ProbabilityManager> {
        private Probability[] regionProbs;
        private Probability[] statProbs;

        public void Init(Probability[] regionProbs, Probability[] statProbs)
        {
            this.regionProbs = regionProbs;
            this.statProbs = statProbs;
        }
        
        public Probability FindProbByAction(string actionType, string probName) {
            Probability[] probs = null;
            if(probName == "Region") {
                probs = regionProbs;
            }
            else if(probName == "Stat") {
                probs = statProbs;
            }
            
            Probability prob = null;
            for(int i=0; i<probs.Length; ++i) {
                if(probs[i].ActionType != actionType) {
                    continue;
                }
                prob = probs[i];
            }
            return prob;
        }
       
        public void ResetProbValues(Probability prob, string probType, List<string> probDataTypeLIst) {
            if(prob == null) {
                Debug.LogError("regionProb is null.");
                return;
            }

            //foreach (string probDataType in probDataTypeLIst)
            //{
            //    prob.ResetProbValues(probDataType, probType);
            //}
        }


        public string ConvertToString(eStat estat)
        {
            switch (estat)
            {
                case eStat.STR:
                    return "STR";
                case eStat.AGI:
                    return "AGI";
                case eStat.OBS:
                    return "OBS";
                case eStat.BAR:
                    return "BAR";
                case eStat.PAT:
                    return "PAT";
                case eStat.LUC:
                    return "LUC";
                default:
                    return string.Empty;
            }
        }

        public string ConvertToString(eRegion eregion)
        {
            switch (eregion)
            {
                case eRegion.AREA:
                    return "AREA";
                case eRegion.CITY:
                    return "CITY";
                case eRegion.ALL:
                    return "ALL";
                default:
                    return string.Empty;
            }
        }

        public int ConvertToIndex(string data)
        {
            switch (data)
            {
                case "AREA":
                case "STR":
                    return 0;
                case "CITY":
                case "AGI":
                    return 1;
                case "ALL":
                case "OBS":
                    return 2;
                case "BAR":
                    return 3;
                case "PAT":
                    return 4;
                case "LUC":
                    return 5;
                default:
                    return -1;
            }
        }

    }
    
}