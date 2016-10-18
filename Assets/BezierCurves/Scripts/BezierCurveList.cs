using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
//[ExecuteInEditMode]
public class BezierCurveList : MonoBehaviour {

	public BezierPoint point1;
	public BezierPoint point2;

	[SerializeField]
	private List<BezierCurve> list = new List<BezierCurve>();

	[SerializeField]
	private Dictionary<BezierPoint, List<BezierPoint>> dic = new Dictionary<BezierPoint, List<BezierPoint>>();

	public void Init()
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
			//Debug.LogWarning(s);
		}
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
		//Debug.Log(s);
	}

	public List<BezierPoint> GetPath(BezierPoint p1, BezierPoint p2)
	{
		if (p1 == p2)
			return null;

		if(dic == null || dic.Count == 0)
		{
			Init();
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

		if (path.Contains(p1))
		{
			path.Remove(p1);
		}

		path.Reverse();
		return path;
	}

	public List<BezierCurve> List
	{
		get
		{
			return list;
		}
	}

	public Dictionary<BezierPoint, List<BezierPoint>> Dic
	{
		get
		{
			return dic;
		}
	}

	public List<BezierCurve> FindCurves(BezierPoint bezierPoint)
	{
		return list.FindAll(curve => Array.Exists<BezierPoint>(curve.points, point => point == bezierPoint));
	}
}
