using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadInfo : MonoBehaviour
{
    private void Start()
    {
        var image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning($"image ist null");
            return;
        }

        var language = Application.systemLanguage.ToString().ToLower();

        if (language != "english" && language != "german")
        {
            language = "english";
        }

        var texture = Resources.Load<Sprite>($"{language}_tutorial");

        if (texture != null)
            image.sprite = texture;
        else
            Debug.LogWarning($"Bild für Sprache {language} nicht gefunden!");
    }
}