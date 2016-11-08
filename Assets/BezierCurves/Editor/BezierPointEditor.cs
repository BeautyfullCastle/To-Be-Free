using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(BezierPoint))]
[CanEditMultipleObjects]
public class BezierPointEditor : Editor {
	
	BezierPoint point;
<<<<<<< HEAD

    SerializedProperty curveProp;
    SerializedProperty handleProp;
=======
	
	SerializedProperty handleProp;
>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b

	private static bool showPoints = true;

	void OnEnable(){
		point = (BezierPoint)target;

<<<<<<< HEAD
        curveProp = serializedObject.FindProperty("_curve");
		handleProp = serializedObject.FindProperty("handles");
=======
		handleProp = serializedObject.FindProperty("_handle");
>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b
	}	
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

<<<<<<< HEAD
        EditorGUILayout.PropertyField(curveProp);
        EditorGUILayout.PropertyField(handleProp, true);

        showPoints = EditorGUILayout.Foldout(showPoints, "Handles");

		if (showPoints)
		{
			int handleCount = point.handleCount;
=======
		showPoints = EditorGUILayout.Foldout(showPoints, "Handles");

		if (showPoints)
		{
			int handleCount = handleProp.arraySize;
>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b

			for (int i = 0; i < handleCount; i++)
			{
				DrawHandleInspector(point[i], i);
			}

			if (GUILayout.Button("Add Handle"))
			{
				addHandle();
			}
		}

		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}

<<<<<<< HEAD
=======
	static public void AddHandle(BezierPoint point)
	{
		//this.point
	}

>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b
	private void addHandle()
	{ 
		Undo.RegisterSceneUndo("Add Handle");

		point.AddHandleObj();
		//handleProp.InsertArrayElementAtIndex(handleProp.arraySize);
		//handleProp.GetArrayElementAtIndex(handleProp.arraySize - 1).objectReferenceValue = newHandle;
	}

	void OnSceneGUI()
	{
		Handles.color = Color.green;
		Vector3 newPosition = Handles.FreeMoveHandle(point.position, point.transform.rotation, HandleUtility.GetHandleSize(point.position)*0.2f, Vector3.zero, Handles.CubeCap);
		if(point.position != newPosition) point.position = newPosition;
		
		BezierCurveEditor.DrawOtherPoints(point.curve, point);
	}

	void DrawHandleInspector(BezierHandle handle, int index)
	{
<<<<<<< HEAD
        if (handle == null)
            return;

=======
>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b
		SerializedObject serObj = new SerializedObject(handle);

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("X", GUILayout.Width(20)))
		{
			Undo.RegisterSceneUndo("Remove Handle");
<<<<<<< HEAD
            point.RemoveHandle(handle);
            DestroyImmediate(handle.gameObject);
            
=======
			handleProp.MoveArrayElement(point.GetHandleIndex(handle), point.handleCount - 1);
			handleProp.arraySize--;
			DestroyImmediate(handle.gameObject);
>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b
			return;
		}

		EditorGUILayout.ObjectField(handle.gameObject, typeof(GameObject), true);

		if (index != 0 && GUILayout.Button(@"/\", GUILayout.Width(25)))
		{
			UnityEngine.Object other = handleProp.GetArrayElementAtIndex(index - 1).objectReferenceValue;
			handleProp.GetArrayElementAtIndex(index - 1).objectReferenceValue = handle;
			handleProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
		}

		if (index != handleProp.arraySize - 1 && GUILayout.Button(@"\/", GUILayout.Width(25)))
		{
			UnityEngine.Object other = handleProp.GetArrayElementAtIndex(index + 1).objectReferenceValue;
			handleProp.GetArrayElementAtIndex(index + 1).objectReferenceValue = handle;
			handleProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
		}

		EditorGUILayout.EndHorizontal();

		EditorGUI.indentLevel++;

		Vector3 newHandlePos = EditorGUILayout.Vector3Field("Position : ", handle.transform.localPosition);
		if (newHandlePos != handle.transform.localPosition)
		{
			Undo.RegisterUndo(handle.transform, "Move Bezier Handle");
			handle.transform.localPosition = newHandlePos;
		}

		EditorGUI.indentLevel--;

		if (GUI.changed)
		{
			serObj.ApplyModifiedProperties();
			EditorUtility.SetDirty(serObj.targetObject);
		}
	}
}