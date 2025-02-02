using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.hud
{
    public class ButtonVisibilityController : MonoBehaviour
    {
        [SerializeField] private GameObject _OrderImageButton;
        
        private void OnEnable()
        {
            InvokeRepeating(nameof(CheckVisibilities), 0, 1);
        }
        
        private void CheckVisibilities()
        {
            _OrderImageButton.SetActive(!GameManager.Instance.ImageOrderInProcess);
        }
    }
}