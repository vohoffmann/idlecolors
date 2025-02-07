using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.hud
{
    public class AdditionalElementsVisibility : MonoBehaviour
    {
        [SerializeField] private GameObject _OrderImageButton;
        [SerializeField] private GameObject _ClaimImageRewardsButton;
        [SerializeField] private GameObject _OrderImageCamView;
        
        private void OnEnable()
        {
            InvokeRepeating(nameof(CheckVisibilities), 0, 1);
        }
        
        private void CheckVisibilities()
        {
            _OrderImageButton.SetActive(!GameManager.Instance.ImageOrderInProcess);
            _ClaimImageRewardsButton.SetActive(!GameManager.Instance.ImageOrderInProcess && GameManager.Instance.ImageOrderRewards != 0);
            _OrderImageCamView.SetActive(GameManager.Instance.ImageOrderInProcess || GameManager.Instance.ImageOrderRewards != 0);
        }
    }
}