using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Node node = target as Node;
        if(GUILayout.Button("Add Node"))
        {
            node.AddNode();
        }
    }
    
}
