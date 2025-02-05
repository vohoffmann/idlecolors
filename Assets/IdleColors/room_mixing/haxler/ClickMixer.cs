using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleColors.room_mixing.haxler
{
    public class ClickMixer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _image;
        private Color _color;

        public void OnPointerClick(PointerEventData eventData)
        {
            // das obere menu aufflaschen
            Debug.Log("mixer clicked");
            if (_image != null)
            {
                _color = Color.white;
            }
        }

        private void Update()
        {
            if (_color.r > .23f)
            {
                _image.color = _color;
                _color.r -= Time.deltaTime;
                _color.g -= Time.deltaTime;
                _color.b -= Time.deltaTime;
            }
        }
    }
}