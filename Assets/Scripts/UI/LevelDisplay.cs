using TMPro;
using UnityEngine;

namespace Unstack.UI
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;

        public void UpdateLevel(int level)
        {
            levelText.text = $"Level {level}";
        }
    }
}
