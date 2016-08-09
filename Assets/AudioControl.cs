using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System;

public class AudioControl : MonoBehaviour {
	public AudioMixerGroup masterGroup;

	public void SetsfxVolume (float volume) {
		volume = ChangeSliderValueToDecibel(volume);
		masterGroup.audioMixer.SetFloat("sfxVolume", volume);
	}

	public void SetMusicVolume(float volume)
	{
		volume = ChangeSliderValueToDecibel(volume);
		masterGroup.audioMixer.SetFloat("MusicVolume", volume);
	}

	public void SetMasterVolume(bool isMute)
	{
		float volume = isMute ? 0f : 1f;
		volume = ChangeSliderValueToDecibel(volume);
		masterGroup.audioMixer.SetFloat("MasterVolume", volume);
	}

	private float ChangeSliderValueToDecibel(float volume)
	{
		return volume * 90.0f - 80.0f;
	}
}
