using System;

namespace ToBeFree
{
    public class AbnormalCondition
    {
        private string name;
        private Buff buff;
        private Condition spawnCondition;
        private bool isBody; // body or mental
        private bool isPositive;

        public AbnormalCondition(string name, Buff buff, Condition spawnCondition, bool isBody, bool isPositive)
        {
            this.name = name;
            this.buff = buff;
            this.spawnCondition = spawnCondition;
            this.isBody = isBody;
            this.isPositive = isPositive;
        }
        
        public void Activate(Character character, int value)
        {
            if(name == "despair")
            {
                if(BuffList.Instance.Contains("exhilaration"))
                {
                    BuffList.Instance.Delete(BuffList.Instance.Find("exhilaration"));
                }
            }
            BuffList.Instance.Add(this.buff);
        }


        public Buff Buff
        {
            get
            {
                return buff;
            }

            set
            {
                buff = value;
            }
        }

        public Condition SpawnCondition
        {
            get
            {
                return spawnCondition;
            }

            set
            {
                spawnCondition = value;
            }
        }
    }
}