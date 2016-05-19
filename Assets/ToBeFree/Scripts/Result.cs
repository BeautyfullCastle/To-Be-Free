using System.Collections.Generic;

namespace ToBeFree
{
    public class Result
    {
        private string testStat;
        private ResultScriptAndEffects success;
        private ResultScriptAndEffects failure;

        public Result(string testStat, ResultScriptAndEffects success, ResultScriptAndEffects failure)
        {
            this.testStat = testStat;
            this.success = success;
            this.failure = failure;
        }

        public string TestStat
        {
            get
            {
                return testStat;
            }

            set
            {
                testStat = value;
            }
        }

        public ResultScriptAndEffects Success
        {
            get
            {
                return success;
            }

            set
            {
                success = value;
            }
        }

        public ResultScriptAndEffects Failure
        {
            get
            {
                return failure;
            }

            set
            {
                failure = value;
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

            set
            {
                script = value;
            }
        }

        public ResultEffect[] Effects
        {
            get
            {
                return effects;
            }

            set
            {
                effects = value;
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
        private int index;
        private Effect effect;
        private int value;
        
        public ResultEffect(int index, Effect effect, int value)
        {
            this.index = index;
            this.effect = effect;
            this.value = value;
        }

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public Effect Effect
        {
            get
            {
                return effect;
            }

            set
            {
                effect = value;
            }
        }
    }
}