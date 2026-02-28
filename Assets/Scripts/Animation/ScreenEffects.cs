using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unstack.Animation
{
    public class ScreenEffects : MonoBehaviour
    {
        public static ScreenEffects Instance { get; private set; }

        [SerializeField] private Image flashOverlay;

        private float _flashDuration = 0.3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetFlashDuration(float duration)
        {
            _flashDuration = duration;
        }

        public void FlashRed()
        {
            if (flashOverlay == null) return;
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine(new Color(1f, 0f, 0f, 0.3f)));
        }

        public void FlashWhite()
        {
            if (flashOverlay == null) return;
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine(new Color(1f, 1f, 1f, 0.3f)));
        }

        private IEnumerator FlashCoroutine(Color flashColor)
        {
            flashOverlay.color = flashColor;
            flashOverlay.gameObject.SetActive(true);

            float elapsed = 0f;
            while (elapsed < _flashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _flashDuration;
                Color c = flashColor;
                c.a = Mathf.Lerp(flashColor.a, 0f, t);
                flashOverlay.color = c;
                yield return null;
            }

            flashOverlay.gameObject.SetActive(false);
        }
    }
}
