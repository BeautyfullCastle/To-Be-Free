using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour {
	[SerializeField]
	private AudioMixerGroup masterGroup;
	[SerializeField]
	private UISlider musicVolumeSlider;
	[SerializeField]
	private UISlider sfxVolumeSlider;
	[SerializeField]
	private UIToggle muteCheckbox;

	void Start()
	{
		if(musicVolumeSlider)
		{
			musicVolumeSlider.value = 0.8f;
			SetMusicVolume(musicVolumeSlider.value);
		}
		if(sfxVolumeSlider)
		{
			sfxVolumeSlider.value = 0.8f;
			SetsfxVolume(sfxVolumeSlider.value);
		}
		if(muteCheckbox)
		{
			muteCheckbox.value = false;
		}
		
		this.gameObject.SetActive(false);
	}

	public void SetsfxVolume (float volume)
	{
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
		return volume * 90.0f - 90.0f;
	}
}
