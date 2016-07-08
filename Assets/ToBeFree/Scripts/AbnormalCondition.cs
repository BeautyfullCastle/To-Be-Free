using System;
using System.Collections;

namespace ToBeFree
{
    public enum eBodyMental
    {
        BODY, MENTAL, NULL
    }

    public enum ePositiveNegative
    {
        POSITIVE, NEGATIVE, NULL
    }

    public class AbnormalCondition
    {
        private readonly string name;
        readonly protected Buff buff;
        readonly protected Condition spawnCondition;
        readonly protected int stack;
        readonly protected bool isStack;
        readonly protected eBodyMental isBody; // body or mental
        readonly protected ePositiveNegative isPositive;

        public AbnormalCondition(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive)
        {
            this.name = name;
            this.buff = new Buff(buff);
            this.spawnCondition = spawnCondition;
            this.isBody = isBody;
            this.isPositive = isPositive;
        }
        
        public virtual IEnumerator Activate(Character character)
        {
            yield return BuffManager.Instance.Add(this.buff);
        }

        public virtual IEnumerator DeActivate(Character character)
        {
            yield return BuffManager.Instance.Delete(this.buff, character);

            yield return null;
        }

        public virtual bool CheckCondition(Character character)
        {
            return spawnCondition.CheckCondition(character);
        }

        public Buff Buff
        {
            get
            {
                return buff;
            }
        }

        public Condition SpawnCondition
        {
            get
            {
                return spawnCondition;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }

    public class Despair : AbnormalCondition
    {

        public Despair(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {
            
        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);

            Buff buff_exhilaration = BuffManager.Instance.Find("Exhilaration");
            if (buff_exhilaration == null)
            {
                yield break;
            }
            BuffManager.Instance.Delete(buff_exhilaration, character);
            yield return null;
            
        }
    }

    public class LegInjury : AbnormalCondition
    {
        public LegInjury(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class WristInjury : AbnormalCondition
    {
        public WristInjury(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class InternalInjury : AbnormalCondition
    {
        public InternalInjury(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class Fatigue : AbnormalCondition
    {
        public Fatigue(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class AnxietyDisorder : AbnormalCondition
    {
        public AnxietyDisorder(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class Exhilaration : AbnormalCondition
    {
        public Exhilaration(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);

            Buff buff_exhilaration = BuffManager.Instance.Find("Despair");
            if (buff_exhilaration == null)
            {
                yield break;
            }
            BuffManager.Instance.Delete(buff_exhilaration, character);
            yield return null;
        }
    }

    public class Full : AbnormalCondition
    {
        public Full(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class Concentration : AbnormalCondition
    {
        public Concentration(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class Detention : AbnormalCondition
    {
        public Detention(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            character.IsDetention = true;
            
            yield return null;
        }

        public override IEnumerator DeActivate(Character character)
        {
            base.DeActivate(character);
            character.IsDetention = false;
            yield return null;
        }
    }

    public class DecreaseStrength : AbnormalCondition
    {
        public DecreaseStrength(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class DecreaseAgility : AbnormalCondition
    {
        public DecreaseAgility(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class DecreaseObservation : AbnormalCondition
    {
        public DecreaseObservation(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class DecreaseBargain : AbnormalCondition
    {
        public DecreaseBargain(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class DecreasePatience : AbnormalCondition
    {
        public DecreasePatience(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class DecreaseLuck : AbnormalCondition
    {
        public DecreaseLuck(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }

    public class DecreaseAllStat : AbnormalCondition
    {
        public DecreaseAllStat(string name, Buff buff, Condition spawnCondition, eBodyMental isBody, ePositiveNegative isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {

        }

        public override IEnumerator Activate(Character character)
        {
            base.Activate(character);
            yield return null;
        }
    }
}