using UnityEngine;
using Unstack.Shape;

namespace Unstack.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameConfig config;

        public GameConfig Config => config;
        public GameState State { get; private set; }

        private LevelManager _levelManager;
        private bool _inputLocked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            State = new GameState();
            _levelManager = GetComponent<LevelManager>();
        }

        private void Start()
        {
            if (_levelManager == null)
                _levelManager = GetComponent<LevelManager>();

            State.OnGameOver += HandleGameOver;
            State.OnLevelCleared += HandleLevelCleared;
            StartGame();
        }

        private void OnDestroy()
        {
            if (State != null)
            {
                State.OnGameOver -= HandleGameOver;
                State.OnLevelCleared -= HandleLevelCleared;
            }
        }

        public void StartGame()
        {
            _inputLocked = false;
            State.Reset(config.maxHearts, 1, config.GetShapeCount(1));
            _levelManager.GenerateLevel(1);
        }

        public void NextLevel()
        {
            _inputLocked = false;
            int nextLevel = State.CurrentLevel + 1;
            State.Reset(config.maxHearts, nextLevel, config.GetShapeCount(nextLevel));
            _levelManager.GenerateLevel(nextLevel);
        }

        public void OnShapeTapped(ShapeController shape)
        {
            if (_inputLocked) return;

            if (shape.IsCovered())
            {
                State.Hearts--;
                shape.Animator.PlayShake();
            }
            else
            {
                shape.SetActive(false);
                shape.Animator.PlayFadeOut(() =>
                {
                    Destroy(shape.gameObject);
                });
                State.ShapesRemaining--;
            }
        }

        private void HandleGameOver()
        {
            _inputLocked = true;
        }

        private void HandleLevelCleared()
        {
            _inputLocked = true;
        }
    }
}
