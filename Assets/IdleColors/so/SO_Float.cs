using UnityEngine;

namespace IdleColors.ScriptableObjects
{
    [CreateAssetMenu(fileName = "so_float", menuName = "SO/SO_Float", order = 0)]
    public class SO_Float : ScriptableObject
    {
        public float value;
    }
}