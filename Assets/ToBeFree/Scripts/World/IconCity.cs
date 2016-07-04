using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
    public class IconCity : MonoBehaviour
    {
        public UILabel nameLabel;
        public UISprite informSprite;
        public UISprite policeSprite;
        public UISprite questSprite;

        private City city;
        
        // Use this for initialization
        void Start()
        {
            city = CityManager.Instance.Find(EnumConvert<eCity>.ToEnum(this.name));
            nameLabel.text = this.name;

            //TimeTable.Instance.NotifyEveryday += CheckPieces;
            StartCoroutine(CheckPieces());
        }

        private IEnumerator CheckPieces()
        {
            while (true)
            {
                List<Piece> polices = PieceManager.Instance.FindAll(eSubjectType.POLICE);
                if (polices != null && polices.Count > 0)
                {
                    policeSprite.enabled = polices.Exists(x => x.City == this.city);
                }
                List<Piece> infos = PieceManager.Instance.FindAll(eSubjectType.INFO);
                if (infos != null && infos.Count > 0)
                {
                    informSprite.enabled = infos.Exists(x => x.City == this.city);
                }
                List<Piece> questPieces = PieceManager.Instance.FindAll(eSubjectType.QUEST);
                if (questPieces != null && questPieces.Count > 0)
                {
                    questSprite.enabled = questPieces.Exists(x => x.City == this.city);
                }

                yield return new WaitForSeconds(1f);
            }
        }

        public City City
        {
            get
            {
                return city;
            }
        }
    }
}