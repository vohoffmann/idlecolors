using UnityEngine;

namespace IdleColors.camera
{
    public class SetStartPosition : MonoBehaviour
    {
        private void Awake()
        {
            transform.position = new Vector3(-260, 19, -2.5f);
        }
    }
}
