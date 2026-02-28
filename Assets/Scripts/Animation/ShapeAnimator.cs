using System;
using System.Collections;
using UnityEngine;

namespace Unstack.Animation
{
    public class ShapeAnimator : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private Color _originalColor;

        public void Initialize(LineRenderer lineRenderer)
        {
            _lineRenderer = lineRenderer;
            _originalColor = lineRenderer.startColor;
        }

        public void PlayFadeOut(Action onComplete = null)
        {
            var config = Core.GameManager.Instance.Config;
            StartCoroutine(FadeOutCoroutine(config.fadeDuration, config.floatDistance, onComplete));
        }

        public void PlayShake()
        {
            var config = Core.GameManager.Instance.Config;
            StartCoroutine(ShakeCoroutine(config.shakeDuration, config.shakeIntensity));
        }

        private IEnumerator FadeOutCoroutine(float duration, float floatDistance, Action onComplete)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * floatDistance;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = EaseOutCubic(t);

                // Move up
                transform.position = Vector3.Lerp(startPos, endPos, eased);

                // Fade alpha
                float alpha = 1f - t;
                Color c = _originalColor;
                c.a = alpha;
                _lineRenderer.startColor = c;
                _lineRenderer.endColor = c;

                yield return null;
            }

            onComplete?.Invoke();
        }

        private IEnumerator ShakeCoroutine(float duration, float intensity)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Damped sine wave
                float damping = 1f - t;
                float offset = Mathf.Sin(t * Mathf.PI * 8f) * intensity * damping;

                transform.localPosition = originalPos + Vector3.right * offset;
                yield return null;
            }

            transform.localPosition = originalPos;
        }

        private static float EaseOutCubic(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }
    }
}
