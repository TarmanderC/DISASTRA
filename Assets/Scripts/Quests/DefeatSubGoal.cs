using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDefeatSubGoal", menuName = "Scriptable Objects/Sub Goals/Defeat Enemies")]
public class DefeatSubGoal : QuestSubGoal
{
    public String enemyType;

    public override bool IsCompleted()
    {
        if (currentCount >= requiredCount) {
            completed = true;
        }
        return currentCount >= requiredCount;
    }

    public override void UpdateStatus(Inventory inventory)
    {
        throw new NotImplementedException();
    }

    private void OnEnable() {
        completed = false;
        currentCount = 0;
    }
}
