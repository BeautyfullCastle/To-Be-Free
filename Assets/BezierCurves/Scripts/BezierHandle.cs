using UnityEngine;
using System.Collections;

[System.Serializable]
public class BezierHandle : MonoBehaviour
{
	/// <summary>
	///		- Curve this point belongs to
	/// 	- Changing this value will automatically remove this point from the current curve and add it to the new one
	/// </summary>
	[SerializeField]
	private BezierPoint _point;
	public BezierPoint point
	{
		get { return _point; }
		set
		{
			if (_point)
				_point.RemoveHandle(this);

			_point = value;
			_point.AddHandle(this);
		}
	}

	/// <summary>
	///		- Curve this point belongs to
	/// </summary>
	[SerializeField]
	private BezierCurve _curve;
	public BezierCurve curve
	{
		get { return _curve; }
		set
		{
<<<<<<< HEAD
            if (_curve)
                _curve.SetDirty();
            _curve = value;
            if (_curve)
                _curve.SetDirty();
        }
=======
			_curve = value;
		}
>>>>>>> 175d4de0bdeeceaab19e9805181feb089cc2b45b
	}

	/// <summary>
	/// 	- Local position of the first handle
	/// 	- Setting this value will cause the curve to become dirty
	/// 	- This handle effects the curve generated from this point and the point proceeding it in curve.points
	/// </summary>
	[SerializeField]
	private Vector3 _handle1;
	public Vector3 handle1
	{
		get { return _handle1; }
		set
		{
			if (_handle1 == value) return;
			_handle1 = value;

			if (this.curve)
				this.curve.SetDirty();
		}
	}

	/// <summary>
	///		- Global position of the first handle
	///		- Ultimately stored in the 'handle1' variable
	/// 	- Setting this value will cause the curve to become dirty
	/// 	- This handle effects the curve generated from this point and the point proceeding it in curve.points
	/// </summary>
	public Vector3 globalHandle1
	{
		get { return transform.TransformPoint(handle1); }
		set { handle1 = transform.InverseTransformPoint(value); }
	}

	/// <summary>
	/// 	- Local position of the second handle
	///  	- Setting this value will cause the curve to become dirty
	///		- This handle effects the curve generated from this point and the point coming after it in curve.points
	/// </summary>
	[SerializeField]
	private Vector3 _handle2;
	public Vector3 handle2
	{
		get { return _handle2; }
		set
		{
			if (_handle2 == value) return;
			_handle2 = value;

			if (this.curve)
				this.curve.SetDirty();
		}
	}

	/// <summary>
	///		- Global position of the second handle
	///		- Ultimately stored in the 'handle2' variable
	///		- Setting this value will cause the curve to become dirty
	///		- This handle effects the curve generated from this point and the point coming after it in curve.points 
	/// </summary>
	public Vector3 globalHandle2
	{
		get { return transform.TransformPoint(handle2); }
		set { handle2 = transform.InverseTransformPoint(value); }
	}
}
