using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BezierHandle))]
[CanEditMultipleObjects]
public class BezierHandleEditor : Editor
{
	BezierHandle handle;

	SerializedProperty handle1Prop;
	SerializedProperty handle2Prop;
	SerializedProperty pointProp;
	SerializedProperty curveProp;

	void OnEnable()
	{
		handle = (BezierHandle)target;

		handle1Prop = serializedObject.FindProperty("_handle1");
		handle2Prop = serializedObject.FindProperty("_handle2");
		pointProp = serializedObject.FindProperty("_point");
		curveProp = serializedObject.FindProperty("_curve");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(curveProp);
		EditorGUILayout.PropertyField(pointProp);
		
		EditorGUI.indentLevel++;

		Vector3 newHandlePos1 = EditorGUILayout.Vector3Field("Handle 1 Position : ", handle1Prop.vector3Value);
		if (newHandlePos1 != handle1Prop.vector3Value)
		{
			Undo.RegisterUndo(handle.transform, "Move Bezier Handle 1");
			handle1Prop.vector3Value = newHandlePos1;
		}

		Vector3 newHandlePos2 = EditorGUILayout.Vector3Field("Handle 2 Position : ", handle2Prop.vector3Value);
		if (newHandlePos2 != handle2Prop.vector3Value)
		{
			Undo.RegisterUndo(handle.transform, "Move Bezier Handle 2");
			handle2Prop.vector3Value = newHandlePos2;
		}

		EditorGUI.indentLevel--;

		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}

	void OnSceneGUI()
	{
		BezierCurveEditor.DrawHandleSceneGUI(handle, handle.point);
	}
}