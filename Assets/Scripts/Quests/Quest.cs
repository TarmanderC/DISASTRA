using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public String title;
    public String description;
    public Sprite icon;
    public int rewardCoins;
    public int rewardEXP;

    [Header("Sub Goals")]
    public List<QuestSubGoal> subGoals;

    public bool IsCompleted() {
        for (int i = 0; i < subGoals.Count; i++) {
            if (!subGoals[i].completed) {
                return false;
            }
        }

        return true;
    }

    public void ResetQuest() {
        for (int i = 0; i < subGoals.Count; i++) {
            subGoals[i].completed = false;
            subGoals[i].currentCount = 0;
        }
    }
}
