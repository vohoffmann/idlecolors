using System.Collections;
using TMPro;
using UnityEngine;

namespace IdleColors.hud
{
    public class FloatingText : MonoBehaviour
    {
        public float moveSpeed = 1f; // Geschwindigkeit des Aufsteigens
        public float fadeDuration = 1f; // Dauer bis der Text verschwindet

        private TextMeshProUGUI textMesh;
        private Color startColor;

        void Start()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            startColor = textMesh.color;
            StartCoroutine(FadeAndMove());
        }

        IEnumerator FadeAndMove()
        {
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;

            while (elapsedTime < fadeDuration)
            {
                transform.position = startPosition + new Vector3(0, elapsedTime * moveSpeed, 0);
                textMesh.color = new Color(startColor.r, startColor.g, startColor.b, 1 - (elapsedTime / fadeDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(transform.parent.gameObject); 
        }
    }
}