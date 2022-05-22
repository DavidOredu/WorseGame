using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Customization", menuName = "Data/Customization")]
public class Customization : ScriptableObject
{
    public CustomizationType customizationType;
    public Color color;
    public int level;
    public int price;
    public bool isUnlocked;
    public bool isPicked;
    public AnimationCurve priceAtLevel;

    public enum CustomizationType
    {
        Body,
        Rope,
    }
}
