using UnityEngine;

namespace Unstack.Shape
{
    public class ShapeGenerator
    {
        public Vector2[] GeneratePolyline(Core.GameConfig config, Rect playArea)
        {
            int segmentCount = Random.Range(config.minSegments, config.maxSegments + 1);
            Vector2[] points = new Vector2[segmentCount + 1];

            // Start near the center of the play area to encourage overlapping
            float centerX = playArea.center.x;
            float centerY = playArea.center.y;
            float spawnRadius = Mathf.Min(playArea.width, playArea.height) * 0.25f;

            points[0] = new Vector2(
                centerX + Random.Range(-spawnRadius, spawnRadius),
                centerY + Random.Range(-spawnRadius, spawnRadius)
            );

            float currentAngle = Random.Range(0f, 360f);

            for (int i = 1; i <= segmentCount; i++)
            {
                float length = Random.Range(config.minSegmentLength, config.maxSegmentLength);

                // Random turn angle
                float turnAngle = Random.Range(config.minAngle, config.maxAngle);
                if (Random.value > 0.5f) turnAngle = -turnAngle;
                currentAngle += turnAngle;

                float rad = currentAngle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                Vector2 nextPoint = points[i - 1] + direction * length;

                // Clamp to play area
                nextPoint.x = Mathf.Clamp(nextPoint.x, playArea.xMin, playArea.xMax);
                nextPoint.y = Mathf.Clamp(nextPoint.y, playArea.yMin, playArea.yMax);

                points[i] = nextPoint;
            }

            return points;
        }
    }
}
