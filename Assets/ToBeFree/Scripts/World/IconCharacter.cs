using UnityEngine;

namespace ToBeFree
{
	public class IconCharacter : MonoBehaviour
	{		
		public void MoveCity(IconCity iconCity)
		{
			this.transform.position = iconCity.transform.position;
			NGUIDebug.Log("Moved to " + iconCity.name);
		}
	}
}