using UnityEngine;
using Unstack.Shape;

namespace Unstack.Core
{
    public class LevelManager : MonoBehaviour
    {
        private ShapeGenerator _shapeGenerator;
        private Transform _shapesParent;

        private void Awake()
        {
            _shapeGenerator = new ShapeGenerator();

            _shapesParent = new GameObject("Shapes").transform;
            _shapesParent.SetParent(transform);
        }

        public void GenerateLevel(int level)
        {
            ClearShapes();

            var config = GameManager.Instance.Config;
            int shapeCount = config.GetShapeCount(level);

            Camera cam = Camera.main;
            float camHeight = cam.orthographicSize * 2f;
            float camWidth = camHeight * cam.aspect;
            float padding = config.spawnAreaPadding;
            Rect playArea = new Rect(
                -camWidth / 2f + padding,
                -camHeight / 2f + padding,
                camWidth - padding * 2f,
                camHeight - padding * 2f
            );

            for (int i = 0; i < shapeCount; i++)
            {
                Vector2[] points = _shapeGenerator.GeneratePolyline(config, playArea);
                Color color = config.colorPalette[i % config.colorPalette.Length];

                GameObject shapeGO = new GameObject($"Shape_{i}");
                shapeGO.transform.SetParent(_shapesParent);
                shapeGO.transform.position = new Vector3(0, 0, -i * 0.01f);

                var controller = shapeGO.AddComponent<ShapeController>();
                controller.Initialize(new ShapeData(points, color, i, config.lineWidth));
            }
        }

        private void ClearShapes()
        {
            for (int i = _shapesParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_shapesParent.GetChild(i).gameObject);
            }
        }
    }
}
