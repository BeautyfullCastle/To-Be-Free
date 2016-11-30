using UnityEngine;
using System.Collections.Generic;

namespace ToBeFree
{
	public class UIBuffManager : MonoBehaviour
	{
		private List<UIBuff> buffs = new List<UIBuff>();
		// 우리가 만든 SampleItem을 복사해서 만들기 위해 선언합니다.
		public GameObject objSampleItem;
		// 그리드를 reset position 하기위해 선언합니다.
		public UIGrid grid;

		void Awake()
		{
			BuffManager.AddedBuff += AddBuff;
			BuffManager.DeletedBuff += DeleteBuff;
		}

		void OnDisable()
		{
			foreach(UIBuff buff in buffs)
			{
				if (buff.gameObject == null)
					continue;
				DestroyImmediate(buff.gameObject);
			}
			buffs.Clear();
			grid.Reposition();
		}

		public void AddBuff(Buff buff)
		{
			GameObject gObjItem = NGUITools.AddChild(grid.gameObject, objSampleItem);
			// 이제 이름과 아이콘을 세팅할께요.
			// 그럴려면 먼저 아까 만든 ItemScript를 가져와야겠죠.
			// GetComponent는 해당 게임 오브젝트가 가지고 있는 컴포넌트를 가져오는 역할을 해요.
			UIBuff uiBuff = gObjItem.GetComponent<UIBuff>();
			uiBuff.SetInfo(buff);
			// 이제 그리드와 스크롤뷰를 재정렬 시킵시다.
			grid.Reposition();
			
			// 그리고 관리를 위해 만든걸 리스트에 넣어둡시다.
			buffs.Add(uiBuff);
		}

		public void DeleteBuff(Buff buff)
		{
			UIBuff uiBuff = buffs.Find(x => x.buffName.text == buff.Name);
			if(uiBuff == null)
			{
				return;
			}

			DestroyImmediate(uiBuff.gameObject);
			buffs.Remove(uiBuff);
			grid.Reposition();
		}
	}
}