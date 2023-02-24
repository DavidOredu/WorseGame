using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    string appID = "ca-app-pub-7740510277849665~9961021083";

    string rewardedAdID = "ca-app-pub-3940256099942544/5224354917";

    private RewardedAd rewardedAd;
    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }
    public void CreateAndLoadRewardedAd()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = rewardedAdID;
#elif UNITY_EDITOR
        adUnitId = rewardedAdID;
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }
    public void RequestRewardedAd()
    {
        string adUnitId;
#if UNITY_EDITOR
        adUnitId = rewardedAdID;
#elif UNITY_ANDROID
        adUnitId = rewardedAdID;
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif
        rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);

    }
    private void ShowRewardedAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        ShowRewardedAd();
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
    }
}
