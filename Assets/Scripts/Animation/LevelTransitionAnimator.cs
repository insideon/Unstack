using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unstack.Animation
{
    public class LevelTransitionAnimator : MonoBehaviour
    {
        [SerializeField] private Image fadeOverlay;

        private float _transitionDuration = 0.5f;

        public void SetTransitionDuration(float duration)
        {
            _transitionDuration = duration;
        }

        public void PlayTransition(Action onMidpoint)
        {
            if (fadeOverlay == null)
            {
                onMidpoint?.Invoke();
                return;
            }
            StartCoroutine(TransitionCoroutine(onMidpoint));
        }

        private IEnumerator TransitionCoroutine(Action onMidpoint)
        {
            float halfDuration = _transitionDuration / 2f;

            // Fade to black
            fadeOverlay.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                fadeOverlay.color = new Color(0f, 0f, 0f, t);
                yield return null;
            }
            fadeOverlay.color = new Color(0f, 0f, 0f, 1f);

            // Midpoint callback (swap level content)
            onMidpoint?.Invoke();
            yield return null;

            // Fade in
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                fadeOverlay.color = new Color(0f, 0f, 0f, 1f - t);
                yield return null;
            }

            fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
            fadeOverlay.gameObject.SetActive(false);
        }
    }
}
