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
        void Awake()
        {
            city = CityManager.Instance.Find(EnumConvert<eCity>.ToEnum(this.name));
            nameLabel.text = this.name;

            PieceManager.AddPiece += PieceManager_AddPiece;
            PieceManager.DeletePiece += PieceManager_DeletePiece;

            informSprite.enabled = false;
            policeSprite.enabled = false;
            questSprite.enabled = false;

            
            //TimeTable.Instance.NotifyEveryday += CheckPieces;
            //StartCoroutine(CheckPieces());
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
                policeSprite.enabled = isExist;
            }
            else if (type == eSubjectType.INFO)
            {
                informSprite.enabled = isExist;
            }
            else if (type == eSubjectType.QUEST)
            {
                questSprite.enabled = isExist;
            }
            NGUIDebug.Log(this.name + "'s " + type.ToString() + " sprite is " + isExist);
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

                yield return new WaitForSeconds(.2f);
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