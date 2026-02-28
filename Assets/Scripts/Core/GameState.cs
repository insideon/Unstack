using System;

namespace Unstack.Core
{
    public class GameState
    {
        public event Action<int> OnHeartsChanged;
        public event Action OnGameOver;
        public event Action<int> OnLevelChanged;
        public event Action<int> OnShapesRemainingChanged;
        public event Action OnLevelCleared;

        private int _hearts;
        private int _currentLevel;
        private int _shapesRemaining;

        public int Hearts
        {
            get => _hearts;
            set
            {
                _hearts = value;
                OnHeartsChanged?.Invoke(_hearts);
                if (_hearts <= 0)
                    OnGameOver?.Invoke();
            }
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                OnLevelChanged?.Invoke(_currentLevel);
            }
        }

        public int ShapesRemaining
        {
            get => _shapesRemaining;
            set
            {
                _shapesRemaining = value;
                OnShapesRemainingChanged?.Invoke(_shapesRemaining);
                if (_shapesRemaining <= 0)
                    OnLevelCleared?.Invoke();
            }
        }

        public void Reset(int maxHearts, int level, int shapeCount)
        {
            _hearts = maxHearts;
            _currentLevel = level;
            _shapesRemaining = shapeCount;

            OnHeartsChanged?.Invoke(_hearts);
            OnLevelChanged?.Invoke(_currentLevel);
            OnShapesRemainingChanged?.Invoke(_shapesRemaining);
        }
    }
}
