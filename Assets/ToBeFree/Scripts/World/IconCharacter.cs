using UnityEngine;

namespace ToBeFree
{
	public class IconCharacter : MonoBehaviour
	{		
		public void MoveCity(IconCity iconCity)
		{
			this.transform.position = iconCity.characterOffset.position;
			//NGUIDebug.Log("Moved to " + iconCity.name);
		}
	}
}