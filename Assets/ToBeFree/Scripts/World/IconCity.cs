using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ToBeFree
{
    public class IconCity : MonoBehaviour
    {
        public UILabel nameLabel;
        public GameObject informSprite;        
        public GameObject questSprite;
        public GameObject brokerSprite;
        public GameObject policeSprite;
        public UILabel policeNumLabel;

        private City city;

        // Use this for initialization
        void Awake()
        {
            city = CityManager.Instance.Find(EnumConvert<eCity>.ToEnum(this.name));
            nameLabel.text = this.name;

            PieceManager.AddPiece += PieceManager_AddPiece;
            PieceManager.DeletePiece += PieceManager_DeletePiece;

            informSprite.SetActive(false);
            policeSprite.SetActive(false);
            questSprite.SetActive(false);
            brokerSprite.SetActive(false);
        }

        void Start()
        {
            UIButton m_BtnTest = GetComponent<UIButton>();
            
            EventDelegate.Parameter param = new EventDelegate.Parameter();

            // 파라메타로 지정할 오브젝트가 있는 컴포넌트
            param.obj = gameObject.GetComponent<Transform>();

            // 해당 오브젝트의 변수명
            param.field = "name";

            // 이벤트 등록 및 파라메타 추가
            EventDelegate onClick = new EventDelegate(GameObject.FindObjectOfType<GameManager>(), "ClickCity");

            onClick.parameters[0] = param;

            EventDelegate.Add(m_BtnTest.onClick, onClick);
        
    }

        private void PieceManager_DeletePiece(Piece piece)
        {
            if (piece.City == null)
            {
                return;
            }
            if (piece.City.Name.ToString() != this.name)
            {
                return;
            }
            SetPieceSprite(piece.SubjectType, false);
        }

        private void PieceManager_AddPiece(Piece piece)
        {
            if(piece.City == null)
            {
                return;
            }
            if (piece.City.Name.ToString() != this.name)
            {
                return;
            }
            SetPieceSprite(piece.SubjectType, true);
        }

        private void SetPieceSprite(eSubjectType type, bool isExist)
        {
            if (type == eSubjectType.POLICE)
            {
                policeSprite.SetActive(isExist);
                policeNumLabel.text = PieceManager.Instance.GetNumberOfPiece(eSubjectType.POLICE, this.City).ToString();
            }
            else if (type == eSubjectType.INFO)
            {
                informSprite.SetActive(isExist);
            }
            else if (type == eSubjectType.QUEST)
            {
                questSprite.SetActive(isExist);
            }
            else if (type == eSubjectType.BROKER)
            {
                brokerSprite.SetActive(isExist);
            }
            NGUIDebug.Log(this.name + "'s " + type.ToString() + " sprite is " + isExist);
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