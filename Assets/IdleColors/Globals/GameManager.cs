using System.Collections;
using System.Collections.Generic;
using IdleColors.hud.coin;
using IdleColors.room_collect.collector;
using IdleColors.room_mixing.haxler;
using IdleColors.room_mixing.mixer;
using IdleColors.room_mixing.puffer;
using IdleColors.ScriptableObjects;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace IdleColors.Globals
{
    public class GameManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener
    {
        public static GameManager Instance;

        #region members

        public GameObject cupBp;

        public Text coinsText;
        private readonly int _coinMultiplier = 5;
        public bool ReadyToSave { private set; get; }
        public const string REWARDED_ANDROID = "Rewarded_Android";
        private readonly string GAMEID = "5774375";
        public bool AdsRewardedLoaded { private set; get; }
        public int coins;
        [SerializeField] private AudioSource _coinAudioSource;

        #endregion

        #region SOs

        public SO_Bool so_unlockedRed;
        public SO_Int so_capacityRed;
        public SO_Int so_speedLevelRed;
        public SO_Int so_unloadSpeedRed;

        public SO_Bool so_unlockedGreen;
        public SO_Int so_capacityGreen;
        public SO_Int so_speedLevelGreen;
        public SO_Int so_unloadSpeedGreen;

        public SO_Bool so_unlockedBlue;
        public SO_Int so_capacityBlue;
        public SO_Int so_speedLevelBlue;
        public SO_Int so_unloadSpeedBlue;

        public SO_Int so_haxlerMineralsRed;
        public SO_Int so_haxlerMineralsGreen;
        public SO_Int so_haxlerMineralsBlue;

        public SO_Int so_haxlerSpeedRed;
        public SO_Int so_haxlerSpeedGreen;
        public SO_Int so_haxlerSpeedBlue;

        public SO_Int so_pufferMineralsRed;
        public SO_Int so_pufferMineralsGreen;
        public SO_Int so_pufferMineralsBlue;

        public SO_Int so_pufferLevelRed;
        public SO_Int so_pufferLevelGreen;
        public SO_Int so_pufferLevelBlue;

        public SO_Int so_DroneSpeed;

        #endregion

        #region controllers

        [SerializeField] private CoinController _coinController;

        public MixerController mixerController;

        public CollectorController RedController;
        public CollectorController GreenController;
        public CollectorController BlueController;

        public HaxlerController RedHaxlerController;
        public HaxlerController GreenHaxlerController;
        public HaxlerController BlueHaxlerController;

        public PufferController RedPufferController;
        public PufferController GreenPufferController;
        public PufferController BluePufferController;

        #endregion

        #region MyTypes

        public static readonly Dictionary<int, bool[]> RGB = new()
        {
            //               R      G      B
            { 1, new bool[] { true, false, false } }, // red
            { 2, new bool[] { false, true, false } }, // green
            { 3, new bool[] { false, false, true } }, // blue
            { 4, new bool[] { true, true, false } }, // yellow
            { 5, new bool[] { true, false, true } }, // magenta
            { 6, new bool[] { false, true, true } }, // cyan
            { 7, new bool[] { true, true, true } } // white
        };

        private readonly Dictionary<int, Color> Colors = new()
        {
            //               R      G      B
            { 1, Color.red },
            { 2, Color.green },
            { 3, Color.blue },
            { 4, Color.yellow },
            { 5, Color.magenta },
            { 6, Color.cyan },
            { 7, Color.white },
        };

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            coinsText.text = "" + coins;

            InvokeRepeating(nameof(InitializeAds), 1, 5);
        }

        private void Start()
        {
            Time.timeScale = 0;

            Application.targetFrameRate = -1;
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleException;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleException;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (ReadyToSave && !hasFocus)
            {
                SaveGameData();
                Log("Gamedate saved");
            }

            if (!Advertisement.isInitialized)
            {
                InitializeAds();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            // if (Application.platform == RuntimePlatform.Android)
            // {
            //     Debug.developerConsoleVisible = false;
            // }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Screen.SetResolution(1280, 720, false);
                Application.runInBackground = true;
            }
            // LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }

        private void HandleException(string condition, string stackTrace,
            LogType type)
        {
            if (type == LogType.Exception)
                // Handle the exception here
                LogError("Exception: " + condition + "\n" + stackTrace);
        }

        public void AddCoins(int pCoins)
        {
            if (pCoins > 0)
            {
                coins += pCoins * _coinMultiplier;
                _coinController.TriggerScaling();
                if (!_coinAudioSource.isPlaying)
                {
                    _coinAudioSource.Play();
                }

                EventManager.CoinsAdded.Invoke();
            }

            coinsText.text = "" + coins;
        }

        public void SubCoins(int pCoins)
        {
            coins -= pCoins;
            coinsText.text = "" + coins;
            _coinController.TriggerScaling();
        }

        public int GetCoins()
        {
            return coins;
        }

        public void ResetValues()
        {
            // if (!Application.isEditor)
            // {
            coins = 200;

            so_unlockedRed.value = true;
            so_capacityRed.value = 1;
            so_speedLevelRed.value = 2;
            so_unloadSpeedRed.value = 1;

            so_unlockedGreen.value = false;
            so_capacityGreen.value = 1;
            so_speedLevelGreen.value = 2;
            so_unloadSpeedGreen.value = 1;

            so_unlockedBlue.value = false;
            so_capacityBlue.value = 1;
            so_speedLevelBlue.value = 2;
            so_unloadSpeedBlue.value = 1;

            so_haxlerMineralsRed.value = 0;
            so_haxlerMineralsGreen.value = 0;
            so_haxlerMineralsBlue.value = 0;

            so_haxlerSpeedRed.value = 1;
            so_haxlerSpeedGreen.value = 1;
            so_haxlerSpeedBlue.value = 1;

            so_pufferMineralsRed.value = 0;
            so_pufferMineralsGreen.value = 0;
            so_pufferMineralsBlue.value = 0;

            so_pufferLevelRed.value = 1;
            so_pufferLevelGreen.value = 1;
            so_pufferLevelBlue.value = 1;

            so_DroneSpeed.value = 1;
            // }
            // else
            // {
            //     // diese werte im editor verwenden ...
            //     coins = 200;
            //
            //     so_unlockedRed.value = true;
            //     so_capacityRed.value = 3;
            //     so_speedLevelRed.value = 8;
            //     so_unloadSpeedRed.value = 1;
            //
            //     so_unlockedGreen.value = false;
            //     so_capacityGreen.value = 1;
            //     so_speedLevelGreen.value = 2;
            //     so_unloadSpeedGreen.value = 1;
            //
            //     so_unlockedBlue.value = false;
            //     so_capacityBlue.value = 1;
            //     so_speedLevelBlue.value = 2;
            //     so_unloadSpeedBlue.value = 1;
            //
            //     so_haxlerMineralsRed.value = 0;
            //     so_haxlerMineralsGreen.value = 0;
            //     so_haxlerMineralsBlue.value = 0;
            //
            //     so_haxlerSpeedRed.value = 1;
            //     so_haxlerSpeedGreen.value = 1;
            //     so_haxlerSpeedBlue.value = 1;
            //
            //     so_pufferMineralsRed.value = 72;
            //     so_pufferMineralsGreen.value = 72;
            //     so_pufferMineralsBlue.value = 72;
            //
            //     so_pufferLevelRed.value = 3;
            //     so_pufferLevelGreen.value = 3;
            //     so_pufferLevelBlue.value = 3;
            //
            //     so_DroneSpeed.value = 1;
            // }

            TakeNewValues();
        }

        private void TakeNewValues()
        {
            RedController.TakeInitValues();
            GreenController.TakeInitValues();
            BlueController.TakeInitValues();

            RedHaxlerController.TakeInitValues();
            GreenHaxlerController.TakeInitValues();
            BlueHaxlerController.TakeInitValues();

            RedPufferController.TakeInitValues();
            GreenPufferController.TakeInitValues();
            BluePufferController.TakeInitValues();

            ReadyToSave = true;
            AddCoins(0); // to update GUI
        }

        public void LoadGameData()
        {
            coins = PlayerPrefs.GetInt("coins", 200);

            so_unlockedRed.value = true;
            so_capacityRed.value = PlayerPrefs.GetInt("so_capacityRed", 1);
            so_speedLevelRed.value = PlayerPrefs.GetInt("so_speedLevelRed", 2);
            so_unloadSpeedRed.value = PlayerPrefs.GetInt("so_unloadSpeedRed", 1);

            so_unlockedGreen.value = PlayerPrefs.GetInt("so_unlockedGreen", 0) == 1;
            so_capacityGreen.value = PlayerPrefs.GetInt("so_capacityGreen", 1);
            so_speedLevelGreen.value = PlayerPrefs.GetInt("so_speedLevelGreen", 2);
            so_unloadSpeedGreen.value = PlayerPrefs.GetInt("so_unloadSpeedGreen", 1);

            so_unlockedBlue.value = PlayerPrefs.GetInt("so_unlockedBlue", 0) == 1;
            so_capacityBlue.value = PlayerPrefs.GetInt("so_capacityBlue", 1);
            so_speedLevelBlue.value = PlayerPrefs.GetInt("so_speedLevelBlue", 2);
            so_unloadSpeedBlue.value = PlayerPrefs.GetInt("so_unloadSpeedBlue", 1);

            so_haxlerMineralsRed.value = PlayerPrefs.GetInt("so_haxlerMineralsRed", 0);
            so_haxlerMineralsGreen.value = PlayerPrefs.GetInt("so_haxlerMineralsGreen", 0);
            so_haxlerMineralsBlue.value = PlayerPrefs.GetInt("so_haxlerMineralsBlue", 0);

            so_haxlerSpeedRed.value = PlayerPrefs.GetInt("so_haxlerSpeedRed", 1);
            so_haxlerSpeedGreen.value = PlayerPrefs.GetInt("so_haxlerSpeedGreen", 1);
            so_haxlerSpeedBlue.value = PlayerPrefs.GetInt("so_haxlerSpeedBlue", 1);

            so_pufferMineralsRed.value = PlayerPrefs.GetInt("so_pufferMineralsRed", 0) / 4 * 4;
            so_pufferMineralsGreen.value = PlayerPrefs.GetInt("so_pufferMineralsGreen", 0) / 4 * 4;
            so_pufferMineralsBlue.value = PlayerPrefs.GetInt("so_pufferMineralsBlue", 0) / 4 * 4;

            so_pufferLevelRed.value = PlayerPrefs.GetInt("so_pufferLevelRed", 1);
            so_pufferLevelGreen.value = PlayerPrefs.GetInt("so_pufferLevelGreen", 1);
            so_pufferLevelBlue.value = PlayerPrefs.GetInt("so_pufferLevelBlue", 1);

            so_DroneSpeed.value = PlayerPrefs.GetInt("so_DroneSpeed", 1);

            TakeNewValues();
        }

        public void SaveGameData()
        {
            PlayerPrefs.SetInt("coins", coins);

            PlayerPrefs.SetInt("so_capacityRed", so_capacityRed.value);
            PlayerPrefs.SetInt("so_speedLevelRed", so_speedLevelRed.value);
            PlayerPrefs.SetInt("so_unloadSpeedRed", so_unloadSpeedRed.value);

            PlayerPrefs.SetInt("so_unlockedGreen", so_unlockedGreen.value ? 1 : 0);
            PlayerPrefs.SetInt("so_capacityGreen", so_capacityGreen.value);
            PlayerPrefs.SetInt("so_speedLevelGreen", so_speedLevelGreen.value);
            PlayerPrefs.SetInt("so_unloadSpeedGreen", so_unloadSpeedGreen.value);

            PlayerPrefs.SetInt("so_unlockedBlue", so_unlockedBlue.value ? 1 : 0);
            PlayerPrefs.SetInt("so_capacityBlue", so_capacityBlue.value);
            PlayerPrefs.SetInt("so_speedLevelBlue", so_speedLevelBlue.value);
            PlayerPrefs.SetInt("so_unloadSpeedBlue", so_unloadSpeedBlue.value);

            PlayerPrefs.SetInt("so_haxlerMineralsRed", so_haxlerMineralsRed.value);
            PlayerPrefs.SetInt("so_haxlerMineralsGreen", so_haxlerMineralsGreen.value);
            PlayerPrefs.SetInt("so_haxlerMineralsBlue", so_haxlerMineralsBlue.value);

            PlayerPrefs.SetInt("so_haxlerSpeedRed", so_haxlerSpeedRed.value);
            PlayerPrefs.SetInt("so_haxlerSpeedGreen", so_haxlerSpeedGreen.value);
            PlayerPrefs.SetInt("so_haxlerSpeedBlue", so_haxlerSpeedBlue.value);

            PlayerPrefs.SetInt("so_pufferMineralsRed", so_pufferMineralsRed.value);
            PlayerPrefs.SetInt("so_pufferMineralsGreen", so_pufferMineralsGreen.value);
            PlayerPrefs.SetInt("so_pufferMineralsBlue", so_pufferMineralsBlue.value);

            PlayerPrefs.SetInt("so_pufferLevelRed", so_pufferLevelRed.value);
            PlayerPrefs.SetInt("so_pufferLevelGreen", so_pufferLevelGreen.value);
            PlayerPrefs.SetInt("so_pufferLevelBlue", so_pufferLevelBlue.value);

            PlayerPrefs.SetInt("so_DroneSpeed", so_DroneSpeed.value);

            PlayerPrefs.Save();
        }

        public static void Log(string text)
        {
            print("--LOG--------------------------");
            print(text);

            // Debug.Log("-----------------------------------");
            // Debug.Log(text);
        }

        public static void LogError(string text)
        {
            print("--ERROR------------------------");
            print(text);

            // Debug.LogError("-----------------------------------");
            // Debug.LogError(text);
        }

        public Color GetColorForIndex(int colorIndex)
        {
            return Colors.GetValueOrDefault(colorIndex, Color.white);
        }

        #region ADS

        public void InitializeAds()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (Advertisement.isSupported)
                {
                    if (!Advertisement.isInitialized)
                    {
                        // ACHTUNG 2. parameter ist TEST = JA
                        Debug.Log("try Advertisement.Initialize ...");
                        Advertisement.Initialize(GAMEID, false, this);
                        return;
                    }

                    if (!AdsRewardedLoaded)
                    {
                        Debug.Log("try Load Rewarded Ads ...");
                        LoadRewardedAd();
                        return;
                    }

                    Debug.Log("Advertisement is completely initialized");
                }
                else
                {
                    Log("Ads not supported");
                }
            }
            else
            {
                Debug.Log("not connected");
            }
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
            LoadRewardedAd();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        private void LoadRewardedAd()
        {
            // IMPORTANT! Only load content AFTER initialization 
            Debug.Log("Loading Rewarded Ad ...");
            Advertisement.Load(REWARDED_ANDROID, this);
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Ad Loaded: " + adUnitId);
            AdsRewardedLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }

        #endregion
    }
}