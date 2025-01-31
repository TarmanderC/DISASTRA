using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuest = new List<Quest>();

    public PlayerInventory playerInventory;

    public Animator questAnimator;

    public void StartQuest(Quest quest) {
        activeQuest.Add(quest);

        questAnimator.SetTrigger("NewParty");
    }

    public void FinishTalkQuest(TalkToSubGoal subGoal) {
        subGoal.UpdateStatus(playerInventory.inventory);
    }
}
