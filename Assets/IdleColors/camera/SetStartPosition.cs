using UnityEngine;

namespace IdleColors.camera
{
    public class SetStartPosition : MonoBehaviour
    {
        private void Awake()
        {
            transform.position = new Vector3(-260, 24, -2.5f);
        }
    }
}
