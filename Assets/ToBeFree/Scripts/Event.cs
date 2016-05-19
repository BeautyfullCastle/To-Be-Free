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
        private Select[] selectList;

        public Event()
        {
            selectList = new Select[3];
        }

        public Event(string actionType, string region, string stat, string script, Result result, bool bSelect, Select[] selectList)
         : this()
        {
            this.actionType = actionType;
            this.region = region;
            this.stat = stat;
            this.script = script;
            this.result = result;
            this.bSelect = bSelect;
            this.selectList = selectList;
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

        public Select[] SelectList
        {
            get
            {
                return selectList;
            }

            set
            {
                selectList = value;
            }
        }
    }
}
