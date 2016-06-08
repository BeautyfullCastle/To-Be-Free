using UnityEngine;
using System.Collections;

namespace ToBeFree
{
    public class IconCity : MonoBehaviour
    {
        public UILabel nameLabel;
        public UISprite informSprite;
        public UISprite policeSprite;
        public UISprite questSprite;

        private ToBeFree.City city;

        // Use this for initialization
        void Start()
        {
            city = CityGraph.Instance.Find(this.name);
            nameLabel.text = city.Name;
            policeSprite.enabled = PieceManager.Instance.PoliceList.Exists(x => x.City == this.city);
            informSprite.enabled = PieceManager.Instance.InformList.Exists(x => x.City == this.city);
            questSprite.enabled = PieceManager.Instance.QuestList.Exists(x => x.City == this.city);
        }
        
    }
}