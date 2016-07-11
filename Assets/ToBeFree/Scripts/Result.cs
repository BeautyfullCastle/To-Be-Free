using System;
using System.Collections;
using UnityEngine;

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

        public EffectAmount[] EffectAmounts
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

        public IEnumerator Activate(Character character)
        {
            if(effect==null)
            {
                Debug.LogError("effect is null.");
                yield break;
            }
            yield return effect.Activate(character, amount);
        }

        public void Deactivate(Character character)
        {
            if (effect == null)
            {
                Debug.LogError("effect is null.");
                return;
            }
            effect.Deactivate(character);
        }

        public override string ToString()
        {
            return effect.ToString() + " : " + amount.ToString();
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