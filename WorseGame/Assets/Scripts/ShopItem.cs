using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Data/Shop Item")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public int price;
    public int level;
    public int maxLevel;
    public bool isSoldOut;
    public AnimationCurve priceAtLevel;
}
