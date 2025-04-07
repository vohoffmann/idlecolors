using UnityEngine;

namespace IdleColors.camera.background
{
    public class Stars : MonoBehaviour
    {
        private MeshRenderer _starRenderer;

        void Awake()
        {
            _starRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            var mat = _starRenderer.material;

            var offset = mat.mainTextureOffset;

            offset.x += .08f * Time.unscaledDeltaTime;
            offset.y += .03f * Time.unscaledDeltaTime;

            mat.mainTextureOffset = offset;
        }
    }
}