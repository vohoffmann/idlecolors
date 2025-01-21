using UnityEngine;

namespace IdleColors.hud
{
    [ExecuteAlways]
    public class ScaleButton : MonoBehaviour
    {
        public Vector3 fromScale = new Vector3(1f, 1f, 1f);
        public Vector3 toScale = new Vector3(1.2f, 1.2f, 1f);
        public float scaleSpeed = 3f;
        private RectTransform rectTransform;
        bool isScaling = true;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError("No RectTransform found on the button!");
            }
        }


        void Update()
        {
            if (isScaling)
            {
                rectTransform.localScale =
                    Vector3.Lerp(rectTransform.localScale, toScale, Time.deltaTime * scaleSpeed);

                if (Vector3.Distance(rectTransform.localScale, toScale) < 0.01f)
                {
                    rectTransform.localScale = toScale;
                    isScaling = false;
                }
            }
            else
            {
                rectTransform.localScale =
                    Vector3.Lerp(rectTransform.localScale, fromScale, Time.deltaTime * scaleSpeed);

                if (Vector3.Distance(rectTransform.localScale, fromScale) < 0.01f)
                {
                    rectTransform.localScale = fromScale;
                    isScaling = true;
                }
            }
        }
    }
}