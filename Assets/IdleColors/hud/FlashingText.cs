using TMPro;
using UnityEngine;

namespace IdleColors.hud
{
    public class FlashingText : MonoBehaviour
    {
        private float _alpha;
        private bool _up;

        private void OnEnable()
        {
            InvokeRepeating(nameof(Flash), 2, 3);
            _alpha = 0f;
            var color = GetComponent<TextMeshProUGUI>().color;
            GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, _alpha);
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
            var color = GetComponent<TextMeshProUGUI>().color;
            GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, _alpha);
        }
    }
}