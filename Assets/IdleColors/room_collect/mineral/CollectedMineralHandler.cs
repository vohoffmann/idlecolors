using UnityEngine;

namespace IdleColors.mineral
{
    public class CollectedMineralHandler : MonoBehaviour
    {
        private void Update()
        {
            if (transform.position.y < -3)
            {
                Destroy(gameObject);
            }
        }
    }
}