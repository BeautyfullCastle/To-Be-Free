using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class DiceTester : Singleton<DiceTester>
    {
        private int minSuccessNum;

        public DiceTester()
        {
            minSuccessNum = 5;
        }

        public int Test(int diceNum)
        {
            int successDiceNum = 0;
            System.Random r = new System.Random();
            for (int i = 0; i < diceNum; ++i)
            {
                int randNum = r.Next(1, 7);
                if (randNum >= minSuccessNum)
                {
                    successDiceNum++;
                }
                //Debug.Log("dice test rand num : " + randNum);
            }
            Debug.Log("Dice test succeed? " + successDiceNum);

            return successDiceNum;
        }

        public int MinSuccessNum
        {
            get
            {
                return minSuccessNum;
            }

            set
            {
                minSuccessNum = value;
            }
        }
    }
}
