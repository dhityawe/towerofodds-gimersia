// GameStateManager.cs
// Manages game state transitions using the State Pattern

using System;
using UnityEngine;

/// <summary>
/// Manages the game flow state machine.
/// Flow: Intro → CheckWave (Shop at 0) → RollDice → StartWave → WaveActive → CheckWave → (Shop every 5) → Loop
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameStateManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameStateManager");
                    _instance = go.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }

    [Header("References")]
    [SerializeField] private TowerRuntime playerTower;
    [SerializeField] private ShopManager shopManager;

    [Header("Current State (Read-Only)")]
    [SerializeField] private string currentStateName;

    [Header("Wave Settings")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int maxWaveKelipatan = 5; // Every 5 waves, check if open shop

    // State instances
    private GameState currentState;
    private IntroState introState;
    private RollDiceState rollDiceState;
    private StartWaveState startWaveState;
    private WaveActiveState waveActiveState;
    private CheckWaveCompleteState checkWaveCompleteState;
    private OpenShopState openShopState;
    private GameOverState gameOverState;

    // Events
    public event Action<GameState> OnStateChanged;
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnGameOver;

    // Public accessors
    public TowerRuntime PlayerTower => playerTower;
    public ShopManager ShopManager => shopManager;
    public int CurrentWave => currentWave;
    public int MaxWaveKelipatan => maxWaveKelipatan;
    public GameState CurrentState => currentState;

    void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // Find references if not assigned
        if (playerTower == null)
        {
            playerTower = FindFirstObjectByType<TowerRuntime>();
        }
        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<ShopManager>();
        }

        // Initialize states
        introState = new IntroState(this);
        rollDiceState = new RollDiceState(this);
        startWaveState = new StartWaveState(this);
        waveActiveState = new WaveActiveState(this);
        checkWaveCompleteState = new CheckWaveCompleteState(this);
        openShopState = new OpenShopState(this);
        gameOverState = new GameOverState(this);

        // Subscribe to tower death
        if (playerTower != null)
        {
            playerTower.OnDeath += OnTowerDeath;
        }
    }

    void Start()
    {
        // Start game flow with intro
        ChangeState(introState);
    }

    void Update()
    {
        currentState?.OnUpdate();
    }

    void OnDestroy()
    {
        if (playerTower != null)
        {
            playerTower.OnDeath -= OnTowerDeath;
        }
    }

    #region State Management
    public void ChangeState(GameState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = newState;
        currentStateName = currentState?.GetStateName() ?? "None";

        if (currentState != null)
        {
            currentState.OnEnter();
            OnStateChanged?.Invoke(currentState);
        }

        Debug.Log($"[GameState] → {currentStateName}");
    }
    #endregion

    #region State Trasition

    public void TransitionToIntro()
    {
        ChangeState(introState);
    }

    public void TransitionToCheckWave()
    {
        ChangeState(checkWaveCompleteState);
    }

    public void TransitionToRollDice()
    {
        ChangeState(rollDiceState);
    }

    public void TransitionToStartWave()
    {
        ChangeState(startWaveState);
    }

    public void TransitionToWaveActive()
    {
        ChangeState(waveActiveState);
    }

    public void TransitionToCheckWaveComplete()
    {
        ChangeState(checkWaveCompleteState);
    }

    public void TransitionToOpenShop()
    {
        ChangeState(openShopState);
    }

    public void TransitionToOpenShop(ShopType shopType)
    {
        openShopState.OpenShop(shopType);
        ChangeState(openShopState);
    }

    public void TransitionToGameOver()
    {
        ChangeState(gameOverState);
    }
    #endregion

    #region Wave Management

    public void IncrementWave()
    {
        currentWave++;
        OnWaveStarted?.Invoke(currentWave);
    }

    public void NotifyWaveComplete()
    {
        OnWaveCompleted?.Invoke(currentWave);
    }

    public void NotifyGameOver()
    {
        OnGameOver?.Invoke();
    }

    public bool ShouldOpenShop()
    {
        // Open shop at wave 0 (after intro) and every 5 waves (5, 10, 15, etc.)
        return currentWave % maxWaveKelipatan == 0;
    }

    public void ResetGame()
    {
        currentWave = 0;
        PlayerDataManager.Instance.StartNewGame();

        if (playerTower != null)
        {
            playerTower.ResetToDefault();
        }

        TransitionToIntro();
    }
    #endregion

    #region Event Handler
    private void OnTowerDeath()
    {
        TransitionToGameOver();
    }
    #endregion

    #region Debug
    #if UNITY_EDITOR
    [ContextMenu("Debug: Current State")]
    private void DebugCurrentState()
    {
        Debug.Log($"Current State: {currentStateName} | Wave: {currentWave}");
    }

    [ContextMenu("Debug: Force Next State")]
    private void DebugForceNextState()
    {
        if (currentState == rollDiceState) TransitionToStartWave();
        else if (currentState == startWaveState) TransitionToWaveActive();
        else if (currentState == waveActiveState) TransitionToCheckWaveComplete();
        else if (currentState == checkWaveCompleteState) TransitionToOpenShop();
        else if (currentState == openShopState) TransitionToRollDice();
    }

    [ContextMenu("Debug: Reset Game")]
    private void DebugResetGame()
    {
        ResetGame();
    }
    #endif
    #endregion
}
