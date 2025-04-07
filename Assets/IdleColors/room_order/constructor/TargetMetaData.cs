using UnityEngine;

namespace IdleColors.room_order.constructor
{
    [System.Serializable]
    public class TargetMetaData
    {
        public Vector3 pufferPosition;
        public Vector3 cubePosition;
        public int     colorIndex;
        public bool    done;
    }
}