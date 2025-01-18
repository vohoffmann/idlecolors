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

        // update the buttons visibility regarding the coins
        private void Update()
        {
            if (_collectorScript == null)
            {
                return;
            }

            // unlock
            var coins = GameManager.Instance.GetCoins();

            var costFactor = _collectorScript.costFactor;
            if (!_collectorScript.IsUnlocked())
            {
                activateButton.interactable = coins >= GLOB.COLLECTOR_UNLOCK * costFactor;
            }
            // upgrades
            else
            {
                speedButton.interactable = coins >= costFactor * _collectorScript.GetSpeedLevel() *
                    GLOB.COLLECTOR_SPEED_BASE_PRICE;

                capButton.interactable = coins >= costFactor * _collectorScript.GetCapacity() *
                    GLOB.COLLECTOR_CAPACITY_BASE_PRICE;
                unloadSpeedButton.interactable = coins >= costFactor * _collectorScript.GetUnloadSpeed() *
                    GLOB.COLLECTOR_UNLOADSPEED_BASE_PRICE;
            }
        }

        public void SetCollector(CollectorController collectorScript)
        {
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

            if (!_collectorScript.IsUnlocked())
            {
                _activateButtonText.text = "" +
                                           GLOB.COLLECTOR_UNLOCK *
                                           _collectorScript.costFactor;
                _activateButtonCanvas.SetActive(true);

                _speedButtonCanvas.SetActive(false);
                _capButtonCanvas.SetActive(false);
                _unloadSpeedButtonCanvas.SetActive(false);

                if (Application.platform == RuntimePlatform.Android ||
                    Application.platform == RuntimePlatform.WindowsEditor)
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
            else
            {
                AktivateUpdateView();
            }

            gameObject.SetActive(true);
        }

        private void AktivateUpdateView()
        {
            _activateButtonCanvas.SetActive(false);

            _speedButtonCanvas.SetActive(true);
            _capButtonCanvas.SetActive(true);
            _unloadSpeedButtonCanvas.SetActive(true);
            UpdateButtonText();
        }

        public void UnlockCollector(bool subCoins = true)
        {
            if (_collectorScript == null)
            {
                return;
            }

            if (subCoins)
            {
                GameManager.Instance.SubCoins(GLOB.COLLECTOR_UNLOCK *
                                              _collectorScript.costFactor);
            }

            _collectorScript.Unlock();
            AktivateUpdateView();
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
            var buttons = false;
            // cap
            if (_collectorScript.GetCapacity() < GLOB.COLLECTOR_MAX_CAPACITY)
            {
                _capButtonText.text = "" + _collectorScript.costFactor *
                    _collectorScript.GetCapacity() *
                    GLOB.COLLECTOR_CAPACITY_BASE_PRICE;
                _capacityStatusText.text =
                    "" + _collectorScript.GetCapacity() + " -> " + (_collectorScript.GetCapacity() + 1);
                buttons = true;
            }
            else
            {
                _capButtonCanvas.SetActive(false);
            }

            // speed
            if (_collectorScript.GetSpeedLevel() < GLOB.COLLECTOR_MAX_SPEED)
            {
                _speedButtonText.text = "" + _collectorScript.costFactor *
                    _collectorScript.GetSpeedLevel() *
                    GLOB.COLLECTOR_SPEED_BASE_PRICE;
                _speedUpdateInfoText.text = "" + (_collectorScript.GetSpeedLevel() - 1) + " -> " +
                                            (_collectorScript.GetSpeedLevel());
                buttons = true;
            }
            else
            {
                _speedButtonCanvas.SetActive(false);
            }

            // unload
            if (_collectorScript.GetUnloadSpeed() < GLOB.COLLECTOR_MAX_UNLOADSPEED)
            {
                _unloadSpeedButtonText.text = "" +
                                              _collectorScript.costFactor *
                                              _collectorScript.GetUnloadSpeed() *
                                              GLOB.COLLECTOR_UNLOADSPEED_BASE_PRICE;
                var from = Mathf.Round(19.5f / _collectorScript.GetUnloadSpeed() * 100) / 100;
                var to = Mathf.Round(19.5f / (_collectorScript.GetUnloadSpeed() + 1) * 100) / 100;
                _unloadSpeedStatusText.text = $"{from:F2} -> {to:F2} sec";
                buttons = true;
            }
            else
            {
                _unloadSpeedButtonCanvas.SetActive(false);
            }

            if (!buttons)
            {
                _noMoreUpdatesButtonText.SetActive(true);
                _speedStatusText.text = _collectorScript.GetSpeedLevel().ToString();
                _capycityInfoText.text = _collectorScript.GetCapacity().ToString();
                _unloadspeedInfoText.text = "" + Mathf.Round(19.5f / _collectorScript.GetUnloadSpeed() * 100) / 100 + " sec";
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
            if (!GameManager.Instance.AdsInitialized || !GameManager.Instance.AdsRewardedLoaded)
            {
                GameManager.Instance.InitializeAds();
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
                Debug.Log("Unity Ads Rewarded Ad Completed");

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