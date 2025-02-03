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
        }

        private void FixedUpdate()
        {
            if (transform.localPosition.y < -30)
            {
                Destroy(gameObject);
            }
        }
    }
}