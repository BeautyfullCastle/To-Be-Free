namespace ToBeFree
{
    public class Result
    {
        private eTestStat testStat;
        private ResultScriptAndEffects success;
        private ResultScriptAndEffects failure;

        public Result(eTestStat testStat, ResultScriptAndEffects success, ResultScriptAndEffects failure)
        {
            this.testStat = testStat;
            this.success = success;
            this.failure = failure;
        }

        public eTestStat TestStat
        {
            get
            {
                return testStat;
            }
        }

        public ResultScriptAndEffects Success
        {
            get
            {
                return success;
            }
        }

        public ResultScriptAndEffects Failure
        {
            get
            {
                return failure;
            }
        }
    }

    public class ResultScriptAndEffects
    {
        private string script;
        private ResultEffect[] effects;

        public string Script
        {
            get
            {
                return script;
            }
        }

        public ResultEffect[] Effects
        {
            get
            {
                return effects;
            }
        }

        public ResultScriptAndEffects(string script, ResultEffect[] effects)
        {
            this.script = script;
            this.effects = effects;
        }
    }

    public struct ResultEffect
    {
        private Effect effect;
        private int amount;
        

        public ResultEffect(int index, Effect effect, AbnormalCondition abnormalCondition=null, int amount=0)
        {
            this.effect = effect;
            this.amount = amount;
        }

        public int Value
        {
            get
            {
                return amount;
            }
        }

        public Effect Effect
        {
            get
            {
                return effect;
            }
        }
    }
}