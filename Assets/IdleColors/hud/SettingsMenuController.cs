using IdleColors.camera;
using IdleColors.Globals;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace IdleColors.hud
{
    public class SettingsMenuController : MonoBehaviour
    {
        [SerializeField] private Canvas menu;
        [SerializeField] private GameObject startMenu;
        [SerializeField] private GameObject inGamePanel;
        [SerializeField] private GameObject BgButton;
        [SerializeField] private Slider soundSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider lightSlider;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private GameObject _saveHint;
        
        private bool _status;
        private readonly string musicVolume = "music_volume";
        private readonly string sfxVolume = "sfx_volume";

        private void Start()
        {
            _saveHint.SetActive(false);
            menu.enabled = false;
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
            lightSlider.onValueChanged.AddListener(OnLightSliderChanged);

            if (PlayerPrefs.HasKey(musicVolume))
            {
                musicSlider.value = PlayerPrefs.GetFloat(musicVolume);
                mixer.SetFloat(musicVolume, PlayerPrefs.GetFloat(musicVolume));
            }
            else
            {
                mixer.SetFloat(musicVolume, -40f);
            }


            if (PlayerPrefs.HasKey(sfxVolume))
            {
                soundSlider.value = PlayerPrefs.GetFloat(sfxVolume);
                mixer.SetFloat(sfxVolume, PlayerPrefs.GetFloat(sfxVolume));
            }
            else
            {
                mixer.SetFloat(sfxVolume, -20f);
            }
        }

        public void OnMouseUp()
        {
            BgButton.SetActive(!BgButton.activeSelf);
            if (GameManager.Instance.ReadyToSave)
            {
                Time.timeScale = _status ? 1 : 0;
                menu.enabled = !_status;
                _status = !_status;
            }
        }

        private void OnMusicSliderChanged(float value)
        {
            PlayerPrefs.SetFloat(musicVolume, value);
            PlayerPrefs.Save();
            mixer.SetFloat(musicVolume, value);
        }

        private void OnSoundSliderChanged(float value)
        {
            PlayerPrefs.SetFloat(sfxVolume, value);
            PlayerPrefs.Save();
            mixer.SetFloat(sfxVolume, value);
        }

        private void OnLightSliderChanged(float value)
        {
            CameraController.Instance.SetLightIntensity(value);
        }

        public void ContinueGame()
        {
            GameManager.Instance.LoadGameData();
            ActivateIngameMenue();
        }

        public void ExitGame()
        {
            _saveHint.SetActive(true);
            GameManager.Instance.SaveGameData();
            // if (Application.platform == RuntimePlatform.Android)
            // {
            //     var activity =
            //         new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>(
            //             "currentActivity");
            //     activity.Call<bool>("moveTaskToBack", true);
            // }
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void ResetGame()
        {
            GameManager.Instance.ResetValues();
            ActivateIngameMenue();
        }

        private void ActivateIngameMenue()
        {
            inGamePanel.SetActive(true);
            startMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}