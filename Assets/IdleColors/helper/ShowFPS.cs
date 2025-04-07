using UnityEngine;
using UnityEngine.UI;

namespace IdleColors.helper
{
    public class ShowFPS : MonoBehaviour
    {
        public  Text  fpsText;
        private float _deltaTime;

        void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            fpsText.text = "fps : " + (int)Mathf.Ceil(fps);
        }
    }
}