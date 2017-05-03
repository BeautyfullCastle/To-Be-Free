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
		[SerializeField]
		private AudioSource bgmAudio;
		private AudioSource prevBgmAudio;

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
			Debug.LogError("Can't find '" + keyName + "' audio source.");
			return null;
		}

		internal void ChangeBGM(object audioName)
		{
			throw new NotImplementedException();
		}

		public void ChangeBGM(string keyName)
		{
			AudioSource foundAudio = Find(keyName);
			if (foundAudio == bgmAudio)
			{
				return;
			}

			if(bgmAudio)
			{
				bgmAudio.Stop();
			}

			if (keyName == "Main" || keyName == "Crackdown")
			{
				if (bgmAudio.gameObject.name != "Detention" && bgmAudio.gameObject.name != "Camp")
				{
					bgmAudio = foundAudio;
				}
				prevBgmAudio = foundAudio;
			}
			else
			{
				bgmAudio = foundAudio;
			}
			
			if(bgmAudio)
			{
				//if(bgmAudio != prevBgmAudio)
				{
					bgmAudio.Play();
				}
			}
		}

		public void ChangeToPrevBGM()
		{
			if(bgmAudio)
			{
				bgmAudio.Stop();
			}
			if(prevBgmAudio)
			{
				bgmAudio = prevBgmAudio;
				bgmAudio.Play();
			}
		}

		private void Init()
		{
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
