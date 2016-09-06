using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
[ExecuteInEditMode]
public class BezierCurveList : MonoBehaviour {

	public BezierPoint point1;
	public BezierPoint point2;

	[SerializeField]
	private List<BezierCurve> list = new List<BezierCurve>();

	[SerializeField]
	private Dictionary<BezierPoint, List<BezierPoint>> dic = new Dictionary<BezierPoint, List<BezierPoint>>();

	void InitDic()
	{
		dic.Clear();
		foreach (BezierCurve curve in list)
		{
			foreach (BezierPoint point in curve.points)
			{
				if (dic.ContainsKey(point))
				{
					continue;
				}
				dic.Add(point, new List<BezierPoint>());
			}
		}

		foreach (BezierCurve curve in list)
		{
			foreach (BezierPoint point in curve.points)
			{
				int i = curve.GetPointIndex(point);
				int lastI = curve.points.Length - 1;

				BezierPoint leftP = null;
				BezierPoint rightP = null;
				
				if (i == 0)
				{
					if (curve.close)
						leftP = curve[lastI];

					rightP = curve[i + 1];
				}
				else if(i == lastI)
				{
					leftP = curve[i - 1];
					if (curve.close)
						rightP = curve[0];
				}
				else
				{
					leftP = curve[i - 1];
					rightP = curve[i + 1];
				}

				if(leftP!=null && dic[point].Contains(leftP)==false)
				{
					dic[point].Add(leftP);
				}
				if(rightP!=null && dic[point].Contains(rightP)==false)
				{
					dic[point].Add(rightP);
				}
			}
		}

		foreach(List<BezierPoint> list in dic.Values)
		{
			string s = string.Empty;
			foreach(BezierPoint p in list)
			{
				s += p.gameObject.name + ", ";
			}
			Debug.LogWarning(s);
		}
	}
	void Awake()
	{
		foreach (BezierCurve curve in GetComponentsInChildren<BezierCurve>())
		{
			if (list.Contains(curve))
			{
				continue;
			}
			list.Add(curve);
		}
		InitDic();
	}

	// Update is called once per frame
	void Update () {
		//Awake();
	}

	void OnDrawGizmos()
	{
		if (this.enabled == false)
			return;

		Gizmos.color = Color.red;
		List<BezierPoint> points = GetPath(point1, point2);
		string s = string.Empty;
		for(int i=0; i<points.Count-1; ++i)
		{
			//Gizmos.color = new Color(i * 0.1f, 0f, 0f);
			Gizmos.DrawLine(points[i].position, points[i + 1].position);
			Gizmos.DrawSphere(points[i].transform.position, 0.05f);
			s += points[i].gameObject.name + ", ";
		}
		Gizmos.DrawSphere(points[points.Count-1].transform.position, 0.05f);
		s += points[points.Count - 1].gameObject.name;
		Debug.Log(s);
	}

	public List<BezierPoint> GetPath(BezierPoint p1, BezierPoint p2)
	{
		if (p1 == p2)
			return null;

		if(dic == null || dic.Count == 0)
		{
			Awake();
		}

		Queue<BezierPoint> queue = new Queue<BezierPoint>();
		Dictionary<BezierPoint, BezierPoint> parentDic = new Dictionary<BezierPoint, BezierPoint>();

		queue.Enqueue(p1);
		parentDic.Add(p1, p1);

		bool findSame = true;
		while (findSame)
		{
			BezierPoint parent = queue.Dequeue();
			foreach (BezierPoint p in dic[parent])
			{
				if (parentDic.ContainsKey(p))
					continue;

				queue.Enqueue(p);
				parentDic.Add(p, parent);

				if (p == p2)
				{
					findSame = false;
					break;
				}
			}
		}

		List<BezierPoint> path = new List<BezierPoint>();
		path.Add(p2);

		BezierPoint point = parentDic[p2];
		path.Add(point);
		
		while (point != p1)
		{
			point = parentDic[point];
			path.Add(point);
		}

		path.Reverse();
		return path;

	}

