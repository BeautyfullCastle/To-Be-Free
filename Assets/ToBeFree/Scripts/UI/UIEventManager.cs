using UnityEngine;
using System.Collections;


namespace ToBeFree
{
    public enum eUIEventLabelType
    {
        EVENT, RESULT, RESULT_EFFECT, DICENUM
    }

    public class UIEventManager : MonoBehaviour
    {
        public UILabel eventScript;
        public UILabel resultScript;
        public UILabel[] selectLabels;
        public UILabel resultEffectScript;
        public UILabel diceNum;
        
        private UILabel[] allLabel;
        private Select[] selectList;

        public void Start()
        {
            allLabel = new UILabel[7];
            allLabel[0] = eventScript;
            allLabel[1] = resultScript;
            allLabel[2] = resultEffectScript;
            allLabel[3] = diceNum;
            allLabel[4] = selectLabels[0];
            allLabel[5] = selectLabels[1];
            allLabel[6] = selectLabels[2];

            EventManager.UIChanged += OnChanged;
            EventManager.SelectUIChanged += OnSelectUIChanged;
            EventManager.UIOpen += () => { gameObject.SetActive(true); ClearAll(); };

            ClearAll();
            gameObject.SetActive(false);
        }

        public void OnChanged(eUIEventLabelType type, string text)
        {
            gameObject.SetActive(true);
            switch (type)
            {
                case eUIEventLabelType.EVENT:
                    eventScript.text = text;
                    break;
                case eUIEventLabelType.RESULT:
                    resultScript.text = text;
                    break;
                case eUIEventLabelType.RESULT_EFFECT:
                    resultEffectScript.text = text;
                    break;
                case eUIEventLabelType.DICENUM:
                    diceNum.text = text;
                    break;
                default:
                    break;
            }
        }

        public void OnSelectUIChanged(Select[] selectList)
        {
            this.selectList = selectList;

            for (int i = 0; i < selectLabels.Length; ++i)
            {
                selectLabels[i].text = string.Empty;
            }

            for (int i=0; i<selectList.Length; ++i)
            {
                selectLabels[i].text = selectList[i].Script;
            }
        }
        
        public void OnClick(string index)
        {
            ClearAll();
            SelectManager.Instance.OnClick(selectList[int.Parse(index)]);
        }

        public void OnClickOK()
        {
            ClearAll();
            gameObject.SetActive(false);
        }

        public void ClearAll()
        {
            for (int i = 0; i < allLabel.Length; ++i)
            {
                allLabel[i].text = string.Empty;
            }
        }
    }
}