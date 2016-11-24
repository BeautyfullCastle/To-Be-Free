using UnityEngine;

public class NGUIEventRegister : Singleton<NGUIEventRegister>
{
	// 이벤트 동적할당 함수
	public void AddOnClickEvent(MonoBehaviour target, UIButton btn, string method, EventDelegate.Parameter[] parameters)
	{
		EventDelegate onClickEvent = new EventDelegate(target, method);

		for (int i = 0; i < parameters.Length; ++i)
		{
			onClickEvent.parameters[i] = parameters[i];
		}
		EventDelegate.Add(btn.onClick, onClickEvent);
	}
}
