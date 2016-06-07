using System;

namespace ToBeFree
{
    public class AbnormalCondition
    {
        protected string name;
        protected Buff buff;
        protected Condition spawnCondition;
        protected bool isBody; // body or mental
        protected bool isPositive;

        public AbnormalCondition(string name, Buff buff, Condition spawnCondition, bool isBody, bool isPositive)
        {
            this.name = name;
            this.buff = new Buff(buff);
            this.spawnCondition = spawnCondition;
            this.isBody = isBody;
            this.isPositive = isPositive;
        }
        
        public virtual void Activate(Character character, int value)
        {
            BuffList.Instance.Add(this.buff);
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
    }

    public class Despair : AbnormalCondition
    {

        public Despair(string name, Buff buff, Condition spawnCondition, bool isBody, bool isPositive) : base(name, buff, spawnCondition, isBody, isPositive)
        {
            
        }

        public override void Activate(Character character, int value)
        {
            base.Activate(character, value);

            Buff buff_exhilaration = BuffList.Instance.Find("Exhilaration");
            if (buff_exhilaration == null)
            {
                return;
            }
            BuffList.Instance.Delete(buff_exhilaration, character);
        }
    }
}