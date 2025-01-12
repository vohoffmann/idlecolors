using UnityEngine;
using UnityEngine.UI;
using IdleColors.Globals;

namespace IdleColors.helper
{
    public class MoneyTransparency : MonoBehaviour
    {
        private Image _image;
        private Button _button;
        private bool _transDirection;
        private float _velocity;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponentInChildren<Button>();
        }

        void Update()
        {
            if (_image == null ||
                _button == null
               )
            {
                GameManager.LogError("no image or Button : " + gameObject);
                return;
            }

            var color = _image.color;

            if (!_button.interactable)
            {
                color.a = .3f;
                _image.color = color;
            }

            if (_transDirection)
            {
                color.a = Mathf.SmoothDamp(color.a, 1, ref _velocity, 0.2f);
                if (color.a >= .95f)
                {
                    _transDirection = !_transDirection;
                    return;
                }
            }
            else
            {
                color.a = Mathf.SmoothDamp(color.a, .4f, ref _velocity, 0.2f);
                if (color.a <= .5f)
                {
                    _transDirection = !_transDirection;
                    return;
                }
            }

            _image.color = color;
        }
    }
}