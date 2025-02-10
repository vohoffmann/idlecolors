using System;
using System.Collections.Generic;
using IdleColors.Globals;
using IdleColors.room_mixing.mixer;
using IdleColors.room_mixing.puffer;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.hud
{
    public class OrderPanelController : MonoBehaviour
    {
        [SerializeField] private PufferController _redPuffer;
        [SerializeField] private PufferController _greenPuffer;
        [SerializeField] private PufferController _bluePuffer;
        [SerializeField] private MixerController _mixer;
        [SerializeField] private Button _redButton;
        [SerializeField] private TextMeshProUGUI _redAvailableOrdersText;
        [SerializeField] private Button _greenButton;
        [SerializeField] private TextMeshProUGUI _greenAvailableOrdersText;
        [SerializeField] private Button _blueButton;
        [SerializeField] private TextMeshProUGUI _blueAvailableOrdersText;
        [SerializeField] private Button _yellowButton;
        [SerializeField] private TextMeshProUGUI _yellowAvailableOrdersText;
        [SerializeField] private Button _pinkButton;
        [SerializeField] private TextMeshProUGUI _pinkAvailableOrdersText;
        [SerializeField] private Button _cyanButton;
        [SerializeField] private TextMeshProUGUI _cyanAvailableOrdersText;
        [SerializeField] private Button _whiteButton;
        [SerializeField] private TextMeshProUGUI _whiteAvailableOrdersText;
        [SerializeField] private GameObject _orderQueuePanel;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private ParticleSystem _particleSystem;

        // 1    2    3    4    5     6     7
        // 5   10   30   20   40    50    70
        public static readonly int[] CoinValues = { 0, 50, 100, 150, 200, 300, 350, 500 };

        private int _red, _green, _blue;
        private readonly Queue<int> _orders = new();
        private Image _image;
        private const int APO = 24;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            EventManager.FlashOrderMenu += Flash;
        }


        private void OnDisable()
        {
            EventManager.FlashOrderMenu -= Flash;
        }

        private void Flash()
        {
            // das obere menu flaschen
            if (_image != null)
            {
                _image.color = Color.white;
            }
        }

        private void Update()
        {
            var color = _image.color;
            if (color.r > .23f)
            {
                color.r -= Time.deltaTime;
                color.g -= Time.deltaTime;
                color.b -= Time.deltaTime;
                _image.color = color;
            }
        }

        private void Start()
        {
            InvokeRepeating(nameof(ProcessOrder), 0, 1);
        }

        public void Order(int color)
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
            var transformPosition = clickedButton.transform.localPosition;

            var buttonPos = new Vector3(transformPosition.x - 520, transformPosition.y - 565, 0);
            _particleSystem.transform.position = buttonPos;
            _particleSystem.Play();

            GameManager.Instance.AddCoins(CoinValues[color], buttonPos, this.gameObject);

            _orders.Enqueue(color);

            if (_orders.Count > 0 || _mixer._mixing)
            {
                var newImageObj = new GameObject();
                var newImage = newImageObj.AddComponent<Image>();
                newImage.sprite = _sprite;
                var rectTransform =
                    newImageObj.GetComponent<RectTransform>();
                rectTransform.SetParent(_orderQueuePanel.transform);
                rectTransform.localPosition = Vector3.zero;
                rectTransform.sizeDelta = Vector2.one * .5f;
                rectTransform.rotation = Quaternion.identity;

                var rgb = GameManager.RGB.GetValueOrDefault(color);
                newImage.color = new Color(rgb[0] ? 1 : 0, rgb[1] ? 1 : 0,
                    rgb[2] ? 1 : 0);
            }

            // update button visibility
            UpdateButtonVisibility();
        }

        private void ProcessOrder()
        {
            UpdateButtonVisibility();

            if (_mixer._mixing)
                return;

            if (_orders.Count == 0)
                return;

            if (_orderQueuePanel.transform.childCount > 0)
            {
                Destroy(_orderQueuePanel.transform.GetChild(0)
                    .gameObject);
            }

            var color = _orders.Dequeue();

            var rgb = GameManager.RGB.GetValueOrDefault(color);

            if (rgb[0])
                _redPuffer.OrderMinerals(APO);

            if (rgb[1])
                _greenPuffer.OrderMinerals(APO);

            if (rgb[2])
                _bluePuffer.OrderMinerals(APO);

            _mixer.InitOrderMixing(color, rgb[0] ? APO : 0, rgb[1] ? APO : 0,
                rgb[2] ? APO : 0);
        }

        private void UpdateButtonVisibility()
        {
            _red = _redPuffer.GetAvailableMinerals();
            _green = _greenPuffer.GetAvailableMinerals();
            _blue = _bluePuffer.GetAvailableMinerals();

            // für jede order amountPerOrder abziehen
            foreach (int color in _orders)
            {
                var rgb = GameManager.RGB.GetValueOrDefault(color);
                _red -= rgb[0] ? APO : 0;
                _green -= rgb[1] ? APO : 0;
                _blue -= rgb[2] ? APO : 0;
            }

            var redButtonInteractable = _red >= APO;
            _redButton.interactable = redButtonInteractable;
            _redAvailableOrdersText.text = redButtonInteractable
                ? "" + _red / 24
                : "";
            _redAvailableOrdersText.transform.parent.gameObject.SetActive(redButtonInteractable);


            var greenButtonInteractable = _green >= APO;
            _greenButton.interactable = greenButtonInteractable;
            _greenAvailableOrdersText.text = greenButtonInteractable
                ? "" + _green / 24
                : "";
            _greenAvailableOrdersText.transform.parent.gameObject.SetActive(greenButtonInteractable);


            var blueButtonInteractable = _blue >= APO;
            _blueButton.interactable = blueButtonInteractable;
            _blueAvailableOrdersText.text = blueButtonInteractable
                ? "" + _blue / 24
                : "";
            _blueAvailableOrdersText.transform.parent.gameObject.SetActive(blueButtonInteractable);


            var yellowButtonInteractable = _red >= APO && _green >= APO;
            _yellowButton.interactable = yellowButtonInteractable;
            _yellowAvailableOrdersText.text = yellowButtonInteractable
                ? "" + Mathf.Min(_red, _green) / 24
                : "";
            _yellowAvailableOrdersText.transform.parent.gameObject.SetActive(yellowButtonInteractable);


            var pinkButtonInteractable = _red >= APO && _blue >= APO;
            _pinkButton.interactable = pinkButtonInteractable;
            _pinkAvailableOrdersText.text = pinkButtonInteractable
                ? "" + Mathf.Min(_red, _blue) / 24
                : "";
            _pinkAvailableOrdersText.transform.parent.gameObject.SetActive(pinkButtonInteractable);

            var cyanButtonInteractable = _green >= APO && _blue >= APO;
            _cyanButton.interactable = cyanButtonInteractable;
            _cyanAvailableOrdersText.text = cyanButtonInteractable
                ? "" + Mathf.Min(_green, _blue) / 24
                : "";
            _cyanAvailableOrdersText.transform.parent.gameObject.SetActive(cyanButtonInteractable);


            var whiteButtonInteractable = _red >= APO && _green >= APO && _blue >= APO;
            _whiteButton.interactable = whiteButtonInteractable;
            _whiteAvailableOrdersText.text = whiteButtonInteractable && _blue >= APO
                ? "" + Mathf.Min(_red, _green, _blue) / 24
                : "";
            _whiteAvailableOrdersText.transform.parent.gameObject.SetActive(whiteButtonInteractable);

            var rect = _orderQueuePanel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(_orders.Count * 41, 60);
        }
    }
}