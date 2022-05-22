using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public GameObject levelGeneratorPrefab;
    public GameObject objectPoolerPrefab;

    private GameObject player;
    private GameObject levelGenerator;
    private GameObject objectPooler;

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
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        SetGraphics();
        OnGameStart += SetScoreMultiplier;
        OnGameStart += ResetScore;
        OnGameStart += InstantiateInGameObjects;
        OnGameStart += RemoveGameOverUI;
        OnGameStart += CameraFollowPlayer;

        OnGameEnd += SetScoreToLevel;
        OnGameEnd += DisplayGameOverUI;
        OnGameEnd += SetUIStats;
        OnGameEnd += DestroyInGameObjects;

        
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
        player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        levelGenerator = Instantiate(levelGeneratorPrefab, playerSpawnPoint.position, Quaternion.identity);
        objectPooler = Instantiate(objectPoolerPrefab, playerSpawnPoint.position, Quaternion.identity);
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
        Destroy(player);
        Destroy(levelGenerator);
        Destroy(objectPooler);
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
            if (!scoreComboTimer.isTimeUp)
            {
                scoreComboTimer.UpdateTimer();
            }
            else
            {
                ScoreSystem.GameScore.currentScoreMultiplier = 1;
            }
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
        ScoreSystem.GameScore.score += score * ScoreSystem.GameScore.currentScoreMultiplier;
        ScoreSystem.GameScore.kills++;
        if (ScoreSystem.GameScore.currentScoreMultiplier < ScoreSystem.GameScore.maxMultiplier)
        {
            ScoreSystem.GameScore.currentScoreMultiplier++;
        }
    }
    public void ResetScore()
    {
        ScoreSystem.GameScore.ResetScore();
    }
    public void SetScoreToLevel()
    {
        LevelingManager.instance.levelSystem.AddExperience(ScoreSystem.GameScore.score);
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
    public int GetPercentageOfExperienceToNextLevel(int percentage)
    {
        return (percentage * LevelingManager.instance.levelSystem.GetExperienceToNextLevel(LevelingManager.instance.levelSystem.GetLevelNumber()) / 100);
    }
}
