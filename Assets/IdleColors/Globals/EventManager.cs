using System;
using UnityEngine;

namespace IdleColors.Globals
{
    public class EventManager : MonoBehaviour
    {
        public static Action<GameObject> CupStored;
        public static Action SetBoxPosition;
    }
}