	#region curve get path
	/*
	public List<BezierPoint> GetPath(BezierPoint p1, BezierPoint p2)
	{
		// find curve list contains p1, p2
		List<BezierCurve> p1Curves = list.FindAll(curve => Array.Exists<BezierPoint>(curve.points, point => point == p1));
		if(p1Curves.Count == 0)
		{
			Debug.LogError("p1Curves Count is 0");
		}
		else
		{
			Debug.Log("p1Curves count : " + p1Curves.Count + "points count : " + p1.curve.pointCount);
		}
		List<BezierCurve> p2Curves = list.FindAll(curve => Array.Exists<BezierPoint>(curve.points, point => point == p2));

		List<BezierCurve> curves = new List<BezierCurve>(p1Curves);
		curves.AddRange(p2Curves);

		// find duplicate points
		List<BezierPoint> points = new List<BezierPoint>();
		List<BezierPoint> duplicatePoints = new List<BezierPoint>();

		BezierCurve duplicateCurve = null;
		foreach(BezierCurve p1Curve in p1Curves)
		{
			foreach(BezierCurve p2Curve in p2Curves)
			{
				if(p1Curve == p2Curve)
				{
					duplicateCurve = p2Curve;
					break;
				}
			}
		}

		foreach (BezierCurve curve in curves)
		{
			points.AddRange(curve.points);
		}
		for(int i=0; i< points.Count-1; ++i)
		{
			for (int j = 1; j < points.Count; ++j)
			{
				if(i == j)
				{
					continue;
				}
				if (points[i] == points[j])
				{
					if (duplicatePoints.Contains(points[j]))
					{
						continue;
					}
					duplicatePoints.Add(points[j]);
					Debug.Log("duple " + points[j].gameObject.ToString());
				}
				//Debug.Log("all " + points[j].ToString());
			}
		}

		List<BezierPoint> path = new List<BezierPoint>();

		if(duplicateCurve)
		{
			int p1Index = duplicateCurve.GetPointIndex(p1);
			int p2Index = duplicateCurve.GetPointIndex(p2);

			if (duplicateCurve.close)
			{
				bool p1IsBigger = false;
				if (p1Index > p2Index)
				{
					p1IsBigger = true;

					int temp = p1Index;
					p1Index = p2Index;
					p2Index = temp;
				}

				if ((p2Index - p1Index) <= (p1Index + duplicateCurve.points.Length - p2Index))
				{
					for (int i = p1Index; i <= p2Index; ++i)
					{
						path.Add(duplicateCurve[i]);
					}
				}
				else
				{
					int i = p2Index;
					while (true)
					{
						path.Add(duplicateCurve[i]);
						if (i == p1Index)
						{
							break;
						}
						if (i >= duplicateCurve.points.Length - 1)
						{
							i = 0;
						}
						else
						{
							++i;
						}
					}
				}

				if (p1IsBigger)
				{
					//path.Reverse();
					Debug.Log("Reversed");
				}
			}
			else
			{
				bool p1IsBigger = false;
				if (p1Index > p2Index)
				{
					p1IsBigger = true;

					int temp = p1Index;
					p1Index = p2Index;
					p2Index = temp;
				}

				for (int i=p1Index; i<=p2Index; ++i)
				{
					path.Add(duplicateCurve[i]);
				}

				if (p1IsBigger)
				{
					path.Reverse();
					Debug.Log("Reversed");
				}
			}
			return path;
		}

		// if p1 is duplicate duplicatePoints[i]
		if (duplicatePoints.Contains(p1))
		{
			for(int i=p2.curve.GetPointIndex(p1); i <= p2.curve.GetPointIndex(p2); ++i)
			{
				path.Add(p2.curve.points[i]);
			}
			return path;
		}
		// if more than two curves
		else {
			List<BezierPoint[]> pathList = new List<BezierPoint[]>();

			int start = 0, end = 0;
			for(int i=0; i < duplicatePoints.Count; ++i)
			{
				if (p1.curve.GetPointIndex(p1) < p1.curve.GetPointIndex(duplicatePoints[i]))
				{
					start = p1.curve.GetPointIndex(p1);
					end = p1.curve.GetPointIndex(duplicatePoints[i]);
				}
				else
				{
					end = p1.curve.GetPointIndex(p1);
					start = p1.curve.GetPointIndex(duplicatePoints[i]);
				}
				for (int j = start; j <= end; ++j)
				{
					path.Add(p1.curve.points[j]);
				}

				if (p2.curve.GetPointIndex(p2) < p2.curve.GetPointIndex(duplicatePoints[i]))
				{
					start = p2.curve.GetPointIndex(p2);
					end = p2.curve.GetPointIndex(duplicatePoints[i]);
				}
				else
				{
					end = p2.curve.GetPointIndex(p2);
					start = p2.curve.GetPointIndex(duplicatePoints[i])+1;
				}
				for (int j = start; j <= end; ++j)
				{
					path.Add(p2.curve.points[j]);
				}
				pathList.Add(path.ToArray());
				path.Clear();
			}
			int pathCount = 100;
			foreach (BezierPoint[] p in pathList)
			{
				if (p.Length < pathCount)
				{
					pathCount = p.Length;
					path = new List<BezierPoint>(p);
				}
			}
			return path;
		}

		return null;
	}
	*/
	#endregion
}
