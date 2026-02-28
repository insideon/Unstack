using UnityEngine;

namespace Unstack.Shape
{
    public static class ShapeOverlapDetector
    {
        public static bool IsCovered(ShapeController target)
        {
            var shapes = Object.FindObjectsByType<ShapeController>(FindObjectsSortMode.None);
            Vector2[] targetPoints = target.GetWorldPoints();

            foreach (var other in shapes)
            {
                if (other == target || !other.IsActive) continue;
                if (other.Data.SortingOrder <= target.Data.SortingOrder) continue;

                Vector2[] otherPoints = other.GetWorldPoints();

                if (PolylinesIntersect(targetPoints, otherPoints))
                    return true;
            }

            return false;
        }

        private static bool PolylinesIntersect(Vector2[] polyA, Vector2[] polyB)
        {
            for (int i = 0; i < polyA.Length - 1; i++)
            {
                for (int j = 0; j < polyB.Length - 1; j++)
                {
                    if (SegmentsIntersect(polyA[i], polyA[i + 1], polyB[j], polyB[j + 1]))
                        return true;
                }
            }
            return false;
        }

        private static bool SegmentsIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float d1 = Cross(b2 - b1, a1 - b1);
            float d2 = Cross(b2 - b1, a2 - b1);
            float d3 = Cross(a2 - a1, b1 - a1);
            float d4 = Cross(a2 - a1, b2 - a1);

            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
                ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
                return true;

            // Collinear cases
            if (Mathf.Approximately(d1, 0f) && OnSegment(b1, b2, a1)) return true;
            if (Mathf.Approximately(d2, 0f) && OnSegment(b1, b2, a2)) return true;
            if (Mathf.Approximately(d3, 0f) && OnSegment(a1, a2, b1)) return true;
            if (Mathf.Approximately(d4, 0f) && OnSegment(a1, a2, b2)) return true;

            return false;
        }

        private static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            return r.x <= Mathf.Max(p.x, q.x) && r.x >= Mathf.Min(p.x, q.x) &&
                   r.y <= Mathf.Max(p.y, q.y) && r.y >= Mathf.Min(p.y, q.y);
        }
    }
}
