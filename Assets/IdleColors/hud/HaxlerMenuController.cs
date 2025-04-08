using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.room_mixing.haxler;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace IdleColors.hud
{
    public class HaxlerMenuController : MonoBehaviour, IUnityAdsShowListener
    {
        private HaxlerController _haxlerScript;

        [SerializeField] private GameObject      _speedButtonCanvas;
        private                  Button          _speedButton;
        [SerializeField] private Text            _speedButtonText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Button          _speedbyadsbutton;

        [SerializeField] private GameObject      _noMoreUpdatesLabelText;
        [SerializeField] private TextMeshProUGUI _speedInfoText;

        public void SetHaxler(HaxlerController haxlerScript)
        {
            _haxlerScript = haxlerScript;
            _noMoreUpdatesLabelText.SetActive(false);

            if (!_speedButton)
            {
                _speedButton = _speedButtonText.GetComponent<Button>();
            }

            UpdateButtonText();

            gameObject.SetActive(true);
        }

        private void UpdateButtonText()
        {
            if (_haxlerScript.GetSpeedLevel() < GLOB.HAXLER_SPEED_MAX)
            {
                _speedButtonCanvas.SetActive(true);
                var from = 20 - _haxlerScript.GetSpeedLevel();
                var to   = 20 - (_haxlerScript.GetSpeedLevel() + 1);

                _statusText.text = $"{from} -> {to} seconds";
                // _speedButtonText.text = "" + _haxlerScript.costFactor * _haxlerScript.GetSpeedLevel() *
                // GLOB.HAXLER_SPEED_BASE_PRICE;
                _speedButtonText.text = "" + Mathf.RoundToInt(GLOB.HAXLER_SPEED_BASE_PRICE *
                                                              Mathf.Pow(_haxlerScript.costFactor,
                                                                  _haxlerScript.GetSpeedLevel() - 1));
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
                _noMoreUpdatesLabelText.SetActive(true);
                _speedInfoText.text = "" + (20 - _haxlerScript.GetSpeedLevel()) + " sec";
            }
        }

        public void UpgradeSpeed(bool subCoins = false)
        {
            _haxlerScript.UpgradeSpeed(subCoins);

            UpdateButtonText();
        }

        private void Update()
        {
            if (_haxlerScript == null)
            {
                return;
            }

            _speedButton.interactable = GameManager.Instance.GetCoins() >= Mathf.RoundToInt(
                GLOB.HAXLER_SPEED_BASE_PRICE *
                Mathf.Pow(_haxlerScript.costFactor,
                    _haxlerScript.GetSpeedLevel() - 1));
        }

        public void Close()
        {
            _haxlerScript = null;
            CameraController.Instance.UnsetLockedTarget();
            gameObject.SetActive(false);
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
                UpgradeSpeed();
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