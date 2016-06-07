using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToBeFree
{
    public class DiceTester : Singleton<DiceTester>
    {
        private int minSuccessNum;
        
        public delegate void TestEventHandler(eStartTime startTime, Character character);
        public event TestEventHandler StartTestNotify;
        public event TestEventHandler EndTestNotify;

        public DiceTester()
        {
            minSuccessNum = 4;
        }

        public bool Test(int diceNum, Character character)
        {
            if(StartTestNotify != null)
                StartTestNotify(eStartTime.TEST, character);

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

            if(EndTestNotify != null)
                EndTestNotify(eStartTime.TEST, character);

            if (successDiceNum > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
