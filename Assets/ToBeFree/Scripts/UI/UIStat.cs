using UnityEngine;
using System.Collections;

namespace ToBeFree {
    public class UIStat : MonoBehaviour {
        public eStat stat;
        private UILabel label;

        // Use this for initialization
        void Awake() {
            label = this.GetComponent<UILabel>();
            Stat.OnValueChange += OnValueChange;

        }

        void OnValueChange(int value, eStat stat)
        {
            if(stat != this.stat)
            {
                return;
            }

            label.text = value.ToString();
        }

        
    }
}