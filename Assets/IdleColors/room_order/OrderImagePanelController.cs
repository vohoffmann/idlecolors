using System;
using System.Collections;
using IdleColors.Globals;
using IdleColors.room_order.constructor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace IdleColors.room_order
{
    public class OrderImagePanelController : MonoBehaviour
    {
        private Texture2D[] textures;
        private bool imageDeleted;
        [SerializeField] private GameObject _cubePrefab;
        [SerializeField] private GameObject _imageContainer;
        [SerializeField] private GameObject _productionOrderPanel;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private RectTransform _buttonContainer;
        [SerializeField] private GameObject[] _pufferPositions;
        private int buttonIndex;

        public void OrderImage()
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

            var idx = clickedButton.name.Split("#")[0];

            StartCoroutine(InstantiateImage(int.Parse(idx)));
            _productionOrderPanel.SetActive(false);
            GameManager.Instance.ImageOrderInProcess = true;
        }

        private void Awake()
        {
            textures = Resources.LoadAll<Texture2D>("ProductionModels");
            buttonIndex = 0;
            foreach (Texture2D texture in textures)
            {
                try
                {
                    var newButton = Instantiate(_buttonPrefab, _buttonContainer);
                    newButton.GetComponent<Button>().onClick.AddListener(OrderImage);

                    // set name ... first is index for the imageArray ... 2nd is to indicate if displaying or not
                    newButton.name = $"{buttonIndex}#{texture.name.Substring(0, 1)}";
                    var sp = newButton.GetComponentInChildren<UnityEngine.UI.Image>();
                    sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                buttonIndex++;
            }
        }

        private IEnumerator InstantiateImage(int index)
        {
            while (_imageContainer.transform.childCount > 0)
            {
                foreach (Transform child in _imageContainer.transform)
                {
                    Destroy(child.gameObject);
                }

                yield return null;
            }

            GenerateNewimageRaster(textures[index]);
        }

        void GenerateNewimageRaster(Texture2D image)
        {
            for (int z = 0; z < image.height; z++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    Color pixelColor = image.GetPixel(x, z);
                    if (pixelColor.r != 0 || pixelColor.g != 0 || pixelColor.b != 0)
                    {
                        var cube = Instantiate(_cubePrefab, _imageContainer.transform, true);
                        var parentPosition = _imageContainer.transform.position;
                        cube.transform.position = new Vector3(
                            parentPosition.x + x,
                            parentPosition.y + Random.value * 20,
                            parentPosition.z + z);

                        var colorIndex = GameManager.Instance.GetIndexForColor(pixelColor);
                        
                        TargetMetaData infos = new TargetMetaData
                        {
                            pufferPosition = _pufferPositions[colorIndex].transform.position,
                            cubePosition = cube.transform.position,
                            colorIndex = colorIndex
                        };

                        ConstructorController.instance.targets.Add(infos);

                        pixelColor.a = 0;
                        cube.GetComponent<Renderer>().material.color = pixelColor;
                    }
                }
            }
        }
    }
}