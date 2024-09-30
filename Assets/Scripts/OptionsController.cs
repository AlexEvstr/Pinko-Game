using UnityEngine;

public class OptionsController : MonoBehaviour
{
    public AudioSource _music;
    public AudioSource _sounds;
    public GameObject _musicOn;
    public GameObject _musicOff;
    public GameObject _soundsOn;
    public GameObject _soundsOff;
    public GameObject _vibrationOn;
    public GameObject _vibrationOff;
    public AudioClip _clickSound;
    public AudioClip _onSwitchSound;
    public AudioClip _offSwitchSound;
    private HapticFeedbackManager _hapticFeedbackManager;
    private int _vibration;
    [SerializeField] private GameObject _backBtn;
    [SerializeField] private GameObject _saveBtn;

    private void Start()
    {
        _hapticFeedbackManager = GetComponent<HapticFeedbackManager>();
        _music.volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        if (_music.volume == 1) OnMusic();
        else OffMusic();

        _sounds.volume = PlayerPrefs.GetFloat("SoundsVolume", 1);
        if (_sounds.volume == 1) OnSounds();
        else OffSounds();

        _vibration = PlayerPrefs.GetInt("IsVibrationActive", 1);
        if (_vibration == 1) VibrationOn();
        else VibrationOff();
    }

    private void ShowSaveBtn()
    {
        _backBtn.SetActive(false);
        _saveBtn.SetActive(true);
    }

    public void HideShowBtn()
    {
        _saveBtn.SetActive(false);
        _backBtn.SetActive(true);
    }

    public void OffMusic()
    {
        ShowSaveBtn();
        _musicOn.SetActive(false);
        _musicOff.SetActive(true);
        _music.volume = 0;
        PlayerPrefs.SetFloat("MusicVolume", 0);
    }

    public void OnMusic()
    {
        ShowSaveBtn();
        _musicOff.SetActive(false);
        _musicOn.SetActive(true);
        _music.volume = 1;
        PlayerPrefs.SetFloat("MusicVolume", 1);
    }

    public void OffSounds()
    {
        ShowSaveBtn();
        _soundsOn.SetActive(false);
        _soundsOff.SetActive(true);
        _sounds.volume = 0;
        PlayerPrefs.SetFloat("SoundsVolume", 0);
    }

    public void OnSounds()
    {
        ShowSaveBtn();
        _soundsOff.SetActive(false);
        _soundsOn.SetActive(true);
        _sounds.volume = 1;
        PlayerPrefs.SetFloat("SoundsVolume", 1);
    }

    public void VibrationOff()
    {
        ShowSaveBtn();
        _vibrationOn.SetActive(false);
        _vibrationOff.SetActive(true);
        _vibration = 0;
        PlayerPrefs.SetInt("IsVibrationActive", 0);
    }

    public void VibrationOn()
    {
        ShowSaveBtn();
        _vibrationOff.SetActive(false);
        _vibrationOn.SetActive(true);
        _vibration = 1;
        PlayerPrefs.SetInt("IsVibrationActive", 1);
    }

    public void ClickSound()
    {
        _sounds.PlayOneShot(_clickSound);
    }

    public void OnSwithSound()
    {
        _sounds.PlayOneShot(_onSwitchSound);
    }

    public void OffSwithSound()
    {
        _sounds.PlayOneShot(_offSwitchSound);
    }

    public void TryLightHaptic()
    {
        if (_vibration == 1) _hapticFeedbackManager.LightHaptic();
    }

    public void TryMediumHaptic()
    {
        if (_vibration == 1) _hapticFeedbackManager.MediumHaptic();
    }

    public void TryHeavyHaptic()
    {
        if (_vibration == 1) _hapticFeedbackManager.HeavyHaptic();
    }
}