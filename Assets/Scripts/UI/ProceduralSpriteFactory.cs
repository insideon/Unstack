using UnityEngine;

namespace Unstack.UI
{
    public static class ProceduralSpriteFactory
    {
        public static Sprite CreateHeart()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Normalize to [-1, 1]
                    float u = (x + 0.5f) / size * 2f - 1f;
                    float v = (y + 0.5f) / size * 2f - 1f;

                    // Heart SDF: two circles at top + rotated square at bottom
                    // Top-left circle center (-0.35, 0.25), radius 0.4
                    // Top-right circle center (0.35, 0.25), radius 0.4
                    float d1 = new Vector2(u + 0.35f, v - 0.25f).magnitude - 0.4f;
                    float d2 = new Vector2(u - 0.35f, v - 0.25f).magnitude - 0.4f;

                    // Rotated square (diamond) for bottom part
                    float ru = Mathf.Abs(u);
                    float rv = -v + 0.05f;
                    float d3 = (ru + rv - 0.75f) * 0.707f;

                    // Union of all shapes
                    float d = Mathf.Min(Mathf.Min(d1, d2), d3);

                    float alpha = SdfAlpha(d * size);
                    pixels[y * size + x] = new Color(1, 1, 1, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        public static Sprite CreateRoundedRect(int w, int h, int r)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color[w * h];
            float radius = Mathf.Min(r, Mathf.Min(w, h) / 2f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float d = SdfRoundedRect(x + 0.5f, y + 0.5f, w, h, radius);
                    float alpha = SdfAlpha(d);
                    pixels[y * w + x] = new Color(1, 1, 1, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;

            float border = Mathf.Min(radius, 16);
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100,
                0, SpriteMeshType.FullRect, new Vector4(border, border, border, border));
        }

        public static Sprite CreatePlayTriangle()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];

            // Right-pointing triangle vertices (in pixel space, centered)
            Vector2 a = new Vector2(16, 8);   // bottom-left
            Vector2 b = new Vector2(16, 56);  // top-left
            Vector2 c = new Vector2(54, 32);  // right center

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    float d = SdfTriangle(p, a, b, c);
                    float alpha = SdfAlpha(d);
                    pixels[y * size + x] = new Color(1, 1, 1, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        public static Sprite CreateGear()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            float cx = size / 2f;
            float cy = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float angle = Mathf.Atan2(dy, dx);

                    // Outer radius with 8 teeth using sin wave
                    float teeth = Mathf.Sin(angle * 8) * 3f;
                    float outerR = 26f + teeth;
                    float innerR = 10f;

                    // Ring SDF: inside outer, outside inner
                    float dOuter = dist - outerR;
                    float dInner = innerR - dist;
                    float d = Mathf.Max(dOuter, dInner);

                    float alpha = SdfAlpha(d);
                    pixels[y * size + x] = new Color(1, 1, 1, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        public static Sprite CreatePauseBars()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];

            // Two vertical bars: left (18-26), right (38-46), vertical (12-52)
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;

                    float d1 = SdfBox(px - 22f, py - 32f, 5f, 20f);
                    float d2 = SdfBox(px - 42f, py - 32f, 5f, 20f);
                    float d = Mathf.Min(d1, d2);

                    float alpha = SdfAlpha(d);
                    pixels[y * size + x] = new Color(1, 1, 1, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        public static Sprite CreateArrowDown()
        {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];

            // Arrow = vertical stem + downward triangle head
            // Stem: centered horizontal (28-36), vertical (30-58)
            // Triangle head: pointing down, vertices at (12,30), (52,30), (32,6)
            Vector2 ta = new Vector2(12, 28);  // bottom-left of triangle (flipped because y=0 is bottom)
            Vector2 tb = new Vector2(52, 28);  // bottom-right
            Vector2 tc = new Vector2(32, 6);   // bottom tip

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;

                    // Stem (vertical rectangle)
                    float dStem = SdfBox(px - 32f, py - 44f, 6f, 16f);

                    // Downward triangle head
                    Vector2 p = new Vector2(px, py);
                    float dHead = SdfTriangle(p, ta, tb, tc);

                    float d = Mathf.Min(dStem, dHead);
                    float alpha = SdfAlpha(d);
                    pixels[y * size + x] = new Color(1, 1, 1, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        // --- SDF Helpers ---

        private static float SdfAlpha(float d)
        {
            return Mathf.Clamp01(-d * 0.5f + 0.5f);
        }

        private static float SdfRoundedRect(float px, float py, float w, float h, float r)
        {
            // Distance from point to rounded rectangle centered in [0,w]x[0,h]
            float hw = w * 0.5f;
            float hh = h * 0.5f;
            float dx = Mathf.Abs(px - hw) - (hw - r);
            float dy = Mathf.Abs(py - hh) - (hh - r);
            float outside = new Vector2(Mathf.Max(dx, 0), Mathf.Max(dy, 0)).magnitude;
            float inside = Mathf.Min(Mathf.Max(dx, dy), 0);
            return outside + inside - r;
        }

        private static float SdfBox(float dx, float dy, float hw, float hh)
        {
            float qx = Mathf.Abs(dx) - hw;
            float qy = Mathf.Abs(dy) - hh;
            return new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude
                   + Mathf.Min(Mathf.Max(qx, qy), 0);
        }

        private static float SdfTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 e0 = b - a, e1 = c - b, e2 = a - c;
            Vector2 v0 = p - a, v1 = p - b, v2 = p - c;

            Vector2 pq0 = v0 - e0 * Mathf.Clamp01(Vector2.Dot(v0, e0) / Vector2.Dot(e0, e0));
            Vector2 pq1 = v1 - e1 * Mathf.Clamp01(Vector2.Dot(v1, e1) / Vector2.Dot(e1, e1));
            Vector2 pq2 = v2 - e2 * Mathf.Clamp01(Vector2.Dot(v2, e2) / Vector2.Dot(e2, e2));

            float s = Mathf.Sign(e0.x * e2.y - e0.y * e2.x);
            Vector2 d0 = new Vector2(Vector2.Dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x));
            Vector2 d1 = new Vector2(Vector2.Dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x));
            Vector2 d2 = new Vector2(Vector2.Dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x));

            Vector2 dMin = d0;
            if (d1.x < dMin.x) dMin = d1;
            if (d2.x < dMin.x) dMin = d2;

            return -Mathf.Sqrt(dMin.x) * Mathf.Sign(dMin.y);
        }
    }
}
