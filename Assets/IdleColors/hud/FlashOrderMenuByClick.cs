using IdleColors.Globals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleColors.room_mixing.haxler
{
    public class ClickMixer : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            EventManager.FlashOrderMenu.Invoke();
        }
    }
}