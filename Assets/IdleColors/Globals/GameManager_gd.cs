using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using IdleColors.room_order.constructor;
using UnityEngine;

namespace IdleColors.Globals
{
    // this part halds the logic to save and load gamedata
    public partial class GameManager
    {
        private const           string PLAYERDATA = "playerdata";
        public                  bool   ReadyToSave { private set; get; }
        private static readonly string encryptionKey = "nunabeR23!987654"; // 16, 24 oder 32 Zeichen

        private void OnApplicationFocus(bool hasFocus)
        {
            if (ReadyToSave && !hasFocus)
            {
                Debug.Log("call to SaveGameData()");
                SaveGameData();
            }
        }

        public void LoadGameData()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                Debug.Log("webgl ...");
                IndexedDBHandler dbManager = new IndexedDBHandler();
                dbManager.LoadFromIndexedDB("data");
#endif
            }
            else
            {
                HandleGameData(PlayerPrefs.GetString(PLAYERDATA, null));
            }
        }

        private void HandleGameData(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
            {
                Debug.LogWarning("encryptedData empty ... resetting ...");
                ResetValues();
                return;
            }

            string json = Decrypt(encryptedData, encryptionKey);
            var    data = JsonUtility.FromJson<GameData>(json);

            coins = data.coins;

            so_unlockedRed.value    = true;
            so_capacityRed.value    = data.so_capacityRed;
            so_speedLevelRed.value  = data.so_speedLevelRed;
            so_unloadSpeedRed.value = data.so_unloadSpeedRed;

            so_unlockedGreen.value    = data.so_unlockedGreen == 1;
            so_capacityGreen.value    = data.so_capacityGreen;
            so_speedLevelGreen.value  = data.so_speedLevelGreen;
            so_unloadSpeedGreen.value = data.so_unloadSpeedGreen;

            so_unlockedBlue.value    = data.so_unlockedBlue == 1;
            so_capacityBlue.value    = data.so_capacityBlue;
            so_speedLevelBlue.value  = data.so_speedLevelBlue;
            so_unloadSpeedBlue.value = data.so_unloadSpeedBlue;

            so_haxlerMineralsRed.value   = data.so_haxlerMineralsRed;
            so_haxlerMineralsGreen.value = data.so_haxlerMineralsGreen;
            so_haxlerMineralsBlue.value  = data.so_haxlerMineralsBlue;

            so_haxlerSpeedRed.value   = data.so_haxlerSpeedRed;
            so_haxlerSpeedGreen.value = data.so_haxlerSpeedGreen;
            so_haxlerSpeedBlue.value  = data.so_haxlerSpeedBlue;

            so_pufferMineralsRed.value   = data.so_pufferMineralsRed;
            so_pufferMineralsGreen.value = data.so_pufferMineralsGreen;
            so_pufferMineralsBlue.value  = data.so_pufferMineralsBlue;

            so_pufferLevelRed.value   = data.so_pufferLevelRed;
            so_pufferLevelGreen.value = data.so_pufferLevelGreen;
            so_pufferLevelBlue.value  = data.so_pufferLevelBlue;

            so_DroneSpeed.value = data.so_DroneSpeed;

            so_ConstructorSpeed.value = data.so_ConstructorSpeed;

            var index = 0;
            foreach (int finalColor in data.finalColorCounts)
            {
                FinalColorCounts[index] = finalColor;
                index++;
            }

            ConstructorController.instance.targets = new();

            if (data.imageData.Count != 0)
            {
                ConstructorController.instance.targets = data.imageData;
                EventManager.GenerateImageRasterFromData.Invoke();
            }

            TakeNewValues();
        }

        private static string Encrypt(string plainText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV  = new byte[16]; // Initialisierungsvektor mit Nullen (kann randomisiert werden)

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes  = Encoding.UTF8.GetBytes(plainText);
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
                aes.IV  = new byte[16]; // Initialisierungsvektor mit Nullen

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes  = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }

        public void SaveGameData()
        {
            var data = new GameData
            {
                coins                  = coins,
                so_speedLevelRed       = so_speedLevelRed.value,
                so_speedLevelGreen     = so_speedLevelGreen.value,
                so_speedLevelBlue      = so_speedLevelBlue.value,
                so_unloadSpeedRed      = so_unloadSpeedRed.value,
                so_unloadSpeedGreen    = so_unloadSpeedGreen.value,
                so_unloadSpeedBlue     = so_unloadSpeedBlue.value,
                so_unlockedGreen       = so_unlockedGreen.value ? 1 : 0,
                so_unlockedBlue        = so_unlockedBlue.value ? 1 : 0,
                so_capacityRed         = so_capacityRed.value,
                so_capacityGreen       = so_capacityGreen.value,
                so_capacityBlue        = so_capacityBlue.value,
                so_haxlerMineralsRed   = so_haxlerMineralsRed.value,
                so_haxlerMineralsGreen = so_haxlerMineralsGreen.value,
                so_haxlerMineralsBlue  = so_haxlerMineralsBlue.value,
                so_haxlerSpeedRed      = so_haxlerSpeedRed.value,
                so_haxlerSpeedGreen    = so_haxlerSpeedGreen.value,
                so_haxlerSpeedBlue     = so_haxlerSpeedBlue.value,
                so_pufferMineralsRed   = so_pufferMineralsRed.value,
                so_pufferMineralsGreen = so_pufferMineralsGreen.value,
                so_pufferMineralsBlue  = so_pufferMineralsBlue.value,
                so_pufferLevelRed      = so_pufferLevelRed.value,
                so_pufferLevelGreen    = so_pufferLevelGreen.value,
                so_pufferLevelBlue     = so_pufferLevelBlue.value,
                so_DroneSpeed          = so_DroneSpeed.value,
                so_ConstructorSpeed    = so_ConstructorSpeed.value
            };

            var index = 0;
            foreach (int finalColor in FinalColorCounts)
            {
                data.finalColorCounts[index] = finalColor;
                index++;
            }

            data.imageData = ConstructorController.instance.targets;

            string json          = JsonUtility.ToJson(data);
            string encryptedData = Encrypt(json, encryptionKey);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                Debug.Log("webgl ...");
                IndexedDBHandler dbManager = new IndexedDBHandler();
                dbManager.SaveToIndexedDB("data", encryptedData);
#endif
            }
            else
            {
                PlayerPrefs.SetString(PLAYERDATA, encryptedData);

                PlayerPrefs.Save();
            }
        }

        [Serializable]
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

            public List<TargetMetaData> imageData = new List<TargetMetaData>();
        }
    }
}