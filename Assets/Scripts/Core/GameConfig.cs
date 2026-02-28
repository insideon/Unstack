using UnityEngine;

namespace Unstack.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Unstack/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Lives")]
        public int maxHearts = 3;

        [Header("Level Progression")]
        public int baseShapeCount = 5;
        public int shapesPerLevelIncrease = 2;

        [Header("Shape Generation")]
        public int minSegments = 2;
        public int maxSegments = 5;
        public float minSegmentLength = 0.5f;
        public float maxSegmentLength = 2.0f;
        public float minAngle = 30f;
        public float maxAngle = 150f;
        public float lineWidth = 0.15f;
        public float spawnAreaPadding = 0.5f;

        [Header("Colors")]
        public Color[] colorPalette = new Color[]
        {
            new Color(0.91f, 0.30f, 0.24f), // Red
            new Color(0.16f, 0.50f, 0.73f), // Blue
            new Color(0.18f, 0.80f, 0.44f), // Green
            new Color(0.95f, 0.77f, 0.06f), // Yellow
            new Color(0.56f, 0.27f, 0.68f), // Purple
            new Color(0.90f, 0.49f, 0.13f), // Orange
            new Color(0.10f, 0.74f, 0.61f), // Teal
            new Color(0.83f, 0.33f, 0.61f), // Pink
        };

        [Header("Animation")]
        public float fadeDuration = 0.4f;
        public float floatDistance = 1.5f;
        public float shakeDuration = 0.4f;
        public float shakeIntensity = 0.15f;

        public int GetShapeCount(int level)
        {
            return baseShapeCount + (level - 1) * shapesPerLevelIncrease;
        }
    }
}
