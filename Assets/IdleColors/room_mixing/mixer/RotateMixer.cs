using UnityEngine;

namespace IdleColors.room_mixing.mixer
{
    public class RotateMixer : MonoBehaviour
    {
        private void Start()
        {
            enabled = false;
        }

        private void Update()
        {
            transform.Rotate(0, 180f * Time.deltaTime, 0);
        }
    }
}