using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

enum GameState
{
    Intro = 0,
    Playing = 1,
    Win = 2,
    WaitingLose = 3,
    Lose = 4,
    Pause = 5,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    Tower tower;
    [SerializeField]
    PercentCounter percentCounter;
    [SerializeField]
    TimerCounter timerCounter;
    [SerializeField]
    BallShooter ballShooter;
    [SerializeField]
    TextMeshProUGUI ballCountText;
    [SerializeField]
    ComboUI comboUI;
    [SerializeField]
    Animation oneBallRemaining;
    [SerializeField]
    AnimationCurve percentRequiredPerLevel;
    [SerializeField]
    AnimationCurve floorsPerLevel;
    [SerializeField]
    AnimationCurve ballToTileRatioPerLevel;
    [SerializeField]
    AnimationCurve colorCountPerLevel;
    [SerializeField]
    AnimationCurve specialTileChancePerLevel;

    [SerializeField]
    ParticleSystem tileDestroyFx;
    [SerializeField]
    ParticleSystem tileExplosionFx;

    Animator animator;

    float minPercent = 0;
    int tileCount;
    int destroyedTileCount;
    int ballCount;
    GameState gameState = GameState.Intro;

    //game timer params:
    float timeRemaining = 0f;
    bool timerActive = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        animator.speed = 1.0f / Time.timeScale;
        FxPool.Instance.EnsureQuantity(tileExplosionFx, 3);
        FxPool.Instance.EnsureQuantity(tileDestroyFx, 30);
    }

    private void Start()
    {
        TileColorManager.Instance.SetColorList(SaveData.CurrentColorList);
        TileColorManager.Instance.SetMaxColors(Mathf.FloorToInt(colorCountPerLevel.Evaluate(SaveData.CurrentLevel)), true);
        minPercent = percentRequiredPerLevel.Evaluate(SaveData.CurrentLevel);
        tower.FloorCount = Mathf.FloorToInt(floorsPerLevel.Evaluate(SaveData.CurrentLevel));

        //Check first if the explosive barrels should appear.
        if (RemoteConfig.BOOL_EXPLOSIVE_BARRELS_ENABLED)
        {
            //TODO: Make this prettier 
            int _lvlForExplosives = SaveData.CurrentLevel - (RemoteConfig.INT_EXPLOSIVE_BARRELS_MIN_LEVEL - 1);
            tower.SpecialTileChance = specialTileChancePerLevel.Evaluate(_lvlForExplosives);

        }
        else
        {
            tower.SpecialTileChance = 0;
        }


        tower.OnTileDestroyedCallback += OnTileDestroyed;
        tower.BuildTower();

        tileCount = tower.FloorCount * tower.TileCountPerFloor;
        ballCount = Mathf.FloorToInt(ballToTileRatioPerLevel.Evaluate(SaveData.CurrentLevel) * tileCount);
        ballCountText.text = ballCount.ToString("N0");
        ballShooter.OnBallShot += OnBallShot;

        //purely cosmetic, so the timer color is always the same color for level bar
        int percentageColor = Mathf.FloorToInt(Random.value * TileColorManager.Instance.ColorCount);

        percentCounter.SetColor(TileColorManager.Instance.GetColor(percentageColor));
        percentCounter.SetValue(SaveData.PreviousHighscore);
        percentCounter.SetShadowValue(SaveData.PreviousHighscore);
        percentCounter.SetValueSmooth(0f);

        timerCounter.gameObject.SetActive(RemoteConfig.BOOL_LEVEL_TIMER_ON);
        if (RemoteConfig.BOOL_LEVEL_TIMER_ON)
        {
            timerCounter.SetColor(TileColorManager.Instance.GetColor(Mathf.FloorToInt(percentageColor)));
            timerCounter.SetValueSmooth(1f);
        }

    }

    void OnBallShot()
    {
        ballCount--;
        ballCountText.text = ballCount.ToString("N0");
        if (ballCount == 1) {
            oneBallRemaining.Play();
        }
        else if (ballCount == 0) {
            SaveData.PreviousHighscore = Mathf.Max(SaveData.PreviousHighscore, ((float)destroyedTileCount / tileCount) / minPercent);
            SetGameState(GameState.WaitingLose);
        }
    }

    void SetGameState(GameState state)
    {
        gameState = state;
        animator.SetInteger("GameState", (int)state);

        if (state != GameState.Playing)
        {
            timerActive = false;
        }
    }

    public void OnTileDestroyed(TowerTile tile)
    {
        if (gameState == GameState.Playing || gameState == GameState.WaitingLose) {
            comboUI.CountCombo(tile.transform.position);
            destroyedTileCount++;
            float p = (float)destroyedTileCount / tileCount;
            percentCounter.SetValueSmooth(p / minPercent);
            if (p >= minPercent) {
                CameraShakeManager.Instance.StopAll(true);
                CameraShakeManager.Instance.enabled = false;
                SaveData.CurrentLevel++;
                SaveData.PreviousHighscore = 0;
                SetGameState(GameState.Win);
                if (SaveData.VibrationEnabled == 1)
                    Handheld.Vibrate();
            }
        }
    }

    private void Update()
    {
        HandleTimer();
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        tower.StartGame();
    }

    public void StartTimer()
    {
        if (RemoteConfig.BOOL_LEVEL_TIMER_ON)
        {
            timerActive = true;
            timeRemaining = RemoteConfig.FLOAT_LEVEL_TIMER_SECONDS;
        }
    }

    public void TogglePause(bool isPaused)
    {
        if (isPaused)
        {
            SetGameState(GameState.Pause);
        }
        else
        {
            SetGameState(GameState.Playing);
            if (RemoteConfig.BOOL_LEVEL_TIMER_ON)
            {
                timerActive = true;
            }
        }
        
    }

    private void HandleTimer()
    {

        if (timerActive)
        {
            timeRemaining -= Time.deltaTime;
            timerCounter.SetValue(timeRemaining / RemoteConfig.FLOAT_LEVEL_TIMER_SECONDS);

            if (timeRemaining <= 0)
            {
                timerActive = false;
                SetGameState(GameState.WaitingLose);
            }
        }


    }
}
