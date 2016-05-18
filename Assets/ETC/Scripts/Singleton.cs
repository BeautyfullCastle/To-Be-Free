using System;

public class Singleton<T> where T : class, new()
{
	public static T Instance
	{
		get;
		private set;
	}

	static Singleton()
	{
		if (Singleton<T>.Instance == null)
		{
			Singleton<T>.Instance = new T();
		}
	}

	public virtual void Clear()
	{
		Singleton<T>.Instance = null;	
		Singleton<T>.Instance = new T();
	}
}