

using UnityEngine;

namespace ToBeFree
{
    public enum eEventAction
    {
        WORK = 0, MOVE, INSPECT, DETENTION, ESCAPE, GLOBAL, BROKER,
        QUEST,
        NULL
    }

    public enum eDifficulty
    {
        EASY, NORMAL, HARD
    }

    public class Event
    {
        private eEventAction actionType;
        private string region;
        private eTestStat stat;
        private eDifficulty difficulty;
        private string script;
        private int resultIndex;
        private int[] selectIndexList;        
        

        public Event(eEventAction actionType, string region, eTestStat stat, eDifficulty difficulty, string script, int resultIndex, int[] selectIndexList)
        {
            this.actionType = actionType;
            this.region = region;
            this.stat = stat;
            this.difficulty = difficulty;
            this.script = script;
            this.resultIndex = resultIndex;
            this.selectIndexList = selectIndexList;
        }

        public Event(Event event_)
        {
            this.actionType = event_.actionType;
            this.region = event_.region;
            this.stat = event_.stat;
            this.difficulty = event_.difficulty;
            this.script = string.Copy(event_.script);
            this.resultIndex = event_.resultIndex;
            this.selectIndexList = event_.selectIndexList;
        }
        
        public eEventAction ActionType { get { return actionType; } }
        public string Region { get { return region; } }
        public eTestStat TestStat { get { return stat; } }
        public string Script { get { return script; } }

        public bool HasSelect
        {
            get
            {
                if(selectIndexList[0] == -99)
                {
                    return false;
                }
                return true;
            }
        }

        public Result Result
        {
            get
            {
                if(resultIndex == -99)
                {
                    return null;
                }
                return ResultManager.Instance.List[resultIndex];
            }
        }

        public int[] SelectIndexList
        {
            get
            {
                if (HasSelect)
                {
                    return selectIndexList;
                }
                return null;
            }
        }
    }
}