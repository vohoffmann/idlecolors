using System;
using UnityEngine;

namespace IdleColors.Globals
{
    public class EventManager : MonoBehaviour
    {
        public static Action<GameObject> BoxStored;
        public static Action SetBoxPosition;
    }
}