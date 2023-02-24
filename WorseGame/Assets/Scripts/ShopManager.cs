using MoreMountains.Feedbacks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : SingletonDontDestroy<ShopManager>
{
    public List<TextMeshProUGUI> moneyText = new List<TextMeshProUGUI>();

    public List<ShopItem> shopItems;
    public List<Button> shopButtons;
    public List<TextMeshProUGUI> priceTexts;

    public List<Customization> customizationData;
    public List<Button> customizationButtons;
    public List<TextMeshProUGUI> customizationTexts;

    public List<List<Image>> shopItemIndicators = new List<List<Image>>();
    public List<List<Image>> customizationItemIndicators = new List<List<Image>>();

    PlayerData playerData;
    ScoreData scoreData;

    public List<BoxData> boxData = new List<BoxData>();
    public List<PoolInfo> boxInfo = new List<PoolInfo>();
    public List<PowerupData> powerupData = new List<PowerupData>();

    [Space]
    public CurrencyData currencyData;
    public PoolList spawnPoolList;
    public SpawnableBoxes spawnableBoxes;

    [Space]
    public List<MMFeedbacks> adsTextGlowFeedbacks = new List<MMFeedbacks>();
    public List<TextMeshProUGUI> currencyToRewardAfterAd = new List<TextMeshProUGUI>();

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < adsTextGlowFeedbacks.Count; i++)
        {
            adsTextGlowFeedbacks[i].Initialization();
        }

        playerData = Resources.Load<PlayerData>("PlayerData");
        scoreData = Resources.Load<ScoreData>("ScoreData");

        GameManager.OnGameEnd += UpdateShopUI;
    }
    void Start()
    {
        SetCurrencyToRewardAfterAdText();
        SetShopItemPrices();
        CheckAllItemAndCusLevels();
        SetPriceTexts();
        
        UpdateShopUI();

        GetIndicators();
        SetIndicatorActivity();

        SetCusDataActivity();

        SetPlayerDataCustomization();
        SetSpawnPoolList();

        GameSaveManager.OnResetGame += SetShopItemPrices;
        GameSaveManager.OnResetGame += CheckAllItemAndCusLevels;
        GameSaveManager.OnResetGame += SetPriceTexts;
        GameSaveManager.OnResetGame += UpdateShopUI;
        GameSaveManager.OnResetGame += SetIndicatorActivity;
        GameSaveManager.OnResetGame += SetCusDataActivity;
        GameSaveManager.OnResetGame += SetPlayerDataCustomization;
        GameSaveManager.OnResetGame += SetSpawnPoolList;
        GameSaveManager.OnResetGame += SetCurrencyToRewardAfterAdText;
    }
    public void SetCurrencyToRewardAfterAdText()
    {
        for (int i = 0; i < currencyToRewardAfterAd.Count; i++)
        {
            currencyToRewardAfterAd[i].text = "+$" + (.25f * LevelingManager.instance.levelSystem.GetExperienceToNextLevel(LevelingManager.instance.levelSystem.GetLevelNumber())).ToString("N0");
        }
    }
    private void CheckAllItemAndCusLevels()
    {
        for (int i = 0; i < shopItems.Count; i++)
            CheckItemLevel(shopItems[i]);
        for (int i = 0; i < customizationData.Count; i++)
            CheckItemLevel(customizationData[i]);
    }
    private void SetCusDataActivity()
    {
        for (int i = 0; i < customizationData.Count; i++)
        {
            if (customizationData[i].isPicked)
                SetCustomizationInfo(customizationData[i]);
        }
    }
    // Update is called once per frame
    void Update()
    {
        SetCurrencyText();
    }
    void SetSpawnPoolList()
    {
        

        for (int i = 0; i < boxData.Count; i++)
        {
            if (boxData[i].isUnlocked)
            {
                if(!CheckIfContainsTag(spawnPoolList.poolsToSpawn, boxInfo[i].poolInfo.tag))
                    spawnPoolList.poolsToSpawn.Add(boxInfo[i].poolInfo);

                if(!spawnableBoxes.poolTags.Contains(boxInfo[i].poolInfo.tag))
                    spawnableBoxes.poolTags.Add(boxInfo[i].poolInfo.tag);
            }
        }
    }
    bool CheckIfContainsTag(List<ObjectPooler.Pool> item, ObjectPooler.PoolTag tag)
    {
        for (int i = 0; i < item.Count; i++)
        {
            if (item[i].tag == tag)
                return true;
        }
        return false;
    }
    void SetPlayerDataCustomization()
    {
        for (int i = 0; i < customizationData.Count; i++)
        {
            if(customizationData[i].customizationType == Customization.CustomizationType.Body && customizationData[i].isPicked)
                playerData.color = customizationData[i].color;
        }
        for (int i = 0; i < customizationData.Count; i++)
        {
            if (customizationData[i].customizationType == Customization.CustomizationType.Rope && customizationData[i].isPicked)
                playerData.ropeColor = customizationData[i].color;
        }
    }
    void GetIndicators()
    {
        /*------------------- SHOP ITEMS -----------------*/
        for (int i = 0; i < shopButtons.Count; i++)
        {
            List<Image> indicatorsOfItem = new List<Image>();
            var children = shopButtons[i].transform.GetComponentsInChildren<Image>();
            foreach (var child in children)
            {
                if (child.name.Contains("Indicator"))
                {
                    indicatorsOfItem.Add(child.GetComponent<Image>());
                }
            }
            shopItemIndicators.Add(indicatorsOfItem);
        }
        /*------------------- CUSTOMIZATION ITEMS -----------------*/
        for (int i = 0; i < customizationButtons.Count; i++)
        {
            List<Image> indicatorsOfItem = new List<Image>();
            var children = customizationButtons[i].transform.GetComponentsInChildren<Image>();
            foreach (var child in children)
            {
                if (child.name.Contains("Indicator"))
                {
                    indicatorsOfItem.Add(child.GetComponent<Image>());
                }
            }
            customizationItemIndicators.Add(indicatorsOfItem);
        }
    }
    void SetIndicatorActivity()
    {
        /*------------------- SHOP ITEMS -----------------*/
        for (int i = 0; i < shopItems.Count; i++)
        {
            for (int x = 0; x < shopItemIndicators[i].Count; x++)
            {
                var indicator = shopItemIndicators[i][x];
                indicator.color = Color.black;
            }
        }
        for (int i = 0; i < shopItems.Count; i++)
        {
            for (int x = 0; x < shopItems[i].level; x++)
            {
                var indicator = shopItemIndicators[i][x];
                indicator.color = Color.white;
            }
        }
        /*------------------- CUSTOMIZATION ITEMS -----------------*/
        for (int i = 0; i < customizationData.Count; i++)
        {
            for (int x = 0; x < customizationItemIndicators[i].Count; x++)
            {
                var indicator = customizationItemIndicators[i][x];
                indicator.color = Color.black;
            }
        }
        for (int i = 0; i < customizationItemIndicators.Count; i++)
        {
            for (int x = 0; x < customizationData[i].level; x++)
            {
                if (customizationData[i].isPicked)
                {
                    var indicator = customizationItemIndicators[i][x];
                    indicator.color = Color.white;
                }
            }
        }
    }
    int GetPriceAtLevel(ShopItem item)
    {
        var currentValue = item.priceAtLevel.Evaluate(item.level);
        var nextValue = item.priceAtLevel.Evaluate(item.level + 1);

        var priceAtLevel = nextValue - currentValue;
        return Mathf.CeilToInt(priceAtLevel);
    }
    int GetPriceAtLevel(Customization item)
    {
        var currentValue = item.priceAtLevel.Evaluate(item.level);
        var nextValue = item.priceAtLevel.Evaluate(item.level + 1);

        var priceAtLevel = nextValue - currentValue;
        return Mathf.CeilToInt(priceAtLevel);
    }
    void SetShopItemPrices()
    {
        /*------------------- SHOP ITEMS -----------------*/
        for (int i = 0; i < shopItems.Count; i++)
        {
            shopItems[i].price = GetPriceAtLevel(shopItems[i]);
        }

        /*------------------- CUSTOMIZATION ITEMS -----------------*/
        for (int i = 0; i < customizationData.Count; i++)
        {
            customizationData[i].price = GetPriceAtLevel(customizationData[i]);
        }
    }
    void SetPriceTexts()
    {
        /*------------------- SHOP ITEMS -----------------*/
        for (int i = 0; i < priceTexts.Count; i++)
        {
            if (shopItems[i].isSoldOut)
                priceTexts[i].text = "Sold Out";
            else
                priceTexts[i].text = "$" + shopItems[i].price.ToString("N0");
        }
        /*------------------- CUSTOMIZATION ITEMS -----------------*/
        for (int i = 0; i < customizationTexts.Count; i++)
        {
            if (customizationData[i].isUnlocked)
            {
                if (customizationData[i].isPicked)
                    customizationTexts[i].text = "In Use";
                else
                    customizationTexts[i].text = "Click To Use";
            }
            else
            {
                customizationTexts[i].text = "$" + customizationData[i].price.ToString("N0");
            }
        }
    }
    public void SetCurrencyText()
    {
        for (int i = 0; i < moneyText.Count; i++)
            moneyText[i].text = "$" + currencyData.currencyValue.ToString("N0");
    }
    private void UpdateShopUI()
    {
        /*------------------- SHOP ITEMS -----------------*/
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (currencyData.currencyValue >= shopItems[i].price && !shopItems[i].isSoldOut)
            {
                shopButtons[i].interactable = true;
            }
            else
            {
                shopButtons[i].interactable = false;
            }
        }
        /*------------------- CUSTOMIZATION ITEMS -----------------*/
        for (int i = 0; i < customizationData.Count; i++)
        {
            if (customizationData[i].isUnlocked)
            {
                if (customizationData[i].isPicked)
                    customizationButtons[i].interactable = false;
                else
                    customizationButtons[i].interactable = true;
            }
            else
            {
                if (customizationData[i].price < currencyData.currencyValue)
                    customizationButtons[i].interactable = true;
                else
                    customizationButtons[i].interactable = false;
            }
        }
    }
    void IncreaseItemLevel(ShopItem shopItem)
    {
        shopItem.level++;
    }
    void IncreaseItemLevel(Customization customization)
    {
        customization.level++;
    }
    void SpendCurrency(ShopItem shopItem)
    {
        currencyData.currencyValue -= shopItem.price;
    }
    void SpendCurrency(Customization customization)
    {
        currencyData.currencyValue -= customization.price;
    }
    void CheckItemLevel(ShopItem shopItem)
    {
        if (shopItem.level >= shopItem.maxLevel)
        {
            shopItem.isSoldOut = true;
        }
        else
        {
            shopItem.isSoldOut = false;
        }
    }
    void CheckItemLevel(Customization customization)
    {
        if (customization.level >= 1)
        {
            customization.isUnlocked = true;
        }
        else
        {
            customization.isUnlocked = false;
        }
    }
    void ApplyEffect(ShopItem shopItem)
    {
        var index = shopItems.IndexOf(shopItem);
        switch (index)
        {
            case 0:
                playerData.health++;
                break;
            case 1:
                scoreData.maxScoreMultiplier += 2;
                break;
            case 2:
                playerData.isBulletUnlocked = true;
                if (shopItems[index].level == 1)
                {
                    playerData.bulletCount += 6;
                    playerData.killsToSpawnBullet += 9;
                }
                else
                {
                    playerData.bulletCount++;
                    if (IsOdd(shopItems[index].level))
                        playerData.killsToSpawnBullet--;
                }
                break;
            case 3:
                boxData[4].isUnlocked = true;
                SetSpawnPoolList();
                break;
            case 4:
                boxData[5].isUnlocked = true;
                SetSpawnPoolList();
                break;
            case 5:
                playerData.isBig = true;
                break;
            case 6:
                playerData.hasLaserRope = true;
                break;
            case 7:
                playerData.ropeGrapSpeed += 0.02f;
                playerData.ropeDampingRatio -= 0.075f;
                break;
            case 8:
                boxData[6].isUnlocked = true;
                SetSpawnPoolList();
                break;
            case 9:
                if (!boxData[7].isUnlocked)
                {
                    boxData[7].isUnlocked = true;
                    powerupData[1].duration = 5f;
                    SetSpawnPoolList();
                }
                else
                {
                    powerupData[1].duration += 1.5f;
                }
                break;
        }
    }
    void SetCustomizationInfo(Customization item)
    {
        for (int i = 0; i < customizationData.Count; i++)
        {
            if(item.customizationType == Customization.CustomizationType.Body)
            {
                if(customizationData[i].customizationType == Customization.CustomizationType.Body)
                {
                    customizationData[i].isPicked = false;
                }
            }
            else if(item.customizationType == Customization.CustomizationType.Rope)
            {
                if (customizationData[i].customizationType == Customization.CustomizationType.Rope)
                {
                    customizationData[i].isPicked = false;
                }
            }
        }

        item.isPicked = true;
    }
    public bool IsOdd(int value)
    {
        if (value % 2 == 1)
            return true;
        else
            return false;
    }
    public void BuyItem(ShopItem item)
    {
        // reduce value of currency with price
        SpendCurrency(item);
        // set new price for item and increase item level
        IncreaseItemLevel(item);
        SetShopItemPrices();
        SetIndicatorActivity();
        CheckItemLevel(item);
        SetPriceTexts();
        // Check item level
        // apply bought effect
        ApplyEffect(item);
        UpdateShopUI();
        // play purchase feedback
        GameSaveManager.instance.SaveAll();
    }
    public void BuyItem(Customization item)
    {
        if (!item.isUnlocked)
        {
            // reduce value of currency with price
            SpendCurrency(item);
            // set new price for item and increase item level
            IncreaseItemLevel(item);
            SetShopItemPrices();
            SetIndicatorActivity();
            CheckItemLevel(item);
            SetPriceTexts();
            // Check item level
            // apply bought effect
            UpdateShopUI();
            // play purchase feedback
            GameSaveManager.instance.SaveAll();
        }
    }
    public void SwapItem(Customization item)
    {
        if (item.isUnlocked)
        {
            SetCustomizationInfo(item);
            SetIndicatorActivity();
            SetPriceTexts();
            UpdateShopUI();
            SetPlayerDataCustomization();

            GameSaveManager.instance.SaveAll();
        }
    }
}
