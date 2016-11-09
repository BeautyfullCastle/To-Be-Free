using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor, ISerializationCallbackReceiver
{	
	BezierCurve curve;
	SerializedProperty resolutionProp;
	SerializedProperty closeProp;
	SerializedProperty pointsProp;
	SerializedProperty colorProp;
	
	private static bool showPoints = true;
	
	void OnEnable()
	{
		curve = (BezierCurve)target;
		
		resolutionProp = serializedObject.FindProperty("resolution");
		closeProp = serializedObject.FindProperty("_close");
		pointsProp = serializedObject.FindProperty("points");
		colorProp = serializedObject.FindProperty("drawColor");
	}
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(resolutionProp);
		EditorGUILayout.PropertyField(closeProp);
		EditorGUILayout.PropertyField(colorProp);

		EditorGUILayout.PropertyField(pointsProp, showPoints);

		showPoints = EditorGUILayout.Foldout(showPoints, "Points");

		if (showPoints)
		{
			for (int i = 0; i < curve.pointCount; i++)
			{
				DrawPointInspector(curve[i], i);
			}

			if (GUILayout.Button("Add Point"))
			{
				AddPoint();
			}
		}
		
		if(GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);

			CheckPointsModified();
		}
	}

	private void AddPoint()
	{
		Undo.RegisterSceneUndo("Add Point");

		GameObject pointObject = new GameObject("Point " + curve.pointCount);
		pointObject.transform.parent = curve.transform;
		pointObject.transform.localPosition = Vector3.zero;
		BezierPoint newPoint = pointObject.AddComponent<BezierPoint>();
		newPoint.curve = curve;

		//pointsProp.InsertArrayElementAtIndex(pointsProp.arraySize);
		//pointsProp.GetArrayElementAtIndex(pointsProp.arraySize - 1).objectReferenceValue = newPoint;

		newPoint.AddHandleObj();
	}

	BezierPoint[] prevPoints = null;
	// points element가 바뀔 때만 처리
	private void CheckPointsModified()
	{
		if (prevPoints == null)
		{
			prevPoints = (BezierPoint[])curve.Points.Clone();
			return;
		}

		if(curve.Points.Length == prevPoints.Length)
		{
			for (int i = 0; i < curve.Points.Length; ++i)
			{
				if (curve.Points[i] == prevPoints[i])
					continue;

				//curve.Points[i].curve = curve;
				//curve.Points[i].AddHandleObj();
			}
		}

		prevPoints = (BezierPoint[])curve.Points.Clone();
	}

	void OnSceneGUI()
	{
		for(int i = 0; i < curve.pointCount; i++)
		{
			if (curve[i] == null)
			{
				Debug.LogError(curve.name + " curve's " + "some point is null.");
				curve.gameObject.SetActive(false);
				continue;
			}
			DrawPointSceneGUI(curve[i]);
		}
	}
	
	void DrawPointInspector(BezierPoint point, int index)
	{
		if (point == null)
			return;

		SerializedObject serObj = new SerializedObject(point);
		
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("X", GUILayout.Width(20)))
		{
			Undo.RegisterSceneUndo("Remove Point");
			pointsProp.MoveArrayElement(curve.GetPointIndex(point), curve.pointCount - 1);
			pointsProp.arraySize--;
			//curve.Points[index].handle.c;
			DestroyImmediate(point.gameObject);
			return;
		}
		
		EditorGUILayout.ObjectField(point.gameObject, typeof(GameObject), true);
		
		if(index != 0 && GUILayout.Button(@"/\", GUILayout.Width(25)))
		{
			UnityEngine.Object other = pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue;
			pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue = point;
			pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
		}
		
		if(index != curve.pointCount - 1 && GUILayout.Button(@"\/", GUILayout.Width(25)))
		{
			UnityEngine.Object other = pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue;
			pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue = point;
			pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
		}
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUI.indentLevel++;
		
		Vector3 newPointPos = EditorGUILayout.Vector3Field("Position : ", point.transform.localPosition);
		if(newPointPos != point.transform.localPosition)
		{
			Undo.RegisterUndo(point.transform, "Move Bezier Point");
			point.transform.localPosition = newPointPos;
		}
		
		EditorGUI.indentLevel--;
		
		if(GUI.changed)
		{
			serObj.ApplyModifiedProperties();
			EditorUtility.SetDirty(serObj.targetObject);
		}
	}
	
	static void DrawPointSceneGUI(BezierPoint point)
	{
		if (point == null)
		{
			Debug.LogError("some point is null.");
			return;
		}

		Handles.Label(point.position + new Vector3(0, HandleUtility.GetHandleSize(point.position) * 0.4f, 0), point.gameObject.name);
		
		Handles.color = Color.green;
		Vector3 newPosition = Handles.FreeMoveHandle(point.position, point.transform.rotation, HandleUtility.GetHandleSize(point.position)*0.1f, Vector3.zero, Handles.RectangleCap);
		
		if(newPosition != point.position)
		{
			Undo.RegisterUndo(point.transform, "Move Point");
			point.transform.position = newPosition;
		}

		if (point.Handles != null)
		{
			foreach (BezierHandle handle in point.Handles)
			{
				DrawHandleSceneGUI(handle, point);
			}
		}
	}

	public static void DrawHandleSceneGUI(BezierHandle handle, BezierPoint point)
	{
		if (handle == null || point == null)
			return;

		Handles.color = Color.cyan;
		Vector3 newGlobal1 = Handles.FreeMoveHandle(handle.globalHandle1, Quaternion.Euler(Vector3.zero), HandleUtility.GetHandleSize(handle.globalHandle1) * 0.075f, Vector3.zero, Handles.CircleCap);
		if (handle.globalHandle1 != newGlobal1)
		{
			Undo.RegisterUndo(handle, "Move Handle 1");
			handle.globalHandle1 = newGlobal1;
		}

		Vector3 newGlobal2 = Handles.FreeMoveHandle(handle.globalHandle2, Quaternion.Euler(Vector3.zero), HandleUtility.GetHandleSize(handle.globalHandle2) * 0.075f, Vector3.zero, Handles.CircleCap);
		if (handle.globalHandle2 != newGlobal2)
		{
			Undo.RegisterUndo(handle, "Move Handle 2");
			handle.globalHandle2 = newGlobal2;
		}

		Handles.color = Color.yellow;
		Handles.DrawLine(point.position, handle.globalHandle1);
		Handles.DrawLine(point.position, handle.globalHandle2);
	}

	public static void DrawOtherPoints(BezierCurve curve, BezierPoint caller)
	{
		if (curve == null)
			return;

		foreach(BezierPoint p in curve.GetAnchorPoints())
		{
			if (p == null)
			{
				Debug.LogError(curve.name + " curve's " + "some point is null.");
				curve.gameObject.SetActive(false);
				continue;
			}

			DrawPointSceneGUI(p);
		}
	}

	public static void DrawOtherHandles(BezierPoint point, BezierHandle caller)
	{
		foreach (BezierHandle h in point.GetAnchorHandles())
		{
			if (h != caller) DrawHandleSceneGUI(h, point);
		}
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		//throw new NotImplementedException();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		//throw new NotImplementedException();
	}
}
