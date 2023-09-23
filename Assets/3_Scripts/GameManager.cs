using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public enum GameState
{
    Intro = 0,
    Playing = 1,
    Win = 2,
    WaitingLose = 3,
    Lose = 4,
    Pause = 5,
    PowerUp = 6,
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
    PowerUpUI powerUpUI;
    [SerializeField]
    GameObject pauseButton;
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
    MissionManager missionManager;
    [SerializeField]
    PowerUpManager powerUpManager;

    [SerializeField]
    ParticleSystem tileDestroyFx;
    [SerializeField]
    ParticleSystem tileExplosionFx;

    Animator animator;

    float minPercent = 0;
    int tileCount;
    int destroyedTileCount;
    int ballCount;
    int startingBallCount;
    GameState gameState = GameState.Intro;

    //game timer params:
    float timeRemaining = 0f;
    float totalTime = 0f;
    bool timerActive = false;

    PowerUpConfig currentPowerUp;
    float defaultTimeScale;
    int multiballsLeft;

    int ballMiss = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        animator.speed = 1.0f / Time.timeScale;
        FxPool.Instance.EnsureQuantity(tileExplosionFx, 3);
        FxPool.Instance.EnsureQuantity(tileDestroyFx, 30);
        defaultTimeScale = Time.timeScale;
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
        startingBallCount = ballCount;
        ballCountText.text = ballCount.ToString("N0");
        ballShooter.OnBallShot += OnBallShot;
        ballShooter.OnTargetSaved += OnTargetSaved;

        //purely cosmetic, so the timer color is always the same color for level bar
        int percentageColor = Mathf.FloorToInt(Random.value * TileColorManager.Instance.ColorCount);
        percentCounter.SetColor(TileColorManager.Instance.GetColor(percentageColor));
        percentCounter.SetLevel(SaveData.CurrentLevel);
        percentCounter.SetValue(SaveData.PreviousHighscore);
        percentCounter.SetShadowValue(SaveData.PreviousHighscore);
        percentCounter.SetValueSmooth(0f);

        timerCounter.gameObject.SetActive(RemoteConfig.BOOL_LEVEL_TIMER_ON);
        if (RemoteConfig.BOOL_LEVEL_TIMER_ON)
        {
            timerCounter.SetColor(TileColorManager.Instance.GetColor(Mathf.FloorToInt(percentageColor)));
            timerCounter.SetValueSmooth(1f);
            totalTime = RemoteConfig.FLOAT_LEVEL_TIMER_SECONDS;
        }

        pauseButton.SetActive(RemoteConfig.BOOL_PAUSE_BUTTON_ENABLED);

    }

    void OnTargetSaved()
    {
        multiballsLeft--;
        powerUpUI.UpdatePowerUpCounter(multiballsLeft);
        if (multiballsLeft == 0)
        {
            powerUpUI.PowerUpInactive();
            ballShooter.FireMultiballs();
        }
    }

    void OnBallShot()
    {
        if (gameState == GameState.PowerUp)
            return;
        ballCount--;
        ballCountText.text = ballCount.ToString("N0");
        if (ballCount == 1)
        {
            oneBallRemaining.Play();
        }
        else if (ballCount == 0)
        {
            SaveData.PreviousHighscore = Mathf.Max(SaveData.PreviousHighscore, ((float)destroyedTileCount / tileCount) / minPercent);
            SetGameState(GameState.WaitingLose);
            missionManager.LevelFinished();
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

    public GameState GetGameState()
    {
        return gameState;
    }

    public void OnTileDestroyed(TowerTile tile)
    {
        if (gameState == GameState.Playing || gameState == GameState.WaitingLose || gameState == GameState.PowerUp)
        {
            comboUI.CountCombo(tile.transform.position);
            destroyedTileCount++;
            missionManager.RecordHigscore(destroyedTileCount);
            float p = (float)destroyedTileCount / tileCount;
            percentCounter.SetValueSmooth(p / minPercent);
            if (p >= minPercent)
            {
                CameraShakeManager.Instance.StopAll(true);
                CameraShakeManager.Instance.enabled = false;
                SaveData.CurrentLevel++;
                SaveData.PreviousHighscore = 0;
                SetGameState(GameState.Win);
                missionManager.RecordHalfBalls(ballCount > startingBallCount/2f);
                missionManager.RecordHalfTime(timeRemaining > RemoteConfig.FLOAT_LEVEL_TIMER_SECONDS/2f);
                missionManager.LevelFinished();

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
            timeRemaining = totalTime;
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
            timerCounter.SetValue(timeRemaining / totalTime);

            if (timeRemaining <= 0)
            {
                timerActive = false;
                SetGameState(GameState.WaitingLose);
                missionManager.RecordHigscore(destroyedTileCount);
                missionManager.LevelFinished();
            }
        }


    }

    public void HandlePowerUp(PowerUpConfig powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.Multyball:
                HandleMultiball();
                break;
            case PowerUpType.TimerBoost:
                totalTime += powerUp.value;
                timeRemaining += powerUp.value;
                break;
            case PowerUpType.ExtraBalls:
                ballCount += powerUp.value;
                ballCountText.text = ballCount.ToString("N0");
                break;
        }

    }

    private void HandleMultiball()
    {
        multiballsLeft = RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT;
        powerUpUI.PowerUpActive();
        Time.timeScale = defaultTimeScale * 0.25f;
        SetGameState(GameState.PowerUp);

    }

    public void HandleReward(RewardConfig reward)
    { 
        switch(reward.type)
        {
            case RewardType.TimerBoost:
                powerUpManager.AddPowerUp(PowerUpType.TimerBoost, reward.amount);
                break;
            case RewardType.ExtraPowerup:
                powerUpManager.AddPowerUp(PowerUpType.Multyball, reward.amount);
                break;
            case RewardType.ExtraBalls:
                powerUpManager.AddPowerUp(PowerUpType.ExtraBalls, reward.amount);
                break;
        }
    }

    public void HandlePowerUpModeOver()
    {
        Time.timeScale = defaultTimeScale;
        SetGameState(GameState.Playing);

        if (RemoteConfig.BOOL_LEVEL_TIMER_ON)
        {
            timerActive = true;

        }

    }
}
