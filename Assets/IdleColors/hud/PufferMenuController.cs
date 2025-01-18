using IdleColors.camera;
using IdleColors.Globals;
using IdleColors.room_mixing.puffer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleColors.hud
{
    public class PufferMenuController : MonoBehaviour
    {
        private PufferController _pufferScript;

        [SerializeField] private GameObject _capacityButtonCanvas;
        [SerializeField] private TextMeshProUGUI _statusText;
        private Button _capacityButton;
        [SerializeField] private Text _capacityText;

        [SerializeField] private GameObject _noMoreUpdatesLabelText;
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
                var to = (_pufferScript.GetLevel() + 1) * 24;
                _statusText.text = $"{from} -> {to}";
                _capacityText.text = "" + _pufferScript.costFactor * _pufferScript.GetLevel() *
                    GLOB.PUFFER_CAPACITY_BASE_PRICE;
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

            _capacityButton.interactable = GameManager.Instance.GetCoins() >= _pufferScript.costFactor *
                _pufferScript.GetLevel() * GLOB.PUFFER_CAPACITY_BASE_PRICE;
        }

        public void UpgradeCapacity()
        {
            _pufferScript.UpgradeCapacity();
            UpdateButtonText();
        }

        public void Close()
        {
            _pufferScript = null;
            CameraController.Instance.UnsetLockedTarget();
            gameObject.SetActive(false);
        }
    }
}