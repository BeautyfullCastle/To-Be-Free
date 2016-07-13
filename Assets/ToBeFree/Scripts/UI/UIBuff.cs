﻿using UnityEngine;
using System.Collections;
using System;

namespace ToBeFree
{
    public class UIBuff : MonoBehaviour
    {
        public UILabel buffName;
        public UILabel amount;

        private Buff buff;

        public void SetInfo(Buff buff)
        {
            this.buff = buff;
            buffName.text = buff.Name;
            for (int i = 0; i < buff.EffectAmountList.Length; ++i)
            {
                amount.text += buff.EffectAmountList[i].ToString();
            }
        }
    }
}