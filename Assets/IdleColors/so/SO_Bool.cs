using UnityEngine;

namespace IdleColors.ScriptableObjects
{
    [CreateAssetMenu(fileName = "so-bool", menuName = "SO/SO_Bool", order = 0)]
    public class SO_Bool : ScriptableObject
    {
        public bool value;
    }
}