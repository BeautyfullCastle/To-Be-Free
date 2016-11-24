using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour {
	public AudioMixerGroup masterGroup;

	void Start()
	{
		SetsfxVolume(.5f);
		SetMusicVolume(.5f);
		SetMasterVolume(false);
		this.gameObject.SetActive(false);
	}

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
