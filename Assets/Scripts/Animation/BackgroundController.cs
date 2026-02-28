using UnityEngine;

namespace Unstack.Animation
{
    public class BackgroundController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        public void Initialize(Color topColor, Color bottomColor)
        {
            // Create gradient texture (1x256)
            var texture = new Texture2D(1, 256, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < 256; y++)
            {
                float t = (float)y / 255f;
                texture.SetPixel(0, y, Color.Lerp(bottomColor, topColor, t));
            }
            texture.Apply();

            // Create sprite from texture
            var sprite = Sprite.Create(texture,
                new Rect(0, 0, 1, 256),
                new Vector2(0.5f, 0.5f),
                1f);

            _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = -1000;

            // Scale to fill screen
            FitToScreen();
        }

        private void FitToScreen()
        {
            var cam = Camera.main;
            if (cam == null) return;

            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;

            // Sprite is 1x256 with 1 PPU, so natural size is 1 x 256 world units
            transform.localScale = new Vector3(camWidth + 1f, camHeight / 256f + 0.01f, 1f);
            transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 10f);
        }
    }
}
