using System.Runtime.InteropServices;
using UnityEngine;

public class IndexedDBHandler : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SaveData(string key, string value);

    [DllImport("__Internal")]
    private static extern void LoadData(string key);

    public void SaveToIndexedDB(string key, string value)
    {
        SaveData(key + "", value + "");
    }

    public string LoadFromIndexedDB(string key)
    {
        LoadData(key + "");
        return "";
    }
#endif
}