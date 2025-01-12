using UnityEditor;
using UnityEngine;

namespace IdleColors.hud.coin
{
     [ExecuteAlways]
    public class CoinController : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(0, 200 * Time.deltaTime, 0);
            var localScale = transform.localScale;

            if (localScale.x > 36)
            {
                var scaleSpeed = 50 * Time.deltaTime;
                localScale = new Vector3(localScale.x - scaleSpeed, localScale.y - scaleSpeed, localScale.z - scaleSpeed);
                transform.localScale = localScale;
            }
        }

        public void TriggerScaling()
        {
            transform.localScale = new Vector3(65, 65, 65);
        }
#if UNITY_EDITOR
        private void OnEnable()
        {
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }
#endif
    }
}