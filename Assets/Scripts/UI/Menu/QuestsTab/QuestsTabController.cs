using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class QuestsTabController : MonoBehaviour
{
    public KnightBattle knightBattle;
    public MenuController menuController;
    public QuestManager questManager;
    public List<Image> questTabImages = new List<Image>();

    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;
    public Transform subGoalPanel;
    public List<GameObject> subGoalObjects = new List<GameObject>();
    public GameObject subGoalPrefab;
    public GameObject doneButton;

    public TextMeshProUGUI questGoldRewardText;
    public TextMeshProUGUI questEXPRewardText;

    public GameObject questListContainer;
    public GameObject questSelectPrefab;

    public GameObject questInfoPanel;


    private int currentQuestIndex;

    void Start() {
        ActivateTab(0);
    }

    public void CompleteQuest() {
        // If the goal is a collecting subgoal, then it removes the item from the inventory
        Quest currentQuest = questManager.activeQuest[currentQuestIndex];
        Inventory inventory = questManager.playerInventory.inventory;

        for (int i = 0; i < currentQuest.subGoals.Count; i++) {
            if (currentQuest.subGoals[i] is CollectSubGoal) {
                CollectSubGoal currentSubGoal = (CollectSubGoal) currentQuest.subGoals[i];
                inventory.Remove(currentSubGoal.itemData, currentSubGoal.requiredCount);
            }
        }

        // Gives the reward to the player
        CharacterData.currentExp += currentQuest.rewardEXP;
        knightBattle.Gold += currentQuest.rewardCoins;

        // Resets the quest in case of future use
        questManager.activeQuest[currentQuestIndex].ResetQuest();
        questManager.activeQuest.Remove(questManager.activeQuest[currentQuestIndex]);
        ActivateTab(currentQuestIndex);

        menuController.playerInventory.UpdateInventory();
    }

    public void ActivateTab(int tabNo) {
        UpdateTabs();
        if (tabNo < questTabImages.Count) {
            questInfoPanel.SetActive(true);

            currentQuestIndex = tabNo;

            for (int i = 0; i < questTabImages.Count; i++) {
                questTabImages[i].color = Color.grey;
            }

            questTabImages[tabNo].color = Color.white;
            UpdateStatsPage(tabNo);
        } else {
            questInfoPanel.SetActive(false);
        }
    }

    public void UpdateTabs() {
        questTabImages.Clear();
        foreach(Transform child in questListContainer.transform) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < questManager.activeQuest.Count; i++) {
            GameObject questTab = Instantiate(questSelectPrefab, questListContainer.transform);
            Button btn = questTab.GetComponent<Button>();

            int index = i;
            btn.onClick.AddListener(() => {
                ActivateTab(index);
            });

            questTabImages.Add(questTab.GetComponent<Image>());
        }

        for (int i = 0; i < questTabImages.Count; i++) {
            GameObject currentName = questTabImages[i].transform.Find("DescriptionText").gameObject;

            // Name
            currentName.GetComponent<TextMeshProUGUI>().text = questManager.activeQuest[i].title;
        }
    }

    public void UpdateStatsPage(int tabNum) {
        // Title
        questNameText.text = questManager.activeQuest[tabNum].title;

        // Description
        descriptionText.text = questManager.activeQuest[tabNum].description;
        
        // SubGoals
        UpdateSubGoal(tabNum);

        // Rewards
        questGoldRewardText.text = "" + questManager.activeQuest[tabNum].rewardCoins;
        questEXPRewardText.text = "" + questManager.activeQuest[tabNum].rewardEXP;

        // Done Button
        doneButton.SetActive(questManager.activeQuest[tabNum].IsCompleted());
    }

    private void UpdateSubGoal(int questIndex) {
        subGoalObjects.Clear();
        foreach(Transform child in subGoalPanel) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < questManager.activeQuest[questIndex].subGoals.Count; i++) {
            GameObject subGoalTab = Instantiate(subGoalPrefab, subGoalPanel);

            int index = i;

            subGoalObjects.Add(subGoalTab);
        }

        for (int i = 0; i < subGoalObjects.Count; i++) {
            QuestSubGoal currentSubGoal = questManager.activeQuest[questIndex].subGoals[i];

            currentSubGoal.UpdateStatus(questManager.playerInventory.inventory);

            GameObject description = subGoalObjects[i].transform.Find("DescriptionText").gameObject;
            GameObject completeCheck = subGoalObjects[i].transform.Find("CompleteCheck").gameObject;
            GameObject CompletionText = subGoalObjects[i].transform.Find("CompletionText").gameObject;

            description.GetComponent<TextMeshProUGUI>().text = currentSubGoal.description;
            completeCheck.SetActive(currentSubGoal.completed);
            CompletionText.GetComponent<TextMeshProUGUI>().text = currentSubGoal.currentCount + "/" + currentSubGoal.requiredCount;
        }
    }
}
