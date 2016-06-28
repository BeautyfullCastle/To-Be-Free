using UnityEngine;
using System.Collections;


namespace ToBeFree
{
    public enum eUIEventLabelType
    {
        EVENT, RESULT, SELECT_0, SELECT_1, SELECT_2, RESULT_EFFECT, DICENUM
    }

    public class UIEventManager : MonoBehaviour
    {
        public UILabel eventScript;
        public UILabel resultScript;
        public UILabel[] selectScript;
        public UILabel resultEffectScript;
        public UILabel diceNum;
        
        private UILabel[] allLabel;

        public void Start()
        {
            allLabel = new UILabel[7];
            allLabel[0] = eventScript;
            allLabel[1] = resultScript;
            allLabel[2] = resultEffectScript;
            allLabel[3] = diceNum;
            allLabel[4] = selectScript[0];
            allLabel[5] = selectScript[1];
            allLabel[6] = selectScript[2];

            EventManager.UIChanged += OnChanged;
            EventManager.UIOpen += () => gameObject.SetActive(true);

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
                case eUIEventLabelType.SELECT_0:
                    selectScript[0].text = text;
                    break;
                case eUIEventLabelType.SELECT_1:
                    selectScript[1].text = text;
                    break;
                case eUIEventLabelType.SELECT_2:
                    selectScript[2].text = text;
                    break;
                default:
                    break;
            }
        }
        
        public void OnClick(string text)
        {
            ClearAll();
            SelectManager.Instance.OnClick(text);
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