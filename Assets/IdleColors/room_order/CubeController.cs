using UnityEngine;

namespace IdleColors.room_order
{
    public class CubeController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var color = GetComponent<Renderer>().material.color;
            color.a = 1;
            GetComponent<Renderer>().material.color = color;
            // GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
