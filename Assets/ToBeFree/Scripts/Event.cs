using System.Collections;

namespace ToBeFree
{
    public class Event
    {
        private string actionType;
        private string region;
        private string stat;
        private string script;
        private Result result;
        private bool bSelect;
        private int[] selectIndexList;

        public Event()
        {
            selectIndexList = new int[3];
        }

        public Event(string actionType, string region, string stat, string script, Result result, bool bSelect, int[] selectIndexList)
         : this()
        {
            this.actionType = actionType;
            this.region = region;
            this.stat = stat;
            this.script = script;
            this.result = result;
            this.bSelect = bSelect;
            this.selectIndexList = selectIndexList;
        }
        
        public string ActionType { get { return actionType; } }
        public string Region { get { return region; } }
        public string Stat { get { return stat; } }
        public string Script { get { return script; } }
        public bool BSelect { get { return bSelect; } }

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
