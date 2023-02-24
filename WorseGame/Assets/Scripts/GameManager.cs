using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public int targetFPS = 60;

    private bool inGame = false;

    public Transform playerSpawnPoint;
    public GameObject playerPrefab;
    public GameObject levelGeneratorPrefab;
    public GameObject objectPoolerPrefab;
    public GameObject powerupManagerPrefab;

    private GameObject playerObj;
    private GameObject levelGenerator;
    private GameObject objectPooler;
    private GameObject powerupManager;

    public LevelWindow levelWindow;
    public Cinemachine.CinemachineVirtualCamera cmVcam;


    public List<Image> healthIcons = new List<Image>();

    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    private CurrencyData currencyData;

    public Timer scoreComboTimer;
    public float scoreComboTime;

    public ScoreUI scoreUI;
    public ScoreData scoreData;
    public SettingsData settingsData;

    public MMFeedbacks currencyAwardFeedback;

    public PowerupController powerupController;
    public PowerupActions powerupActions;

    public Player player;
    public PlayerData playerData { get; private set; }

    public float xpToNextLevelNormalized;
    public List<bool> boolList { get; private set; } = new List<bool> { true, false };
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        playerData = Resources.Load<PlayerData>("PlayerData");

     //   Application.targetFrameRate = targetFPS;
        SetGraphics();
        OnGameStart += SetInGame;
        OnGameStart += SetScoreMultiplier;
        OnGameStart += ResetScore;
        OnGameStart += InstantiateInGameObjects;
        OnGameStart += RemoveGameOverUI;
        OnGameStart += CameraFollowPlayer;
        OnGameStart += ActivatePauseButton;

        OnGameEnd += DeactivatePauseButton;
        OnGameEnd += SetScoreToLevel;
        OnGameEnd += DisplayGameOverUI;
        OnGameEnd += SetUIStats;
        OnGameEnd += DestroyInGameObjects;
        OnGameEnd += SetInGame;
    }
    public void SetGraphics()
    {
        UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset = settingsData.pipelineSetting;
        QualitySettings.renderPipeline = settingsData.pipelineSetting;
    }
    void Start()
    {
        scoreComboTimer = new Timer(scoreComboTime);
        scoreComboTimer.SetTimer();

        currencyData = Resources.Load<CurrencyData>("CurrencyData");
        LevelingManager.instance.levelSystemAnimated.OnLevelChanged += LevelSystemAnimated_OnLevelChanged;
    }
    void SetInGame()
    {
        inGame = !inGame;
    }
    void SetScoreMultiplier()
    {
        ScoreSystem.GameScore.maxMultiplier = scoreData.maxScoreMultiplier;
    }
    public void CameraFollowPlayer()
    {
        cmVcam.m_Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void InstantiateInGameObjects()
    {
        playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        levelGenerator = Instantiate(levelGeneratorPrefab, playerSpawnPoint.position, Quaternion.identity);
        objectPooler = Instantiate(objectPoolerPrefab, playerSpawnPoint.position, Quaternion.identity);
        powerupManager = Instantiate(powerupManagerPrefab, playerSpawnPoint.position, Quaternion.identity);

        player = playerObj.GetComponentInChildren<Player>();
    }
    public void DestroyInGameObjects()
    {
        foreach (var obj in ObjectPooler.instance.spawnedObjects)
        {
            // play destruction feedback
            if (obj.activeSelf)
                obj.GetComponent<IDestructible>().InstantiateDestructParticle();
            Destroy(obj);
        }
        Destroy(playerObj);
        Destroy(levelGenerator);
        Destroy(objectPooler);
        Destroy(powerupManager);
    }
    public void ActivatePauseButton()
    {
        UIManager.instance.SetPauseButton(true);
    }
    public void DeactivatePauseButton()
    {
        UIManager.instance.SetPauseButton(false);
    }
    private void LevelSystemAnimated_OnLevelChanged(object sender, EventArgs e)
    {
        AddCurrency(LevelingManager.instance.levelSystem.GetExperienceToNextLevel(LevelingManager.instance.levelSystemAnimated.level - 1), true);
    }

    // Update is called once per frame
    void Update()
    {
        if (ScoreSystem.GameScore.currentScoreMultiplier > 1)
        {
            if (!scoreComboTimer.IsTimeUp)
            {
                scoreComboTimer.UpdateTimer();
            }
            else
            {
                ScoreSystem.GameScore.currentScoreMultiplier = 1;
            }
        }
        if((powerupController == null || powerupActions == null) && inGame)
        {
            powerupManager = GameObject.FindGameObjectWithTag("PowerupManager");
            powerupController = powerupManager.GetComponent<PowerupController>();
            powerupActions = powerupManager.GetComponent<PowerupActions>();
        }
        if (inGame)
        {
            SetXPToNextLevelNormalized();
        }
    }
    public void AddCurrency(int amount, bool playSound)
    {
        currencyData.currencyValue += amount;
        if (settingsData.sound && playSound)
            currencyAwardFeedback.PlayFeedbacks();
    }
    public void AddScore(int score)
    {
        scoreComboTimer.ResetTimer();

        if (!player.superMultiplier)
            ScoreSystem.GameScore.score += score * ScoreSystem.GameScore.currentScoreMultiplier;
        else
            ScoreSystem.GameScore.score += score * ScoreSystem.GameScore.currentScoreMultiplier * scoreData.superMultiplierMultiplier;

        ScoreSystem.GameScore.kills++;
        if (ScoreSystem.GameScore.currentScoreMultiplier < ScoreSystem.GameScore.maxMultiplier)
        {
            ScoreSystem.GameScore.currentScoreMultiplier++;
        }
    }
    public void SetXPToNextLevelNormalized()
    {
        xpToNextLevelNormalized = Mathf.Abs(ScoreSystem.GameScore.score / LevelingManager.instance.levelSystem.GetExperienceToNextLevelFloat(LevelingManager.instance.levelSystem.GetLevelNumber()));
    }
    public void ResetScore()
    {
        ScoreSystem.GameScore.ResetScore();
    }
    public void SetScoreToLevel()
    {
        AddXP(ScoreSystem.GameScore.score);
    }
    public void AddXP(int xp)
    {
        LevelingManager.instance.levelSystem.AddExperience(xp);
    }
    public void AwardAdsReward()
    {
        AddCurrency(Mathf.CeilToInt(.25f * LevelingManager.instance.levelSystem.GetExperienceToNextLevel(LevelingManager.instance.levelSystem.GetLevelNumber())), true);
        AddXP(LevelingManager.instance.levelSystem.GetExperienceToNextLevel(LevelingManager.instance.levelSystem.GetLevelNumber()));
        ShopManager.instance.SetCurrencyToRewardAfterAdText();
        // TODO: Reward currency
    }
    public void SetUIStats()
    {
        if (ScoreSystem.GameScore.score > scoreData.highScore)
        {
            scoreData.highScore = ScoreSystem.GameScore.score;
            if (scoreData.highScore > 0)
                scoreUI.newHighScoreFeedback.gameObject.SetActive(true);
        }
        else
            scoreUI.newHighScoreFeedback.gameObject.SetActive(false);

        if (ScoreSystem.GameScore.kills > scoreData.maxKills)
        {
            scoreData.maxKills = ScoreSystem.GameScore.kills;
            if (scoreData.maxKills > 0)
                scoreUI.newMaxKillsFeedback.gameObject.SetActive(true);
        }
        else
            scoreUI.newMaxKillsFeedback.gameObject.SetActive(false);
    }
    public void DisplayGameOverUI()
    {
        UIManager.instance.UpdatePopUp(1);
        levelWindow.SetLevelStats();
        levelWindow.SetStatsTexts();
    }
    public void RemoveGameOverUI()
    {
        UIManager.instance.ClosePopUp();
    }
    public void SkipLevelingAnimation()
    {
        LevelingManager.instance.levelSystemAnimated.SetLevelSystem();
    }
    public void GameStart()
    {
        OnGameStart?.Invoke();
    }
    public void GameEnd()
    {
        OnGameEnd?.Invoke();
    }
    public void KillPlayer()
    {
        player.Damage(true);
    }
    public float GetPercentageOfExperienceToNextLevel(float percentage)
    {
        return (percentage) * LevelingManager.instance.levelSystem.GetExperienceToNextLevelFloat(LevelingManager.instance.levelSystem.GetLevelNumber());
    }
}
