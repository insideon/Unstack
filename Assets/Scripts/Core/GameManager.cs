using UnityEngine;
using Unstack.Shape;
using Unstack.Audio;
using Unstack.Animation;

namespace Unstack.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameConfig config;

        public GameConfig Config => config;
        public GameState State { get; private set; }
        public ScoreManager ScoreManager { get; private set; }

        private LevelManager _levelManager;
        private bool _inputLocked;
        private LevelTransitionAnimator _levelTransitionAnimator;
        private bool _waitingForMenu = true;
        private bool _tutorialActive;
        private bool _firstShapeRemoved;

        public event System.Action OnFirstShapeRemoved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            State = new GameState();
            ScoreManager = new ScoreManager();
            _levelManager = GetComponent<LevelManager>();
        }

        private void Start()
        {
            if (_levelManager == null)
                _levelManager = GetComponent<LevelManager>();

            State.OnGameOver += HandleGameOver;
            State.OnLevelCleared += HandleLevelCleared;

            // Don't auto-start; wait for menu Play button
        }

        private void OnDestroy()
        {
            if (State != null)
            {
                State.OnGameOver -= HandleGameOver;
                State.OnLevelCleared -= HandleLevelCleared;
            }
        }

        public void SetLevelTransitionAnimator(LevelTransitionAnimator animator)
        {
            _levelTransitionAnimator = animator;
        }

        public void StartGame()
        {
            _inputLocked = false;
            _waitingForMenu = false;
            _firstShapeRemoved = false;
            _tutorialActive = !SaveManager.HasPlayedBefore();
            ScoreManager.Reset();
            State.Reset(config.maxHearts, 1, config.GetShapeCount(1));
            _levelManager.GenerateLevel(1);
            AudioManager.Instance?.StartMusic();
        }

        public void NextLevel()
        {
            int nextLevel = State.CurrentLevel + 1;
            SaveManager.SetHighestLevel(nextLevel);

            bool isPerfect = State.Hearts == config.maxHearts;
            int currentHearts = State.Hearts;

            if (_levelTransitionAnimator != null)
            {
                _inputLocked = true;
                _levelTransitionAnimator.PlayTransition(() =>
                {
                    ScoreManager.OnLevelCleared(isPerfect);
                    _levelManager.GenerateLevel(nextLevel);
                    State.Reset(currentHearts, nextLevel, config.GetShapeCount(nextLevel));
                    _inputLocked = false;
                });
            }
            else
            {
                ScoreManager.OnLevelCleared(isPerfect);
                _levelManager.GenerateLevel(nextLevel);
                State.Reset(currentHearts, nextLevel, config.GetShapeCount(nextLevel));
                _inputLocked = false;
            }
        }

        public void OnShapeTapped(ShapeController shape)
        {
            if (_inputLocked || _waitingForMenu) return;

            if (shape.IsCovered())
            {
                State.Hearts--;
                shape.Animator.PlayShake();
                ScoreManager.OnWrongTap();
                AudioManager.Instance?.PlayWrongTap();
                ScreenEffects.Instance?.FlashRed();
            }
            else
            {
                shape.SetActive(false);
                var shapeColor = shape.Data.Color;
                var shapePosition = shape.transform.position;
                shape.Animator.PlayFadeOut(() =>
                {
                    Destroy(shape.gameObject);
                });
                State.ShapesRemaining--;
                ScoreManager.OnCorrectTap();
                AudioManager.Instance?.PlayCorrectTap();
                ParticleEffectFactory.CreateBurstEffect(shapePosition, shapeColor);

                if (!_firstShapeRemoved)
                {
                    _firstShapeRemoved = true;
                    OnFirstShapeRemoved?.Invoke();
                    if (_tutorialActive)
                    {
                        _tutorialActive = false;
                        SaveManager.SetHasPlayedBefore();
                    }
                }
            }
        }

        private void HandleGameOver()
        {
            _inputLocked = true;
            AudioManager.Instance?.PlayGameOver();
            AudioManager.Instance?.StopMusic();

            int finalScore = ScoreManager.CurrentScore;
            if (finalScore > SaveManager.GetHighScore())
                SaveManager.SetHighScore(finalScore);
            SaveManager.SetHighestLevel(State.CurrentLevel);
        }

        private void HandleLevelCleared()
        {
            _inputLocked = true;
            AudioManager.Instance?.PlayLevelClear();
        }

        public bool IsWaitingForMenu => _waitingForMenu;
    }
}
