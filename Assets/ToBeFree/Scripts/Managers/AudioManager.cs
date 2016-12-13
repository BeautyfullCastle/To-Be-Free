using System;
using UnityEngine;
using SerializableCollections;

namespace ToBeFree
{
	[ExecuteInEditMode]
	public class AudioManager : MonoSingleton<AudioManager>
	{
		[SerializeField]
		private AudioSerializableDictionary dic;

		private AudioManager() { }

		void Awake()
		{
			AudioManager m = AudioManager.Instance;
			Init();
		}

#if UNITY_EDITOR
		void Update()
		{
			if(Application.isPlaying==false)
				Init();
		}
#endif

		public AudioSource Find(string keyName)
		{
			foreach (string key in dic.Keys)
			{
				if (key == keyName)
					return dic[key];
			}
			return null;
		}

		private void Init()
		{
			if (dic != null)
				return;

			dic = new AudioSerializableDictionary();
			AudioSource[] sources = this.GetComponentsInChildren<AudioSource>();
			foreach(AudioSource source in sources)
			{
				dic.Add(source.name, source);
			}
			
			Debug.Log("AudioManager Init()");
		}
	}

	[Serializable]
	public class AudioSerializableDictionary : SerializableDictionary<string, AudioSource>
	{
	}

#if UNITY_EDITOR

	[UnityEditor.CustomPropertyDrawer(typeof(AudioSerializableDictionary))]
	public class ExtendedSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
	{
	}

#endif
}
