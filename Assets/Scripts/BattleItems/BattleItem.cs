using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleItem", menuName = "Scriptable Objects/BattleItem")]
public class BattleItem : ScriptableObject
{
    public String itemName;
    public int healAmount;
    public int TU;
}
