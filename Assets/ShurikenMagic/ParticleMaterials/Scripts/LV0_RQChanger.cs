//RenderQueue_Editor Ver 1.00 by 순순.
//This source distributed by LEVEL 0 (http://cafe.naver.com/level0)
//Copyright© All Rights Reserved

//파티클의 렌더큐를 변경하는 스크립트입니다.
//메터리얼을 직접 수동으로 끌어넣어줘야합니다.
//이 스크립트를 이용하여, 메터리얼의 렌더큐를 수정할 수 있습니다.
//파티클과 UI가 겹치는 문제가 발생할때 사용 가능합니다.

// 3D Text 에서도 사용할 수 있도록 추가

using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class LV0_RQChanger : MonoBehaviour
{
	public Material _mat; // 파티클 컴퍼넌트에 적용되는 메터리얼. 수동으로 끌어다 넣어줘야함.
	public int _renderQueue; // 수정을 할 렌더큐값, NGUI 기준 기본 3000, 즉 NGUI 에서 사용하려면 3000 이상을 넣어줘야합니다.
	public MeshRenderer _meshRender; // 3D 텍스트를 위하여 Mesh Render 수정.

	void Update()
	{
		if (_meshRender != null) _mat = _meshRender.materials[0];
		if (_mat != null) //메터리얼이 정의안되어있으면 실행안됨.
		{
				_mat.renderQueue = _renderQueue; // 렌더큐값 업데이트.

				Destroy(this);
		}
	}
}
