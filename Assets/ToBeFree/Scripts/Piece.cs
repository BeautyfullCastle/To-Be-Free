using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ToBeFree
{
    public class Piece { }

    public class Police : Piece { }

    public class Information : Piece { }

    public class Broker : Piece { }

    public class Quest : Piece
    {
        private int duration;

        public Quest(int duration)
        {
            this.duration = duration;
            TimeTable.Instance.NotifyEveryday += CheckTimeToDisapper;
        }

        public void CheckTimeToDisapper()
        {
            duration--;
            if(duration == 0)
            {
                Disapper();
            }
        }

        private void Disapper()
        {
            Debug.Log("You Die Sensei");
        }
    }

}
