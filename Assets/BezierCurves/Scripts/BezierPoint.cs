#region UsingStatements

using UnityEngine;
using System;
using System.Collections.Generic;
using ToBeFree;

#endregion

/// <summary>
/// 	- Helper class for storing and manipulating Bezier Point data
/// 	- Ensures that handles are in correct relation to one another
/// 	- Handles adding/removing self from curve point lists
/// 	- Calls SetDirty() on curve when edited 
/// </summary>
[ExecuteInEditMode]
[Serializable] 
public class BezierPoint : MonoBehaviour{

	private IconCity iconCity;
	public IconCity IconCity
	{
		get { return this.iconCity; }
		set { this.iconCity = value; }
	}
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BezierCurve"/> is dirty.
	/// </summary>
	/// <value>
	/// <c>true</c> if dirty; otherwise, <c>false</c>.
	/// </value>
	public bool dirty { get; private set; }


	#region PublicProperties

	/// <summary>
	///		- Curve this point belongs to
	/// 	- Changing this value will automatically remove this point from the current curve and add it to the new one
	/// </summary>
	[SerializeField]
	private BezierCurve _curve;
	public BezierCurve curve
	{
		get{return _curve;}
		set
		{
			if(_curve) _curve.RemovePoint(this);
			_curve = value;
			_curve.AddPoint(this);
		}
	}

	
	
	/// <summary>
	/// 	- Shortcut to transform.position
	/// </summary>
	/// <value>
	/// 	- The point's world position
	/// </value>
	public Vector3 position
	{
		get { return transform.position; }
		set { transform.position = value; }
	}
	
	/// <summary>
	/// 	- Shortcut to transform.localPosition
	/// </summary>
	/// <value>
	/// 	- The point's local position.
	/// </value>
	public Vector3 localPosition
	{
		get { return transform.localPosition; }
		set { transform.localPosition = value; }
	}

	[SerializeField]
	private BezierHandle[] handles;
	public BezierHandle[] Handles
	{
		get { return handles; }
		set
		{
			if (handles == value) return;
			handles = value;
			_curve.SetDirty();
		}
	}

	#endregion

	/// <summary>
	///		- set internally
	///		- gets point corresponding to "index" in "points" array
	///		- does not allow direct set
	/// </summary>
	/// <param name='index'>
	/// 	- the index
	/// </param>
	public BezierHandle this[int index]
	{
		get { return Handles[index]; }
	}

	/// <summary>
	/// 	- number of points stored in 'points' variable
	///		- set internally
	///		- does not include "handles"
	/// </summary>
	/// <value>
	/// 	- The point count
	/// </value>
	public int handleCount
	{
		get { return Handles.Length; }
	}

	/// <summary>
	/// 	- Get the index of the given point in this curve
	/// </summary>
	/// <returns>
	/// 	- The index, or -1 if the point is not found
	/// </returns>
	/// <param name='point'>
	/// 	- Point to search for
	/// </param>
	public int GetHandleIndex(BezierHandle point)
	{
		int result = -1;
		for (int i = 0; i < Handles.Length; i++)
		{
			if (Handles[i] == point)
			{
				result = i;
				break;
			}
		}

		return result;
	}

	/// <summary>
	/// 	- Gets a copy of the bezier point array used to define this curve
	/// </summary>
	/// <returns>
	/// 	- The cloned array of points
	/// </returns>
	public BezierHandle[] GetAnchorHandles()
	{
		return (BezierHandle[])Handles.Clone();
	}

	/// <summary>
	/// 	- Removes the given point from the curve ("points" array)
	/// </summary>
	/// <param name='handle'>
	/// 	- The point to remove
	/// </param>
	public void RemoveHandle(BezierHandle handle)
	{
		List<BezierHandle> tempArray = new List<BezierHandle>(this.Handles);
		tempArray.Remove(handle);
		this.Handles = tempArray.ToArray();
		dirty = false;
	}

	/// <summary>
	/// 	- Adds the given point to the end of the curve ("points" array)
	/// </summary>
	/// <param name='handle'>
	/// 	- The point to add.
	/// </param>
	public void AddHandle(BezierHandle handle)
	{
		List<BezierHandle> tempArray = new List<BezierHandle>(this.Handles);
		tempArray.Add(handle);
		this.Handles = tempArray.ToArray();
		dirty = true;
	}

	public void AddHandleObj()
	{
		GameObject handleObject = new GameObject("Handle");
		handleObject.transform.parent = this.transform;
		handleObject.transform.localPosition = Vector3.zero;
		BezierHandle newHandle = handleObject.AddComponent<BezierHandle>();
		newHandle.point = this;
		newHandle.curve = this.curve;
		//this.AddHandle(newHandle);
	}

	#region PrivateVariables

	/// <summary>
	/// 	- Used to determine if this point has moved since the last frame
	/// </summary>
	private Vector3 lastPosition;
	
	#endregion
	
	#region MonoBehaviourFunctions
	
	void Update()
	{
		if(curve == null)
		{
			this.enabled = false;
			return;
		}

		if(!_curve.dirty && transform.position != lastPosition)
		{
			_curve.SetDirty();
			lastPosition = transform.position;
		}
	}

	#endregion
}
