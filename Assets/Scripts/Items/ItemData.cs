using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite itemImage;
    public String itemName;

    public bool isConsumable;
    public int healAmount;
}
