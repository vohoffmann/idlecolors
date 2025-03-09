using System;
using System.Collections;
using IdleColors.Globals;
using IdleColors.hud;
using IdleColors.room_order.constructor;
using TMPro;
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
        private int rewards;
        private bool cleaning;

        public int Rewards
        {
            get => rewards;
            set
            {
                rewards = value;
                GameManager.Instance.ImageOrderRewards = rewards;
            }
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
                    var sp = newButton.GetComponentInChildren<Image>();
                    sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                        "" + CalculateCoinsForImage(texture);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                buttonIndex++;
            }
        }

        private void OnEnable()
        {
            EventManager.GenerateImageRasterFromData += GenerateImageRasterFromData;
        }

        private void OnDisable()
        {
            EventManager.GenerateImageRasterFromData -= GenerateImageRasterFromData;
        }

        public void OrderImage()
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
            var idx = clickedButton.name.Split("#")[0];
            ClaimRewards(int.Parse(idx));
            ClosePanel();
            GameManager.Instance.ImageOrderInProcess = true;
        }

        public void ClaimRewards(int index = -1)
        {
            if (Rewards != 0)
            {
                var coinTextPos = _imageContainer.transform.position;
                GameManager.Instance.AddCoins(Rewards,
                    new Vector3(coinTextPos.x + 8, coinTextPos.y, coinTextPos.z + 8));
                Rewards = 0;
            }
            ConstructorController.instance.targets = new();
            ConstructorController.instance.imageColors = new int[7];

            cleaning = true;
            StartCoroutine(CleanImagePlate());
            if (index != -1)
            {
                StartCoroutine(CreateNewImageRaster(index));
            }
        }

        private IEnumerator CleanImagePlate()
        {
            while (_imageContainer.transform.childCount > 0)
            {
                foreach (Transform child in _imageContainer.transform)
                {
                    if ((child.gameObject.GetComponent<Rigidbody>().constraints &
                         RigidbodyConstraints.FreezePositionZ) == RigidbodyConstraints.FreezePositionZ)
                        child.gameObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionZ;
                }

                yield return null;
            }

            cleaning = false;
        }

        private IEnumerator CreateNewImageRaster(int index)
        {
            while (cleaning)
            {
                yield return new WaitForEndOfFrame();
            }

            GenerateNewimageRaster(textures[index]);
        }

        public void GenerateImageRasterFromData()
        {
            ConstructorController.instance.imageColors = new int[7];
            var data = ConstructorController.instance.targets;

            foreach (TargetMetaData meta in data)
            {
                var cube = Instantiate(_cubePrefab, _imageContainer.transform, true);
                var parentPosition = _imageContainer.transform.position;
                cube.transform.position = meta.cubePosition;

                Rewards += OrderPanelController.CoinValues[meta.colorIndex + 1] / 10;

                ConstructorController.instance.imageColors[meta.colorIndex] += 1;
                ConstructorController.instance.jobDone = false;

                var color = GameManager.Instance.GetColorForIndex(meta.colorIndex + 1);
                color.a = meta.done ? 1 : .01f;
                cube.GetComponent<Renderer>().material.color = color;
            }

            // den constructor nicht gleich los schicken ... damit nicht noch fallende blöcke schon getriggert werden
            ConstructorController.instance.StartCounter();
            ConstructorController.instance.UpdateStatText();
            GameManager.Instance.ImageOrderInProcess = true;
        }

        void GenerateNewimageRaster(Texture2D image)
        {
            ConstructorController.instance.imageColors = new int[7];
            ConstructorController.instance.targets = new();
            Rewards = 0;
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
                        Rewards += OrderPanelController.CoinValues[colorIndex + 1] / 10;

                        var tmpPufferPos = _pufferPositions[colorIndex].transform.position;
                        var tmpCubePos = cube.transform.position;

                        var pufferTargetPos = new Vector3(tmpPufferPos.x, -29.8f, tmpPufferPos.z);
                        var cupeTargetPos = new Vector3(tmpCubePos.x, -29.8f, tmpCubePos.z);

                        TargetMetaData infos = new TargetMetaData
                        {
                            pufferPosition = pufferTargetPos,
                            cubePosition = cupeTargetPos,
                            colorIndex = colorIndex,
                            done = false
                        };

                        ConstructorController.instance.targets.Add(infos);
                        ConstructorController.instance.imageColors[colorIndex] += 1;
                        ConstructorController.instance.jobDone = false;

                        pixelColor.a = .01f;
                        cube.GetComponent<Renderer>().material.color = pixelColor;
                    }
                }
            }

            // den constructor nicht gleich los schicken ... damit nicht noch fallende blöcke schon getriggert werden
            ConstructorController.instance.StartCounter();
            ConstructorController.instance.UpdateStatText();
        }

        private int CalculateCoinsForImage(Texture2D image)
        {
            int coins = 0;
            for (int z = 0; z < image.height; z++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    var color = image.GetPixel(x, z);

                    if (color.r != 0 || color.g != 0 || color.b != 0)
                    {
                        coins += OrderPanelController.CoinValues[GameManager.Instance.GetIndexForColor(color) + 1] / 10;
                    }
                }
            }

            return coins;
        }

        public void ClosePanel()
        {
            _productionOrderPanel.SetActive(false);
        }
    }
}