using System;
using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
    private void Awake()
    {
        var text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = "v " + Application.version;
        }
    }
}