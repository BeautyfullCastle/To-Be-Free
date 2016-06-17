namespace ToBeFree
{
    public enum eEventAction
    {
        WORK = 0, MOVE, INSPECT, DETENTION, ESCAPE, GLOBAL, NULL
    }

    public enum eDifficulty
    {
        EASY, NORMAL, HARD, NULL
    }

    public class Event
    {
        private string actionType;
        private string region;
        private string stat;
        private eDifficulty difficulty;
        private string script;
        private Result result;
        private bool bSelect;
        private Select[] selectList;

        // for quest
        private int remainDay;

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

            if(ActionType == "QUEST")
            {

            }
        }

        public Event(Event event_)
        {
            this.actionType = string.Copy(event_.actionType);
            this.region = string.Copy(event_.region);
            this.stat = string.Copy(event_.stat);
            this.script = string.Copy(event_.script);
            this.result = new Result(event_.result.TestStat, event_.result.Success, event_.result.Failure);
            this.bSelect = event_.bSelect;
            this.selectList = (Select[])event_.selectList.Clone();
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