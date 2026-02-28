using UnityEngine;
using UnityEngine.InputSystem;
using Unstack.Core;
using Unstack.Shape;

namespace Unstack.InputHandling
{
    public class TouchInputHandler : MonoBehaviour
    {
        private InputActionAsset _inputAsset;
        private InputAction _attackAction;
        private InputAction _pointAction;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;

            _inputAsset = ScriptableObject.CreateInstance<InputActionAsset>();

            var playerMap = _inputAsset.AddActionMap("Player");
            _attackAction = playerMap.AddAction("Attack", InputActionType.Button);
            _attackAction.AddBinding("<Mouse>/leftButton");
            _attackAction.AddBinding("<Touchscreen>/primaryTouch/tap");

            var uiMap = _inputAsset.AddActionMap("UI");
            _pointAction = uiMap.AddAction("Point", InputActionType.PassThrough);
            _pointAction.AddBinding("<Mouse>/position");
            _pointAction.AddBinding("<Touchscreen>/primaryTouch/position");
        }

        private void OnEnable()
        {
            if (_inputAsset == null) return;
            _inputAsset.Enable();
            _attackAction.performed += OnTap;
        }

        private void OnDisable()
        {
            if (_attackAction == null) return;
            _attackAction.performed -= OnTap;
            _inputAsset.Disable();
        }

        private void OnDestroy()
        {
            if (_inputAsset != null)
            {
                _inputAsset.Disable();
                Destroy(_inputAsset);
            }
        }

        private void OnTap(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = _pointAction.ReadValue<Vector2>();
            Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);

            Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
            if (hits.Length == 0) return;

            ShapeController topShape = null;
            int highestOrder = int.MinValue;

            foreach (var hit in hits)
            {
                var shape = hit.GetComponent<ShapeController>();
                if (shape != null && shape.IsActive && shape.Data.SortingOrder > highestOrder)
                {
                    highestOrder = shape.Data.SortingOrder;
                    topShape = shape;
                }
            }

            if (topShape != null)
            {
                GameManager.Instance.OnShapeTapped(topShape);
            }
        }
    }
}
