using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unstack.UI
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private Image overlayBackground;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private RectTransform arrowTransform;

        private void OnEnable()
        {
            if (arrowTransform != null)
                StartCoroutine(ArrowBobbingAnimation());
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (overlayBackground != null)
            {
                overlayBackground.color = new Color(0f, 0f, 0f, 0.4f);
                overlayBackground.raycastTarget = false;
            }

            if (instructionText != null)
            {
                instructionText.text = "Tap the topmost shape\nto unstack!";
                instructionText.raycastTarget = false;
            }
        }

        public void Dismiss()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        private IEnumerator ArrowBobbingAnimation()
        {
            if (arrowTransform == null) yield break;

            Vector2 startPos = arrowTransform.anchoredPosition;
            float bobbingAmount = 15f;
            float speed = 2f;

            while (true)
            {
                float offset = Mathf.Sin(Time.time * speed) * bobbingAmount;
                arrowTransform.anchoredPosition = startPos + Vector2.down * offset;
                yield return null;
            }
        }
    }
}
