using UnityEngine;

namespace Unstack.Shape
{
    public class ShapeData
    {
        public Vector2[] Points { get; }
        public Color Color { get; }
        public int SortingOrder { get; }
        public float LineWidth { get; }

        public ShapeData(Vector2[] points, Color color, int sortingOrder, float lineWidth)
        {
            Points = points;
            Color = color;
            SortingOrder = sortingOrder;
            LineWidth = lineWidth;
        }
    }
}
