using UnityEngine;
using TMPro;

namespace Unstack.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;

        public void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = score.ToString();
        }

        public void UpdateCombo(int combo)
        {
            if (comboText == null) return;

            if (combo >= 2)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = $"x{combo} Combo!";
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }
}
