using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

public class LevelWindow : MonoBehaviour
{
    private LevelData levelData;


    [SerializeField] private TextMeshProUGUI experienceToExperienceToNextLevelText = null;
    [SerializeField] private TextMeshProUGUI levelText = null;
    [SerializeField] private TextMeshProUGUI currencyText = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI highScoreText = null;
    [SerializeField] private TextMeshProUGUI killsText = null;
    [SerializeField] private TextMeshProUGUI maxKillsText = null;
    [SerializeField] private Slider levelSlider = null;

    private LevelSystem levelSystem;
    private LevelSystemAnimated levelSystemAnimated;

    public MMFeedbacks sliderFeedback;
    public MMFeedbacks moneyIncreaseFeedback;
    public MMFeedbacks levelIncreaseFeedback;

    public Button retryButton;
    public Button shopButton;

    private void Awake()
    {
        levelData = Resources.Load<LevelData>("LevelData");
    }
    private void Start()
    {
        SetStatsTexts();
    }
    private void SetExperienceBarSize(float levelNormalized)
    {
        levelSlider.value = levelNormalized;
    }
    private void SetLevelNumber(int levelNumber)
    {
        levelText.text = "Level " + (levelNumber).ToString();
    }
    private void SetCurrencyText()
    {
        currencyText.text = "$" + ShopManager.instance.currencyData.currencyValue.ToString("N0");
    }
    public void LevelUp()
    {
        levelSystem.LevelUp();
        levelSystemAnimated.LevelUp();
    }
    public void SetLevelSystem(LevelSystem levelSystem)
    {
        //set the levelsystemAnimated object
        this.levelSystem = levelSystem;
    }
    public void SetLevelSystemAnimated(LevelSystemAnimated levelSystemAnimated)
    {
        //set the levelsystemAnimated object
        this.levelSystemAnimated = levelSystemAnimated;

        //update the starting values
        SetLevelNumber(levelSystemAnimated.GetLevelNumber());
        
        SetExperienceBarSize(levelSystemAnimated.GetAttributeNormalized(levelSystemAnimated.levelAttributeExperience));
       
        //subscribe to the changed events
        levelSystemAnimated.OnExperienceChanged += LevelSystemAnimated_OnExperienceChanged;
        levelSystemAnimated.OnLevelChanged += LevelSystemAnimated_OnLevelChanged;
    }
    public void SetStatsTexts()
    {
        SetCurrencyText();
        scoreText.text = "SCORE: " + ScoreSystem.GameScore.score.ToString("N0");
        highScoreText.text = GameManager.instance.scoreData.highScore.ToString("N0");
        killsText.text = "KILLS: " + ScoreSystem.GameScore.kills.ToString("N0");
        maxKillsText.text = "MAX KILLS: " + GameManager.instance.scoreData.maxKills.ToString("N0");
        experienceToExperienceToNextLevelText.text = levelSystemAnimated.levelAttributeExperience.ToString("N0") + "/" + levelSystemAnimated.experienceToNextLevel.ToString("N0");
    }
    public void SetLevelStats()
    {
        SetLevelNumber(levelSystemAnimated.GetLevelNumber());
        SetExperienceBarSize(levelSystemAnimated.GetAttributeNormalized(levelSystemAnimated.levelAttributeExperience));
    }
    private void LevelSystemAnimated_OnLevelChanged(object sender, System.EventArgs e)
    {
        //level changed update text
        SetLevelStats();
        SetStatsTexts();
        sliderFeedback.PlayFeedbacks();
        moneyIncreaseFeedback.PlayFeedbacks();
        levelIncreaseFeedback.PlayFeedbacks();
    }

    private void LevelSystemAnimated_OnExperienceChanged(object sender, System.EventArgs e)
    {
        //experience changed update text
        SetLevelStats();
        SetStatsTexts();
    }
}
