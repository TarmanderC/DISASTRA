using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectSubGoal", menuName = "Scriptable Objects/Sub Goals/Collect Items")]
public class CollectSubGoal : QuestSubGoal
{
    public ItemData itemData;

    public override void UpdateStatus(Inventory inventory) {
        int number = 0;

        for (int i = 0; i < inventory.slots.Count; i++) {
            if (inventory.slots[i].itemData != null && inventory.slots[i].itemData.Equals(itemData)) {
                number += inventory.slots[i].count;
            }
        }

        currentCount = number;

        IsCompleted();
    }

    public override bool IsCompleted() {
        if (currentCount >= requiredCount) {
            completed = true;
        }
        return currentCount > requiredCount;
    }

    private void OnEnable() {
        completed = false;
        currentCount = 0;
    }
}
