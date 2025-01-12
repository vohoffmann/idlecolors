using UnityEngine;

namespace IdleColors.camera
{
    public class PointToCam : MonoBehaviour
    {
        private Camera _cam;

        private void Awake()
        {
            _cam = Camera.main;
        }

        void Update()
        {
            if (_cam == null)
            {
                _cam = Camera.main;
                return;
            }
            transform.eulerAngles = _cam.transform.eulerAngles;
        }
    }
}