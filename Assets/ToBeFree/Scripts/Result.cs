using System;

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
        private EffectAmount[] effects;

        public string Script
        {
            get
            {
                return script;
            }
        }

        public EffectAmount[] Effects
        {
            get
            {
                return effects;
            }
        }

        public ResultScriptAndEffects(string script, EffectAmount[] effects)
        {
            this.script = script;
            this.effects = effects;
        }
    }

    public class EffectAmount
    {
        private Effect effect;
        private int amount;
        

        public EffectAmount(Effect effect, int amount=0)
        {
            this.effect = effect;
            this.amount = amount;
        }

        public void Activate(Character character)
        {
            effect.Activate(character, amount);
        }

        public void Deactivate(Character character)
        {
            effect.Deactivate(character);
        }

        public int Amount
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