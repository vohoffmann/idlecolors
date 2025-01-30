using System;
using System.Collections;
using IdleColors.Globals;
using UnityEngine;
using Random = UnityEngine.Random;


public class ImageDataExtractor : MonoBehaviour
{
    public Texture2D[] images_1;
    public Texture2D[] images_2;
    public Texture2D[] images_3;

    private int index = 0;

    [SerializeField] private GameObject _cubePrefab;
    [SerializeField] private GameObject _imageContainer;

    private bool imageDeleted;

    private IEnumerator InstantiateImage()
    {
        while (_imageContainer.transform.childCount > 0)
        {
            foreach (Transform child in _imageContainer.transform)
            {
                Destroy(child.gameObject);
            }

            yield return null;
        }

        GenerateNewimageRaster(selectImage());
    }

    private Texture2D selectImage()
    {
        var arrayLevel = 0;

        if (GameManager.Instance.so_unlockedGreen.value)
            arrayLevel++;
        if (GameManager.Instance.so_unlockedBlue.value)
            arrayLevel++;

        Texture2D ret = null;
        switch (arrayLevel)
        {
            case 0:
                if (index == images_1.Length)
                    index = 0;
                ret = images_1[index];
                break;
            case 1:
                if (index == images_2.Length)
                    index = 0;
                ret = images_2[index];
                break;
            case 2:
                if (index == images_3.Length)
                    index = 0;
                ret = images_3[index];
                break;
        }

        index++;
        return ret;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // bild löschen
            StartCoroutine(InstantiateImage());
        }
    }

    void GenerateNewimageRaster(Texture2D image)
    {
        if (image == null)
        {
            Debug.LogError("Image data is null");
            return;
        }

        for (int z = 0; z < image.height; z++)
        {
            for (int x = 0; x < image.width; x++)
            {
                Color pixelColor = image.GetPixel(x, z);
                var cube = Instantiate(_cubePrefab, _imageContainer.transform, true);
                var parentPosition = _imageContainer.transform.position;
                cube.transform.position = new Vector3(parentPosition.x + x * .9f,
                    parentPosition.y + (Random.value * 30),
                    parentPosition.z + z * .9f);
                if (pixelColor.r != 0 || pixelColor.g != 0 || pixelColor.b != 0)
                {
                    pixelColor.a = .1f;
                }

                cube.GetComponent<Renderer>().material.color = pixelColor;
            }
        }
    }
}