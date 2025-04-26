using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.room_mixing.puffer;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace IdleColors.hud
{
    public class PufferMenuController : MonoBehaviour, IUnityAdsShowListener
    {
        private PufferController _pufferScript;

        [SerializeField] private GameObject      _capacityButtonCanvas;
        [SerializeField] private TextMeshProUGUI _statusText;
        private                  Button          _capacityButton;
        [SerializeField] private Text            _capacityText;
        [SerializeField] private Button          _speedbyadsbutton;

        [SerializeField] private GameObject      _noMoreUpdatesLabelText;
        [SerializeField] private TextMeshProUGUI _capacityInfoText;

        public void setPufferScript(PufferController pufferScript)
        {
            _pufferScript = pufferScript;
            _noMoreUpdatesLabelText.SetActive(false);

            if (!_capacityButton)
            {
                _capacityButton = _capacityText.GetComponent<Button>();
            }

            UpdateButtonText();
            gameObject.SetActive(true);
        }

        private void UpdateButtonText()
        {
            // capacity
            if (_pufferScript.GetLevel() < GLOB.PUFFER_LEVEL_MAX)
            {
                _capacityButtonCanvas.SetActive(true);
                var from = _pufferScript.GetLevel() * 24;
                var to   = (_pufferScript.GetLevel() + 1) * 24;
                _statusText.text = $"{from} -> {to}";
                // _capacityText.text = "" + _pufferScript.costFactor * _pufferScript.GetLevel() *
                //     GLOB.PUFFER_CAPACITY_BASE_PRICE;

                _capacityText.text = "" + Mathf.RoundToInt(GLOB.PUFFER_CAPACITY_BASE_PRICE *
                                                           Mathf.Pow(_pufferScript.costFactor,
                                                               _pufferScript.GetLevel() - 1));
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
                _capacityButtonCanvas.SetActive(false);
                _noMoreUpdatesLabelText.SetActive(true);
                _capacityInfoText.text = "" + _pufferScript.GetLevel() * 24;
            }
        }

        // update the buttons visibility regarding the coins
        private void Update()
        {
            if (_pufferScript == null)
            {
                return;
            }

            _capacityButton.interactable = GameManager.Instance.GetCoins() >= Mathf.RoundToInt(
                GLOB.PUFFER_CAPACITY_BASE_PRICE *
                Mathf.Pow(_pufferScript.costFactor,
                    _pufferScript.GetLevel() - 1));
        }

        public void UpgradeCapacity(bool subCoins = false)
        {
            _pufferScript.UpgradeCapacity(subCoins);
            UpdateButtonText();
        }

        public void Close()
        {
            _pufferScript = null;
            CameraController.Instance.UnsetLockedTarget();
            gameObject.SetActive(false);
        }

        public void ShowAdsToUpgradeCapacity()
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
                UpgradeCapacity();
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