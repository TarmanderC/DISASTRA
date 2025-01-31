using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Attack", menuName = "Combat/Attack")]
public class Attack : ScriptableObject
{
    public String attackName;
    public int TUCost;
    public int damage;

    public bool doesItSlide;

    public bool isAOE;
}
