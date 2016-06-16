using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
    public class UIBuff : MonoBehaviour
    {
        public UILabel buffName;
        public UILabel amount;
        public UILabel stack;

        private Buff buff;

        public void SetInfo(Buff buff)
        {
            this.buff = buff;
            buffName.text = buff.Name;
            amount.text = buff.Amount.ToString();
            stack.text = buff.Stack.ToString();
        }
    }
}