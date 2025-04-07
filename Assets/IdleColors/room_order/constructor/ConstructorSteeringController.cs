using System;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.room_order.constructor
{
    public class ConstructorSteeringController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject      _constructorMenu;
        [SerializeField] private SO_Int          _constructorSpeed;
        [SerializeField] private GameObject      _noMoreUpdatesButtonText;
        [SerializeField] private TextMeshProUGUI _speedStatusText;

        [SerializeField] private GameObject _speedButtonCanvas;

        private                  Button          _speedButton;
        [SerializeField] private Text            _speedButtonText;
        [SerializeField] private TextMeshProUGUI _speedUpdateInfoText;
        [SerializeField] private AudioSource     _updateSound;

        private OutlineFlash _flash;


        private void Awake()
        {
            if (_speedButton == null)
            {
                _speedButton = _speedButtonText.GetComponent<Button>();
            }

            _flash = GetComponent<OutlineFlash>();

            if (_constructorSpeed.value == 0)
                InvokeRepeating(nameof(Flash), 5, 5);
        }

        private void Flash()
        {
            if (_constructorSpeed.value == 0)
            {
                _flash.TriggerFlash();
            }
            else
            {
                CancelInvoke(nameof(Flash));
            }
        }

        private void OnEnable()
        {
            EventManager.CoinsAdded += updateMenuView;
        }

        private void OnDisable()
        {
            EventManager.CoinsAdded -= updateMenuView;
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            CameraController.Instance.SetLockedTarget(this);
            updateMenuView();
            _constructorMenu.SetActive(true);
        }

        public void CloseMenu()
        {
            CameraController.Instance.UnsetLockedTarget();
            _constructorMenu.SetActive(false);
        }

        private void updateMenuView()
        {
            if (_constructorSpeed.value < GLOB.CONSTRUCTOR_SPEED_MAX)
            {
                _speedButtonCanvas.SetActive(true);
                _noMoreUpdatesButtonText.SetActive(false);

                _speedUpdateInfoText.text = $"{_constructorSpeed.value} -> {_constructorSpeed.value + 1}";

                var upgradeCost =
                    Mathf.RoundToInt(GLOB.CONSTRUCTOR_SPEED_BASE_PRICE * Mathf.Pow(1.5f, _constructorSpeed.value));
                _speedButtonText.text =
                    "" + upgradeCost;
                _speedButton.interactable = GameManager.Instance.GetCoins() >=
                                            upgradeCost;
            }
            else
            {
                _speedButtonCanvas.SetActive(false);
                _noMoreUpdatesButtonText.SetActive(true);
                _speedStatusText.text = "" + _constructorSpeed.value;
            }
        }

        public void UpdateSpeed()
        {
            if (_constructorSpeed.value < GLOB.CONSTRUCTOR_SPEED_MAX)
            {
                GameManager.Instance.SubCoins(
                    Mathf.RoundToInt(GLOB.CONSTRUCTOR_SPEED_BASE_PRICE * Mathf.Pow(1.5f, _constructorSpeed.value)));
                _constructorSpeed.value += 1;
                _updateSound.Play();
                updateMenuView();
            }
        }
    }
}