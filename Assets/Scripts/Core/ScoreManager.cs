using System;

namespace Unstack.Core
{
    public class ScoreManager
    {
        private const int BasePoints = 100;
        private const float ComboIncrement = 0.5f;
        private const int PerfectLevelBonus = 500;

        public event Action<int> OnScoreChanged;
        public event Action<int> OnComboChanged;
        public event Action<int> OnHighScoreChanged;

        private int _currentScore;
        private int _combo;
        private int _cachedHighScore;

        public int CurrentScore => _currentScore;
        public int Combo => _combo;

        public void Reset()
        {
            _currentScore = 0;
            _combo = 0;
            _cachedHighScore = SaveManager.GetHighScore();
            OnScoreChanged?.Invoke(_currentScore);
            OnComboChanged?.Invoke(_combo);
        }

        public void OnCorrectTap()
        {
            _combo++;
            float multiplier = 1f + (_combo - 1) * ComboIncrement;
            int points = (int)(BasePoints * multiplier);
            _currentScore += points;
            OnComboChanged?.Invoke(_combo);
            OnScoreChanged?.Invoke(_currentScore);

            if (_currentScore > _cachedHighScore)
            {
                _cachedHighScore = _currentScore;
                OnHighScoreChanged?.Invoke(_currentScore);
            }
        }

        public void OnWrongTap()
        {
            _combo = 0;
            OnComboChanged?.Invoke(_combo);
        }

        public void OnLevelCleared(bool isPerfect)
        {
            if (isPerfect)
            {
                _currentScore += PerfectLevelBonus;
                OnScoreChanged?.Invoke(_currentScore);
            }
        }
    }
}
