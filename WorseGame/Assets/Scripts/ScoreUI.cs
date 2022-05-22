using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class ScoreUI : MonoBehaviour, IFeedbacks
{
    public TextMeshProUGUI scoreValueText;
    public TextMeshProUGUI currentScoreMultiplierText;

    public MMFeedbacks scoreChangeFeedback;
    public MMFeedbacks multiplierChangeFeedback;
    public MMFeedbacks newHighScoreFeedback;
    public MMFeedbacks newMaxKillsFeedback;
    public void PlayCollsionFeedback()
    {
        scoreChangeFeedback.PlayFeedbacks();
        multiplierChangeFeedback.PlayFeedbacks();
    }

    public void PlayDamageFeedback()
    {
       
    }
    void Awake()
    {
        GameManager.OnGameStart += SetMultiplierTextColor;
    }
    // Start is called before the first frame update
    void Start()
    {
    }
    private void SetMultiplierTextColor()
    {
        currentScoreMultiplierText.color = Resources.Load<PlayerData>("PlayerData").color;
    }
    // Update is called once per frame
    void Update()
    {
        scoreValueText.text = ScoreSystem.GameScore.score.ToString("N0");
        currentScoreMultiplierText.gameObject.SetActive(ScoreSystem.GameScore.currentScoreMultiplier > 1);
        currentScoreMultiplierText.text = "X" + ScoreSystem.GameScore.currentScoreMultiplier.ToString();
    }
}
