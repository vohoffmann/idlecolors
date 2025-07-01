using System.Collections.Generic;
using IdleColors.hud.coin;
using IdleColors.room_collect.collector;
using IdleColors.room_mixing.haxler;
using IdleColors.room_mixing.mixer;
using IdleColors.room_mixing.puffer;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.Globals
{
    public partial class GameManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener
    {
        public static GameManager Instance;

        #region members

        public           GameObject cupBp;
        public           Text       coinsText;
        private readonly int        _coinMultiplier  = 1;
        public const     string     REWARDED_ANDROID = "Rewarded_Android";

        private readonly string GAMEID = "5774375";

        public static bool MenuBlocked { get; set; }

        public                   bool       ImageOrderInProcess;
        public                   int        ImageOrderRewards;
        [SerializeField] private Transform  _tmpFloatingtextContainer;
        [SerializeField] private Transform  _floatingCoinsUIPosition;
        [SerializeField] private GameObject _floatingCoinsWorldPrefab;
        [SerializeField] private GameObject _floatingCoinsUIPrefab;

        public  int  coins;
        private bool _playCoinSound = true;

        // index 0 is dummy !!!
        public int[] FinalColorCounts = new int[8];

        [SerializeField] private AudioSource _coinAudioSource;

        [SerializeField] private EventSystem _eventSystem;

        #endregion

        #region SOs

        public SO_Bool so_unlockedRed;
        public SO_Int  so_capacityRed;
        public SO_Int  so_speedLevelRed;
        public SO_Int  so_unloadSpeedRed;

        public SO_Bool so_unlockedGreen;
        public SO_Int  so_capacityGreen;
        public SO_Int  so_speedLevelGreen;
        public SO_Int  so_unloadSpeedGreen;

        public SO_Bool so_unlockedBlue;
        public SO_Int  so_capacityBlue;
        public SO_Int  so_speedLevelBlue;
        public SO_Int  so_unloadSpeedBlue;

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

        public SO_Int so_ConstructorSpeed;

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
            { 1, new[] { true, false, false } }, // red
            { 2, new[] { false, true, false } }, // green
            { 3, new[] { false, false, true } }, // blue
            { 4, new[] { true, true, false } }, // yellow
            { 5, new[] { true, false, true } }, // magenta
            { 6, new[] { false, true, true } }, // cyan
            { 7, new[] { true, true, true } } // white
        };

        private readonly Dictionary<int, Color> Idx2Color = new()
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

        private readonly Dictionary<Color, int> Color2Idx = new()
        {
            //               R      G      B
            { new Color(1, 0, 0, 1), 0 },
            { new Color(0, 1, 0, 1), 1 },
            { new Color(0, 0, 1, 1), 2 },
            { new Color(1, 1, 0, 1), 3 },
            { new Color(1, 0, 1, 1), 4 },
            { new Color(0, 1, 1, 1), 5 },
            { new Color(1, 1, 1, 1), 6 }
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

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Screen.SetResolution(1280, 720, false);
                Application.runInBackground = true;
            }
            // LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }

        private void HandleException(string condition, string stackTrace,
            LogType                         type)
        {
            if (type == LogType.Exception)
                // Handle the exception here
                LogError("Exception: " + condition + "\n" + stackTrace);
        }

        public void ToggleCoinSound()
        {
            _playCoinSound = !_playCoinSound;
            // PlayerPrefs.SetInt("CoinSound", _playCoinSound ? 1 : 0);
            // PlayerPrefs.Save();
        }

        public void AddCoins(int pCoins, Vector3 pos = default, GameObject parent = null)
        {
            if (pCoins > 0)
            {
                coins += pCoins * _coinMultiplier;
                _coinController.TriggerScaling();
                if (!_coinAudioSource.isPlaying && _playCoinSound)
                {
                    _coinAudioSource.Play();
                }

                if (pos != Vector3.zero)
                {
                    GameObject floatingText;
                    if (parent == null)
                    {
                        floatingText = Instantiate(_floatingCoinsWorldPrefab, pos, Quaternion.identity);
                        floatingText.transform.SetParent(_tmpFloatingtextContainer, true);
                    }
                    else
                    {
                        floatingText = Instantiate(_floatingCoinsUIPrefab, _floatingCoinsUIPosition, false);
                    }

                    floatingText.GetComponentInChildren<TextMeshProUGUI>().text = "" + pCoins;
                }

                EventManager.CoinsAdded.Invoke();
            }

            coinsText.text = "" + coins;
        }

        public void SubCoins(int pCoins)
        {
            coins          -= pCoins;
            coinsText.text =  "" + coins;
            // _coinController.TriggerScaling();
        }

        public int GetCoins()
        {
            return coins;
        }

        public void ResetValues()
        {
            coins = 200;

            so_unlockedRed.value    = true;
            so_capacityRed.value    = 1;
            so_speedLevelRed.value  = 2;
            so_unloadSpeedRed.value = 1;

            so_unlockedGreen.value    = false;
            so_capacityGreen.value    = 1;
            so_speedLevelGreen.value  = 2;
            so_unloadSpeedGreen.value = 1;

            so_unlockedBlue.value    = false;
            so_capacityBlue.value    = 1;
            so_speedLevelBlue.value  = 2;
            so_unloadSpeedBlue.value = 1;

            so_haxlerMineralsRed.value   = 3;
            so_haxlerMineralsGreen.value = 0;
            so_haxlerMineralsBlue.value  = 0;

            so_haxlerSpeedRed.value   = 1;
            so_haxlerSpeedGreen.value = 1;
            so_haxlerSpeedBlue.value  = 1;

            so_pufferMineralsRed.value   = 20;
            so_pufferMineralsGreen.value = 0;
            so_pufferMineralsBlue.value  = 0;

            so_pufferLevelRed.value   = 1;
            so_pufferLevelGreen.value = 1;
            so_pufferLevelBlue.value  = 1;

            so_DroneSpeed.value = 1;

            so_ConstructorSpeed.value = 0;

            FinalColorCounts = new int[8];

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

            // if (PlayerPrefs.HasKey("CoinSound"))
            // {
            //     _playCoinSound = PlayerPrefs.GetInt("CoinSound", 1) == 1 ? true : false;
            // }

            ReadyToSave = true;
            AddCoins(0); // to update GUI
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
            return Idx2Color.GetValueOrDefault(colorIndex, Color.white);
        }

        public int GetIndexForColor(Color color)
        {
            return Color2Idx.GetValueOrDefault(color, 0);
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

                    LoadRewardedAd();

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

        public void LoadRewardedAd()
        {
            // IMPORTANT! Only load content AFTER initialization 
            Debug.Log("Loading Rewarded Ad ...");
            Advertisement.Load(REWARDED_ANDROID, this);
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Ad Loaded: " + adUnitId);
        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }

        #endregion
    }
}