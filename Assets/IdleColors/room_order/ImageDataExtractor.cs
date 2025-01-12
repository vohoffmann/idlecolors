using System.Collections.Generic;
using IdleColors.Globals;
using UnityEngine;

/// <summary>
/// die klasse extrahiert alle pixel des bildes in ein array ...
/// aus diesen werten wird dan der 'bild ersteller ) gesteuert ...
/// </summary>
public class ImageDataExtractor : MonoBehaviour
{
    public Texture2D inputImage; 
    
    private List<Vector2> pixelPositions = new List<Vector2>();
    private List<Color> pixelColors = new List<Color>();

    void Start()
    {
        if (inputImage == null)
        {
            GameManager.Log("Kein Bild zugewiesen!");
            return;
        }

        ExtractImageData(inputImage);
        GameManager.Log($"Extrahiert: {pixelPositions.Count} Pixelpositionen und Farbwerte.");
    }

    void ExtractImageData(Texture2D image)
    {
        // Durchlaufe jedes Pixel im Bild
        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                // Extrahiere die Farbe des Pixels
                Color pixelColor = image.GetPixel(x, y);

                // Speichere die Pixelposition (x, y) und die Farbe
                pixelPositions.Add(new Vector2(x, y));
                pixelColors.Add(pixelColor);
            }
        }
    }

    public List<Vector2> GetPixelPositions()
    {
        return pixelPositions;
    }

    public List<Color> GetPixelColors()
    {
        return pixelColors;
    }
}