using System.Collections.Generic;
using UnityEngine;

namespace IdleColors.helper
{
    public static class Helper
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();

        public static WaitForSeconds GetWait(float seconds)
        {
            if (WaitDictionary.TryGetValue(seconds, out var wait))
            {
                return wait;
            }

            WaitDictionary[seconds] = new WaitForSeconds(seconds);
            return WaitDictionary[seconds];
        }
    }
}