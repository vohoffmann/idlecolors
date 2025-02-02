using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.hud
{
    public class OderImageButtonVisibility : MonoBehaviour
    {
        private void OnEnable()
        {
            int visibilityLevel = 1;
            if (GameManager.Instance.so_unlockedGreen.value) visibilityLevel++;
            if (GameManager.Instance.so_unlockedBlue.value) visibilityLevel++;

            foreach (Transform button in transform)
            {
                var idxString = button.name.Split("#")[1];
                var idx = int.Parse(idxString);

                button.gameObject.SetActive(visibilityLevel >= idx);
            }
        }
    }
}