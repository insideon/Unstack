using UnityEngine;
using UnityEngine.UI;

namespace Unstack.UI
{
    public class HeartsDisplay : MonoBehaviour
    {
        [SerializeField] private Image[] heartImages;
        [SerializeField] private Color activeColor = new Color(0.91f, 0.30f, 0.24f);
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

        public void UpdateHearts(int currentHearts)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                heartImages[i].color = i < currentHearts ? activeColor : inactiveColor;
            }
        }
    }
}
