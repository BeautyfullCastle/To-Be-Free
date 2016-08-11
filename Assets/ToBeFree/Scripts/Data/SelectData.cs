using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



[Serializable]
public class SelectData : IData
{
    public int index;
    public string subjectType;
    public string objectType;
    public string comparisonOperator;
    public int compareAmount;
    public string script;
    public string linkType;
    public int linkIndex;
}