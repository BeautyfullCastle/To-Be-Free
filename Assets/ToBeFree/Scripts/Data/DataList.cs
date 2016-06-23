using System;
using System.IO;
using UnityEngine;


public interface IData
{
}

public class DataList<T> where T : IData
{
    public T[] dataList;

    public DataList(string file)
    {
        StreamReader reader = new StreamReader(file);
        string json = reader.ReadToEnd();

        var dataList = JsonUtility.FromJson<DataList<T>>(json);
        this.dataList = dataList.dataList;
    }
}