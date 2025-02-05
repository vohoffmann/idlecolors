using TMPro;
using UnityEngine;

namespace IdleColors.hud
{
    public class FlashingText : MonoBehaviour
    {
        private float _alpha;
        private bool _up;
        private Color _color;

        private void Awake()
        {
            _color = GetComponent<TextMeshProUGUI>().color;
        }

        private void OnEnable()
        {
            InvokeRepeating(nameof(Flash), 2, 5);
            _alpha = 0f;
            GetComponent<TextMeshProUGUI>().color = new Color(_color.r, _color.g, _color.b, _alpha);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(Flash));
        }

        private void Flash()
        {
            _up = true;
        }

        private void Update()
        {
            if (_up)
            {
                _alpha += Time.deltaTime * 1;
                if (_alpha >= .8f)
                {
                    _up = false;
                }
            }

            if (!_up && _alpha > .3f)
            {
                _alpha -= Time.deltaTime * .5f;
            }
            
            GetComponent<TextMeshProUGUI>().color = new Color(_color.r, _color.g, _color.b, _alpha);
        }
    }
}