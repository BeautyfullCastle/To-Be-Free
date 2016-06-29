using UnityEngine;
using System.Collections;

namespace ToBeFree
{
    public class IconCharacter : MonoBehaviour
    {
        public Transform Cities;

        // Use this for initialization
        void Awake()
        {
            Character.MoveCity += MoveCity;
        }

        private void MoveCity(string cityName)
        {
            this.transform.position = Cities.Find(cityName).position;
            NGUIDebug.Log("Moved to " + cityName);
        }
    }
}