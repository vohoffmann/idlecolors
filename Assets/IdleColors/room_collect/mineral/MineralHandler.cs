using UnityEngine;

namespace IdleColors.mineral
{
    public class MineralHandler : MonoBehaviour
    {
        public bool targeted;
        public bool dead;

        private void Update()
        {
            if (transform.position.y < -3)
            {
                dead = true;
            }
        }
    }
}