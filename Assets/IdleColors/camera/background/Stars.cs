using UnityEngine;

namespace IdleColors.background
{
    public class Stars : MonoBehaviour
    {
        private MeshRenderer starRenderer;

        void Awake()
        {
            starRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            var mat = starRenderer.material;

            var offset = mat.mainTextureOffset;

            offset.x += .08f * Time.unscaledDeltaTime;
            offset.y += .03f * Time.unscaledDeltaTime;

            mat.mainTextureOffset = offset;
        }
    }
}