using UnityEngine;

namespace IdleColors.ScriptableObjects
{
    [CreateAssetMenu(fileName = "so_int", menuName = "SO/SO_Int", order = 0)]
    public class SO_Int : ScriptableObject
    {
        public int value;
    }
}