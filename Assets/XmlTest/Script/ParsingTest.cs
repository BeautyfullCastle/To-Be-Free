using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text;
using System;

public class ParsingTest : MonoBehaviour
{

    // 파싱 할 xml 파일명
    string m_strName = "test.xml";

    // Use this for initialization
    void Start()
    {
        // 파싱 시작해요
        StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        string strPath = string.Empty;
        // 플랫폼 별로 다르게!!
#if ( UNITY_EDITOR || UNITY_STANDALONE_WIN )
        strPath += ("file:///");
        strPath += (Application.streamingAssetsPath + "/" + m_strName);
#elif UNITY_ANDROID
        strPath =  "jar:file://" + Application.dataPath + "!/assets/" + m_strName;
#endif

        WWW www = new WWW(strPath);

        yield return www;

        Debug.Log("Read Content : " + www.text);

        Interpret(www.text);
    }

    private void Interpret(string _strSource)
    {
        // 인코딩 문제 예외처리.
        // 읽은 데이터의 앞 2바이트 제거(BOM제거)
        // 혹시 오류나시면 BOM제거 부분 코드 없애고 해보시길 바랍니다~!
        StringReader stringReader = new StringReader(_strSource);

        //stringReader.Read();    // BOM 제거 한 데이터로 파싱해요.

        XmlNodeList xmlNodeList = null;

        XmlDocument xmlDoc = new XmlDocument();
        // XML 로드하고.
        xmlDoc.LoadXml(stringReader.ReadToEnd());
        // 최 상위 노드 선택.
        xmlNodeList = xmlDoc.SelectNodes("MyTest");

        foreach (XmlNode node in xmlNodeList)
        {
            // 자식이 있을 때에 돌아요.
            if (node.Name.Equals("MyTest") && node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    Debug.Log("id : " + child.Attributes.GetNamedItem("id").Value);

                    Debug.Log("value : " + child.Attributes.GetNamedItem("value").Value);
                }
            }
        }
    }
}