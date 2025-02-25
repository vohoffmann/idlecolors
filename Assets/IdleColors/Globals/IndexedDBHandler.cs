using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class IndexedDBHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SaveData(string key, string value);

    [DllImport("__Internal")]
    private static extern void LoadData(string key);

    public void SaveToIndexedDB(string key, string value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveData(key + "", value + "");
#endif
    }

    public string LoadFromIndexedDB(string key)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        LoadData(key + "");
#endif
        return "";
    }
}