using UnityEngine;
using Unstack.Animation;

namespace Unstack.Shape
{
    public class ShapeController : MonoBehaviour
    {
        public ShapeData Data { get; private set; }
        public ShapeAnimator Animator { get; private set; }

        private LineRenderer _lineRenderer;
        private EdgeCollider2D _edgeCollider;
        private bool _isActive = true;

        public bool IsActive => _isActive;

        public void Initialize(ShapeData data)
        {
            Data = data;

            // LineRenderer setup
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.startWidth = data.LineWidth;
            _lineRenderer.endWidth = data.LineWidth;
            _lineRenderer.numCornerVertices = 5;
            _lineRenderer.numCapVertices = 5;
            _lineRenderer.startColor = data.Color;
            _lineRenderer.endColor = data.Color;
            _lineRenderer.sortingOrder = data.SortingOrder;

            // Use default sprite material
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.material.color = Color.white;

            // Set positions
            _lineRenderer.positionCount = data.Points.Length;
            for (int i = 0; i < data.Points.Length; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(data.Points[i].x, data.Points[i].y, 0));
            }

            // EdgeCollider2D setup
            _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            _edgeCollider.points = data.Points;
            _edgeCollider.edgeRadius = data.LineWidth / 2f;
            _edgeCollider.isTrigger = true;

            // Animator
            Animator = gameObject.AddComponent<ShapeAnimator>();
            Animator.Initialize(_lineRenderer);
        }

        public bool IsCovered()
        {
            return ShapeOverlapDetector.IsCovered(this);
        }

        public void SetActive(bool active)
        {
            _isActive = active;
        }

        public Vector2[] GetWorldPoints()
        {
            Vector2[] worldPoints = new Vector2[Data.Points.Length];
            for (int i = 0; i < Data.Points.Length; i++)
            {
                worldPoints[i] = (Vector2)transform.position + Data.Points[i];
            }
            return worldPoints;
        }
    }
}
