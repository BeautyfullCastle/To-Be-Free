/// <summary>
/// DupWithoutRename is a utility class for Unity 5 that mimics the Unity 4 behavior of Ctrl+D (or Cmd+D on Mac): objects are duplicated 
/// without being renamed. In Unity 5 they are renamed automatically, and it sucks. Place this file in Assets/Editor so that it is 
/// detected as an editor script.
/// 
/// Note that unlike the real Ctrl+D command, this one only duplicates objects in the scene hierarchy.
/// It can't duplicate other stuff, like assets in your Project database, or array entries in the editor's stock array manager GUI. 
/// For this reason, it's mapped to Ctrl+Shift+D, so you still have access to Ctrl+D for those other tasks.
/// 
/// v1.01. 
/// Code by Eric Heimburg. 
/// Public domain. This is free and unencumbered software released into the public domain. See unlicense.org for details.
/// 
/// v1.10.
/// Edit by Hyunsoo Kim(hyunsookim@devarc.com).
/// Solved kown issue on the below.
/// Changed Hotkey from Ctrl + Shift + D to Ctrl + D(Origianl duplicate function, available replace Hotkey in my computer).
/// </summary>

// Known issue!!! While this function will duplicate selected prefabs correctly, if you duplicate a NON-prefab game object that has child objects
// that ARE prefabs, the duplicated children won't be correctly marked as prefabs. This scenario doesn't seem to occur in practice for me,
// which is good because I don't have time to try to fix it at the moment. If somebody fixes it, though, please email me! eric@heimburg.com

using UnityEngine;
using UnityEditor;

public class DupWithoutRename
{
	[MenuItem("GameObject/Duplicate Without Renaming %d")]
	public static void DuplicateWithoutRenaming()
	{
		Object[] originObjs = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);
		
		EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));

		Object[] newObjs = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable);
		for(int i=0; i < newObjs.Length; ++i)
		{
			newObjs[i].name = originObjs[i].name;
		}
	}
	
	
	[MenuItem("GameObject/Duplicate Without Renaming %d", true)]
	public static bool ValidateDuplicateWithoutRenaming()
	{
		return Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab | SelectionMode.Editable).Length > 0;
	}

}
