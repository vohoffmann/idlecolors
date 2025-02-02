using System;
using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.room_collect.collector;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace IdleColors.hud
{
    public class CollectorMenuController : MonoBehaviour, IUnityAdsShowListener
    {
        private CollectorController _collectorScript;

        [SerializeField] private GameObject _noMoreUpdatesButtonText;
        [SerializeField] private TextMeshProUGUI _speedStatusText;
        [SerializeField] private TextMeshProUGUI _capycityInfoText;
        [SerializeField] private TextMeshProUGUI _unloadspeedInfoText;

        [SerializeField] private GameObject _greenFirst;

        [SerializeField] private GameObject _activateButtonCanvas;
        private Button activateButton;
        [SerializeField] private Text _activateButtonText;
        [SerializeField] private Button _unlockbyadsbutton;
        [SerializeField] private Text _unlockbyadstext;

        [SerializeField] private GameObject _speedButtonCanvas;
        private Button speedButton;
        [SerializeField] private Text _speedButtonText;
        [SerializeField] private TextMeshProUGUI _speedUpdateInfoText;


        [SerializeField] private GameObject _capButtonCanvas;
        private Button capButton;
        [SerializeField] private Text _capButtonText;
        [SerializeField] private TextMeshProUGUI _capacityStatusText;

        [SerializeField] private GameObject _unloadSpeedButtonCanvas;
        private Button unloadSpeedButton;
        [SerializeField] private Text _unloadSpeedButtonText;
        [SerializeField] private TextMeshProUGUI _unloadSpeedStatusText;

        public void SetCollector(CollectorController collectorScript)
        {
            // Debug.Log("Set Collector");
            _collectorScript = collectorScript;
            _noMoreUpdatesButtonText.SetActive(false);

            if (activateButton == null)
            {
                activateButton = _activateButtonCanvas.GetComponentInChildren<Button>();
            }

            if (speedButton == null)
            {
                speedButton = _speedButtonText.GetComponent<Button>();
            }

            if (capButton == null)
            {
                capButton = _capButtonText.GetComponent<Button>();
            }

            if (unloadSpeedButton == null)
            {
                unloadSpeedButton = _unloadSpeedButtonText.GetComponent<Button>();
            }

            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            // Debug.Log("OnEnable");
            InvokeRepeating(nameof(UpdateButtonText), 0, .5f);
        }

        private void OnDisable()
        {
            // Debug.Log("OnDisable");
            CancelInvoke(nameof(UpdateButtonText));
        }

        public void UnlockCollector(bool subCoins = true)
        {
            if (_collectorScript == null)
            {
                return;
            }

            if (subCoins)
            {
                GameManager.Instance.SubCoins(GLOB.COLLECTOR_UNLOCK * _collectorScript.costFactor);
            }

            _collectorScript.Unlock();
            UpdateButtonText();
        }

        public void UpgradeSpeed()
        {
            _collectorScript.UpgradeSpeed();
            UpdateButtonText();
        }

        public void UpgradeCap()
        {
            _collectorScript.UpgradeCapacity();
            UpdateButtonText();
        }

        public void UpgradeUnloadSpeed()
        {
            _collectorScript.UpgradeUnloadSpeed();
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            // Debug.Log("UpdateButtonText");

            if (_collectorScript == null)
            {
                return;
            }

            var coins = GameManager.Instance.GetCoins();
            var costFactor = _collectorScript.costFactor;
            var buttons = false;

            _greenFirst.SetActive(false);

            if (!_collectorScript.IsUnlocked())
            {
                if (_collectorScript.name.Contains("blue") && !GameManager.Instance.so_unlockedGreen.value)
                {
                    _greenFirst.SetActive(true);

                    _activateButtonCanvas.SetActive(false);
                    _speedButtonCanvas.SetActive(false);
                    _capButtonCanvas.SetActive(false);
                    _unloadSpeedButtonCanvas.SetActive(false);
                }
                else
                {
                    _activateButtonCanvas.SetActive(true);

                    _speedButtonCanvas.SetActive(false);
                    _capButtonCanvas.SetActive(false);
                    _unloadSpeedButtonCanvas.SetActive(false);

                    _activateButtonText.text = "" +
                                               GLOB.COLLECTOR_UNLOCK *
                                               _collectorScript.costFactor;
                    activateButton.interactable = coins >= GLOB.COLLECTOR_UNLOCK * costFactor;

                    if (Advertisement.isInitialized && GameManager.Instance.AdsRewardedLoaded)
                    {
                        _unlockbyadsbutton.gameObject.SetActive(true);
                        _unlockbyadstext.gameObject.SetActive(true);
                    }
                    else
                    {
                        _unlockbyadsbutton.gameObject.SetActive(false);
                        _unlockbyadstext.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                _activateButtonCanvas.SetActive(false);

                // capacity
                if (_collectorScript.GetCapacity() < GLOB.COLLECTOR_MAX_CAPACITY)
                {
                    _capButtonCanvas.SetActive(true);

                    _capButtonText.text = "" + costFactor *
                        _collectorScript.GetCapacity() *
                        GLOB.COLLECTOR_CAPACITY_BASE_PRICE;
                    _capacityStatusText.text =
                        "" + _collectorScript.GetCapacity() + " -> " + (_collectorScript.GetCapacity() + 1);
                    capButton.interactable = coins >= costFactor * _collectorScript.GetCapacity() *
                        GLOB.COLLECTOR_CAPACITY_BASE_PRICE;

                    buttons = true;
                }
                else
                {
                    _capButtonCanvas.SetActive(false);
                }

                // speed
                if (_collectorScript.GetSpeedLevel() < GLOB.COLLECTOR_MAX_SPEED)
                {
                    _speedButtonCanvas.SetActive(true);

                    _speedButtonText.text = "" + costFactor *
                        _collectorScript.GetSpeedLevel() *
                        GLOB.COLLECTOR_SPEED_BASE_PRICE;
                    _speedUpdateInfoText.text = "" + (_collectorScript.GetSpeedLevel() - 1) + " -> " +
                                                (_collectorScript.GetSpeedLevel());
                    speedButton.interactable = coins >= costFactor * _collectorScript.GetSpeedLevel() *
                        GLOB.COLLECTOR_SPEED_BASE_PRICE;

                    buttons = true;
                }
                else
                {
                    _speedButtonCanvas.SetActive(false);
                }

                // unload
                if (_collectorScript.GetUnloadSpeed() < GLOB.COLLECTOR_MAX_UNLOADSPEED)
                {
                    _unloadSpeedButtonCanvas.SetActive(true);

                    _unloadSpeedButtonText.text = "" +
                                                  costFactor *
                                                  _collectorScript.GetUnloadSpeed() *
                                                  GLOB.COLLECTOR_UNLOADSPEED_BASE_PRICE;
                    var from = Mathf.Round(19.5f / _collectorScript.GetUnloadSpeed() * 100) / 100;
                    var to = Mathf.Round(19.5f / (_collectorScript.GetUnloadSpeed() + 1) * 100) / 100;
                    _unloadSpeedStatusText.text = $"{from:F2} -> {to:F2} sec";
                    unloadSpeedButton.interactable = coins >= costFactor * _collectorScript.GetUnloadSpeed() *
                        GLOB.COLLECTOR_UNLOADSPEED_BASE_PRICE;

                    buttons = true;
                }
                else
                {
                    _unloadSpeedButtonCanvas.SetActive(false);
                }

                // alles upgrades maximized ?
                if (!buttons)
                {
                    _noMoreUpdatesButtonText.SetActive(true);
                    _speedStatusText.text = _collectorScript.GetSpeedLevel().ToString();
                    _capycityInfoText.text = _collectorScript.GetCapacity().ToString();
                    _unloadspeedInfoText.text =
                        "" + Mathf.Round(19.5f / _collectorScript.GetUnloadSpeed() * 100) / 100 + " sec";
                }
            }
        }

        public void Close()
        {
            _collectorScript = null;
            CameraController.Instance.UnsetLockedTarget();
            gameObject.SetActive(false);
        }

        public void ShowAdsToActivate()
        {
            if (!Advertisement.isInitialized || !GameManager.Instance.AdsRewardedLoaded)
            {
                return;
            }

            Time.timeScale = 0;

            Advertisement.Show(GameManager.REWARDED_ANDROID, this);
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            Time.timeScale = 1;

            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                // Debug.Log("Unity Ads Rewarded Ad Completed");

                // TODO : mit deligate arbeiten ... den ganzen Ads mumpist auslagern und generisch machen, damit das für alles mögliche verwendet werden kann
                UnlockCollector(false);
            }
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Time.timeScale = 1;

            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log("Unity Ads Rewarded Ad Started");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log("Unity Ads Rewarded Ad Clicked");
        }
    }
}