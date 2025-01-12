using UnityEngine;

namespace IdleColors.room_collect.collector
{
    public class SlowRotation : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(.25f, .7f, .3f);
        }
    }
}