using IdleColors.Globals;
using UnityEngine;

namespace IdleColors.hud
{
    /*
     * geht alle buttons in der orderimage ansicht durch und setzt die sichbarkeit abhänig davon,
     * ob die farben bereits zur verfügung stehen
     */
    public class GenerateImageRasterButtonVisibility : MonoBehaviour
    {
        private void OnEnable()
        {
            int visibilityLevel = 1;
            if (GameManager.Instance.so_unlockedGreen.value) visibilityLevel++;
            if (GameManager.Instance.so_unlockedBlue.value) visibilityLevel++;

            foreach (Transform button in transform)
            {
                var idxString = button.name.Split("#")[1];
                var idx       = int.Parse(idxString);

                button.gameObject.GetComponents<UnityEngine.UI.Button>()[0].interactable = visibilityLevel >= idx;
                button.gameObject.transform.GetChild(2).gameObject.SetActive(visibilityLevel < idx);
            }
        }
    }
}