namespace ToBeFree
{
    public class Stat
    {
        private int strength;
        private int agility;
        private int observation;
        private int bargain;
        private int patience;
        private int luck;
        private int totalHP;
        private int totalMental;
        private int totalFoodNum;

        public Stat()
        {
            this.strength = 2;
            this.agility = 2;
            this.observation = 2;
            this.bargain = 2;
            this.patience = 2;
            this.luck = 2;
            this.totalHP = 10;
            this.totalMental = 10;
            this.totalFoodNum = 10;
        }

        public Stat(Stat stat)
        {
            this.strength = stat.strength;
            this.agility = stat.agility;
            this.observation = stat.observation;
            this.bargain = stat.bargain;
            this.patience = stat.patience;
            this.luck = stat.luck;
            this.totalHP = stat.totalHP;
            this.totalMental = stat.totalMental;
            this.totalFoodNum = stat.totalFoodNum;
        }

        public int TotalHP { get { return totalHP; } }
        public int TotalMental { get { return totalMental; } }
        public int TotalFoodNum { get { return totalFoodNum; } }

        public int Strength
        {
            get
            {
                return strength;
            }
            set
            {
                strength = value;
            }
        }

        public int Agility
        {
            get
            {
                return agility;
            }

            set
            {
                agility = value;
            }
        }

        public int Observation
        {
            get
            {
                return observation;
            }

            set
            {
                observation = value;
            }
        }

        public int Bargain
        {
            get
            {
                return bargain;
            }

            set
            {
                bargain = value;
            }
        }

        public int Patience
        {
            get
            {
                return patience;
            }

            set
            {
                patience = value;
            }
        }

        public int Luck
        {
            get
            {
                return luck;
            }

            set
            {
                luck = value;
            }
        }
    }
}