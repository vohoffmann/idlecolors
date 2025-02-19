using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using IdleColors.hud.coin;
using IdleColors.room_collect.collector;
using IdleColors.room_mixing.haxler;
using IdleColors.room_mixing.mixer;
using IdleColors.room_mixing.puffer;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace IdleColors.Globals
{
    public class GameManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener
    {
        public static GameManager Instance;

        #region members

        private const string PLAYERDATA = "playerdata";
        public GameObject cupBp;
        public Text coinsText;
        private readonly int _coinMultiplier = 1;
        public bool ReadyToSave { private set; get; }
        public const string REWARDED_ANDROID = "Rewarded_Android";
        private readonly string GAMEID = "5774375";
        public bool AdsRewardedLoaded { private set; get; }
        public bool ImageOrderInProcess;
        public int ImageOrderRewards;
        [SerializeField] private Transform _tmpFloatingtextContainer;
        [SerializeField] private Transform _floatingCoinsUIPosition;
        [SerializeField] private GameObject _floatingCoinsWorldPrefab;
        [SerializeField] private GameObject _floatingCoinsUIPrefab;

        public int coins;
        private bool _playCoinSound = true;
        private static readonly string encryptionKey = "nunabeR23!987654"; // 16, 24 oder 32 Zeichen

        // index 0 is dummy !!!
        public int[] FinalColorCounts = new int[8];

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
            coins -= pCoins;
            coinsText.text = "" + coins;
            // _coinController.TriggerScaling();
        }

        public int GetCoins()
        {
            return coins;
        }

        public void ResetValues()
        {
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

            so_haxlerMineralsRed.value = 3;
            so_haxlerMineralsGreen.value = 0;
            so_haxlerMineralsBlue.value = 0;

            so_haxlerSpeedRed.value = 1;
            so_haxlerSpeedGreen.value = 1;
            so_haxlerSpeedBlue.value = 1;

            so_pufferMineralsRed.value = 20;
            so_pufferMineralsGreen.value = 0;
            so_pufferMineralsBlue.value = 0;

            so_pufferLevelRed.value = 1;
            so_pufferLevelGreen.value = 1;
            so_pufferLevelBlue.value = 1;

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

        public void LoadGameData()
        {
            string encryptedData;

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                encryptedData = WebSaveSystem.LoadData(PLAYERDATA);
            }
            else
            {
                encryptedData = PlayerPrefs.GetString(PLAYERDATA, null);
            }

            if (string.IsNullOrEmpty(encryptedData))
            {
                Debug.LogWarning("Keine gespeicherten Daten gefunden. Reset der Daten ...");
                ResetValues();
                return;
            }
            else
            {
                Debug.Log("Daten gefunden");
            }

            string json = Decrypt(encryptedData, encryptionKey);
            var data = JsonUtility.FromJson<GameData>(json);

            coins = data.coins;

            so_unlockedRed.value = true;
            so_capacityRed.value = data.so_capacityRed;
            so_speedLevelRed.value = data.so_speedLevelRed;
            so_unloadSpeedRed.value = data.so_unloadSpeedRed;

            so_unlockedGreen.value = data.so_unlockedGreen == 1;
            so_capacityGreen.value = data.so_capacityGreen;
            so_speedLevelGreen.value = data.so_speedLevelGreen;
            so_unloadSpeedGreen.value = data.so_unloadSpeedGreen;

            so_unlockedBlue.value = data.so_unlockedBlue == 1;
            so_capacityBlue.value = data.so_capacityBlue;
            so_speedLevelBlue.value = data.so_speedLevelBlue;
            so_unloadSpeedBlue.value = data.so_unloadSpeedBlue;

            so_haxlerMineralsRed.value = data.so_haxlerMineralsRed;
            so_haxlerMineralsGreen.value = data.so_haxlerMineralsGreen;
            so_haxlerMineralsBlue.value = data.so_haxlerMineralsBlue;

            so_haxlerSpeedRed.value = data.so_haxlerSpeedRed;
            so_haxlerSpeedGreen.value = data.so_haxlerSpeedGreen;
            so_haxlerSpeedBlue.value = data.so_haxlerSpeedBlue;

            so_pufferMineralsRed.value = data.so_pufferMineralsRed;
            so_pufferMineralsGreen.value = data.so_pufferMineralsGreen;
            so_pufferMineralsBlue.value = data.so_pufferMineralsBlue;

            so_pufferLevelRed.value = data.so_pufferLevelRed;
            so_pufferLevelGreen.value = data.so_pufferLevelGreen;
            so_pufferLevelBlue.value = data.so_pufferLevelBlue;

            so_DroneSpeed.value = data.so_DroneSpeed;

            so_ConstructorSpeed.value = data.so_ConstructorSpeed;

            var index = 0;
            foreach (int finalColor in data.finalColorCounts)
            {
                FinalColorCounts[index] = finalColor;
                index++;
            }

            TakeNewValues();
        }

        private static string Encrypt(string plainText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16]; // Initialisierungsvektor mit Nullen (kann randomisiert werden)

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }

        private static string Decrypt(string cipherText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[16]; // Initialisierungsvektor mit Nullen

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }

        public void SaveGameData()
        {
            var data = new GameData
            {
                coins = coins,
                so_speedLevelRed = so_speedLevelRed.value,
                so_speedLevelGreen = so_speedLevelGreen.value,
                so_speedLevelBlue = so_speedLevelBlue.value,
                so_unloadSpeedRed = so_unloadSpeedRed.value,
                so_unloadSpeedGreen = so_unloadSpeedGreen.value,
                so_unloadSpeedBlue = so_unloadSpeedBlue.value,
                so_unlockedGreen = so_unlockedGreen.value ? 1 : 0,
                so_unlockedBlue = so_unlockedBlue.value ? 1 : 0,
                so_capacityRed = so_capacityRed.value,
                so_capacityGreen = so_capacityGreen.value,
                so_capacityBlue = so_capacityBlue.value,
                so_haxlerMineralsRed = so_haxlerMineralsRed.value,
                so_haxlerMineralsGreen = so_haxlerMineralsGreen.value,
                so_haxlerMineralsBlue = so_haxlerMineralsBlue.value,
                so_haxlerSpeedRed = so_haxlerSpeedRed.value,
                so_haxlerSpeedGreen = so_haxlerSpeedGreen.value,
                so_haxlerSpeedBlue = so_haxlerSpeedBlue.value,
                so_pufferMineralsRed = so_pufferMineralsRed.value,
                so_pufferMineralsGreen = so_pufferMineralsGreen.value,
                so_pufferMineralsBlue = so_pufferMineralsBlue.value,
                so_pufferLevelRed = so_pufferLevelRed.value,
                so_pufferLevelGreen = so_pufferLevelGreen.value,
                so_pufferLevelBlue = so_pufferLevelBlue.value,
                so_DroneSpeed = so_DroneSpeed.value,
                so_ConstructorSpeed = so_ConstructorSpeed.value
            };

            var index = 0;
            foreach (int finalColor in FinalColorCounts)
            {
                data.finalColorCounts[index] = finalColor;
                index++;
            }

            string json = JsonUtility.ToJson(data);
            string encryptedData = Encrypt(json, encryptionKey);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                WebSaveSystem.SaveData(PLAYERDATA, encryptedData);
            }
            else
            {
                PlayerPrefs.SetString(PLAYERDATA, encryptedData);

                PlayerPrefs.Save();
            }
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

                    if (!AdsRewardedLoaded)
                    {
                        Debug.Log("try Load Rewarded Ads ...");
                        LoadRewardedAd();
                        return;
                    }

                    // Debug.Log("Advertisement is completely initialized");
                }
                else
                {
                    // Log("Ads not supported");
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

    class GameData
    {
        public int coins;

        public int so_capacityRed;
        public int so_speedLevelRed;
        public int so_unloadSpeedRed;

        public int so_unlockedGreen;
        public int so_capacityGreen;
        public int so_speedLevelGreen;
        public int so_unloadSpeedGreen;

        public int so_unlockedBlue;
        public int so_capacityBlue;
        public int so_speedLevelBlue;
        public int so_unloadSpeedBlue;

        public int so_haxlerMineralsRed;
        public int so_haxlerMineralsGreen;
        public int so_haxlerMineralsBlue;

        public int so_haxlerSpeedRed;
        public int so_haxlerSpeedGreen;
        public int so_haxlerSpeedBlue;

        public int so_pufferMineralsRed;
        public int so_pufferMineralsGreen;
        public int so_pufferMineralsBlue;

        public int so_pufferLevelRed;
        public int so_pufferLevelGreen;
        public int so_pufferLevelBlue;

        public int so_DroneSpeed;

        public int so_ConstructorSpeed;

        public int[] finalColorCounts = new int[8];
    }
}