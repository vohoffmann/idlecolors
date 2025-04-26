using System;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.room_order.constructor
{
    public class ConstructorSteeringController : MonoBehaviour, IPointerClickHandler, IUnityAdsShowListener
    {
        [SerializeField] private GameObject      _constructorMenu;
        [SerializeField] private SO_Int          _constructorSpeed;
        [SerializeField] private GameObject      _noMoreUpdatesButtonText;
        [SerializeField] private TextMeshProUGUI _speedStatusText;

        [SerializeField] private GameObject _speedButtonCanvas;

        private                  Button          _speedButton;
        [SerializeField] private Text            _speedButtonText;
        [SerializeField] private TextMeshProUGUI _speedUpdateInfoText;
        [SerializeField] private Button          _speedbyadsbutton;
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
                if (Advertisement.isInitialized && GameManager.Instance.AdsRewardedLoaded)
                {
                    _speedbyadsbutton.gameObject.SetActive(true);
                }
                else
                {
                    _speedbyadsbutton.gameObject.SetActive(false);
                }
            }
            else
            {
                _speedButtonCanvas.SetActive(false);
                _noMoreUpdatesButtonText.SetActive(true);
                _speedStatusText.text = "" + _constructorSpeed.value;
            }
        }

        public void UpdateSpeed(bool subCoins = false)
        {
            if (_constructorSpeed.value < GLOB.CONSTRUCTOR_SPEED_MAX)
            {
                if (subCoins)
                {
                    GameManager.Instance.SubCoins(
                        Mathf.RoundToInt(GLOB.CONSTRUCTOR_SPEED_BASE_PRICE * Mathf.Pow(1.5f, _constructorSpeed.value)));
                }

                _constructorSpeed.value += 1;
                _updateSound.Play();
                updateMenuView();
            }
        }

        public void ShowAdsToUpgradeSpeed()
        {
            if (!Advertisement.isInitialized || !GameManager.Instance.AdsRewardedLoaded)
            {
                return;
            }

            Time.timeScale = 0;

            GameManager.MenuBlocked = true;

            Advertisement.Show(GameManager.REWARDED_ANDROID, this);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Time.timeScale = 1;

            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                UpdateSpeed();
            }

            GameManager.MenuBlocked = false;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Time.timeScale = 1;

            Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            // Debug.Log("Unity Ads Rewarded Ad Started");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            // Debug.Log("Unity Ads Rewarded Ad Clicked");
        }
    }
}