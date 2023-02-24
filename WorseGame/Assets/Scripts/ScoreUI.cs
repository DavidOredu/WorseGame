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
    private void SetMultiplierTextColor()
    {
        currentScoreMultiplierText.color = GameManager.instance.playerData.color;
    }
    // Update is called once per frame
    void Update()
    {
        scoreValueText.text = ScoreSystem.GameScore.score.ToString("N0");
        currentScoreMultiplierText.gameObject.SetActive(ScoreSystem.GameScore.currentScoreMultiplier > 1);
        SetMultiplierTextColor();
        currentScoreMultiplierText.text = "X" + ScoreSystem.GameScore.currentScoreMultiplier.ToString();
    }
}
