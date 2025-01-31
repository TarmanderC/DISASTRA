using System;
using UnityEngine;

public abstract class QuestSubGoal : ScriptableObject
{
    public String description;
    public bool completed = false;

    public int requiredCount;
    public int currentCount;

    public abstract bool IsCompleted();
    public abstract void UpdateStatus(Inventory inventory);
}
