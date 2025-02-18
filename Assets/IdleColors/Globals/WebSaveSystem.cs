using System.Runtime.InteropServices;
using UnityEngine;

namespace IdleColors.Globals
{
    public class WebSaveSystem : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void SetLocalStorage(string key, string value);

        [DllImport("__Internal")]
        private static extern string GetLocalStorage(string key);

        public static void SaveData(string key, string value)
        {
            SetLocalStorage(key, value);
        }

        public static string LoadData(string key)
        {
            return GetLocalStorage(key);
        }
    }
}