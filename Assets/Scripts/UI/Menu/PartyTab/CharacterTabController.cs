using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class CharacterTabController : MonoBehaviour
{
    public List<Image> playerTabImages = new List<Image>();
    public GameObject playerTabPrefab;
    public GameManager gm;
    public Transform selectPlayerFormPanel;

    public GameObject nameObject;
    public GameObject characterImage;
    public GameObject levelNum;
    public GameObject HPObject;
    public GameObject attackObject;
    public GameObject HPBar;
    public GameObject EXPBar;

    void Start() {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo) {
        if (tabNo < playerTabImages.Count) {
            for (int i = 0; i < playerTabImages.Count; i++) {
                playerTabImages[i].color = Color.grey;
            }

            playerTabImages[tabNo].color = Color.white;
            UpdateTabs();
            UpdateStatsPage(tabNo);
        }
    }

    public void UpdateTabs() {
        playerTabImages.Clear();
        foreach(Transform child in selectPlayerFormPanel) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < gm.allPlayers.Count; i++) {
            GameObject playerForm = Instantiate(playerTabPrefab, selectPlayerFormPanel);
            Button btn = playerForm.GetComponent<Button>();

            int index = i;
            btn.onClick.AddListener(() => {
                ActivateTab(index);
            });

            playerTabImages.Add(playerForm.GetComponent<Image>());

            gm.allPlayers[i].currentHealth = gm.allPlayers[i].characterData.currentHealth;
        }

        for (int i = 0; i < playerTabImages.Count; i++) {
            GameObject currentName = playerTabImages[i].transform.Find("Name Text").gameObject;
            GameObject currentLevel = playerTabImages[i].transform.Find("Level Text").gameObject;
            GameObject currentImage = playerTabImages[i].transform.Find("Circle").transform.Find("Icon").gameObject;

            // Name
            currentName.GetComponent<TextMeshProUGUI>().text = gm.allPlayers[i].characterData.characterName;

            // Level
            int numLevel = CharacterData.level;
            currentLevel.GetComponent<TextMeshProUGUI>().text = "Level " + numLevel;

            // Image
            Sprite icon = gm.allPlayers[i].characterData.characterImage;
            currentImage.GetComponent<Image>().sprite = icon;
        }
    }

    public void UpdateStatsPage(int tabNum) {
        // Name
        nameObject.GetComponent<TextMeshProUGUI>().text = gm.allPlayers[tabNum].characterData.characterName;

        // Level
        int numLevel = CharacterData.level;
        levelNum.GetComponent<TextMeshProUGUI>().text = "Level " + numLevel;

        // Image
        Sprite icon = gm.allPlayers[tabNum].characterData.characterImage;
        characterImage.GetComponent<Image>().sprite = icon;

        // HP
        int numMaxHP = gm.allPlayers[tabNum].characterData.maxHealth;
        HPObject.GetComponent<TextMeshProUGUI>().text = "HP: " + numMaxHP;

        // ATK
        int numATK = gm.allPlayers[tabNum].characterData.baseAttack;
        attackObject.GetComponent<TextMeshProUGUI>().text = "ATK: " + numATK;


        // HP Bar
        Slider slidey = HPBar.GetComponent<Slider>();
        slidey.interactable = false;

        int playerCurrentHealth = gm.allPlayers[tabNum].characterData.currentHealth;
        
        float num = (float)playerCurrentHealth / numMaxHP;

        slidey.value = num;

        TextMeshProUGUI HPText = HPBar.transform.Find("CurrentHPText").gameObject.GetComponent<TextMeshProUGUI>();
        HPText.text = $"{playerCurrentHealth}/{numMaxHP}";

        // EXP Bar
        Slider slidey2 = EXPBar.GetComponent<Slider>();
        slidey2.interactable = false;

        int playerCurrentEXP = CharacterData.currentExp;
        
        float num2 = (float)playerCurrentEXP / CharacterData.maxExp;

        slidey2.value = num2;

        TextMeshProUGUI HPText2 = EXPBar.transform.Find("CurrentHPText").gameObject.GetComponent<TextMeshProUGUI>();
        HPText2.text = $"{playerCurrentEXP}/{CharacterData.maxExp}";
    }
}
