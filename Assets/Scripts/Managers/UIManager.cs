using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform attackButtonContainer;
    public GameObject actionButtonPrefab;
    public GameObject cancelButton;
    public GameObject enemySelectPanel;
    public GameObject[] enemySelectButtons = new GameObject[4];
    public List<Transform> enemySelectTransforms = new List<Transform>();
    public int selectedEnemyIndex;
    public Canvas canvas;


    public GameObject actionPanel;
    public GameObject actionButtonPanel;

    public GameObject timeline;
    public List<GameObject> timelineList = new List<GameObject>();

    public GameObject healthBarPanel;
    public GameObject enemyHealthPanel;
    public List<GameObject> playerHealthList = new List<GameObject>();
    public List<GameObject> enemyHealthList = new List<GameObject>();



    private BattleManager bm;


    public GameObject attackButton;
    public GameObject itemButton;
    public GameObject attacksPanel;
    public GameObject itemsPanel;

    public GameObject backButton;


    public GameObject quickTimeEventBar;
    public QuickTimeEvent quickTimeEvent;


    public GameObject resultsScreen;
    public int goldAmount;
    public List<Collectable> itemAwards = new List<Collectable>();
    public GameObject itemAwardPrefab;

    void Start() {
        cancelButton.GetComponent<Button>().onClick.AddListener(() => {
            ShowEnemySelection(false);
                
        });

        actionPanel.SetActive(false);

        bm = gameObject.GetComponent<BattleManager>();

        quickTimeEventBar.SetActive(false);

        attackButton.SetActive(false);
        itemButton.SetActive(false);
        attacksPanel.SetActive(false);
        itemsPanel.SetActive(false);
    }

    void Update() {

    }
    
    public void DisplayAttackButtons(Character character, List<Attack> attacks) {
        ClearButtons();
        //Debug.Log("UIManager: Display Attack Buttons");

        foreach (var attack in attacks) {
            GameObject button = Instantiate(actionButtonPrefab, attackButtonContainer);
            button.GetComponentInChildren<TMP_Text>().text = attack.attackName;
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(() => {
                character.SetCurrentAttack(attack);
                
            });
        }

        cancelButton.GetComponent<Button>().onClick.AddListener(() => {
            character.clearAttack();
                
        });

        ShowCancelButton(false);

        enemySelectTransforms.Clear();
        for (int i = 0; i < bm.enemies.Length; i++) {
            enemySelectTransforms.Add(bm.enemies[i].transform);
        }

        attackButton.SetActive(true);
        itemButton.SetActive(true);
        attacksPanel.SetActive(false);
        itemsPanel.SetActive(false);
    }

    public void updateTimeline() {
        //Debug.Log("UIManager: Update Timeline");
        foreach(GameObject child in timelineList) {
            Destroy(child);
        }

        timelineList.Clear();

        foreach (Character character in bm.queue) {
            GameObject icon = Instantiate(character.timelineIcon, timeline.transform);
            timelineList.Add(icon);
        }
    }

    public void StartPlayerHealth() {
        //Debug.Log("UIManager: Start Player Health");
        
        playerHealthList.Clear();

        int[] customOrder = { 2, 0, 1, 3 };
        for (int i = 0; i < customOrder.Length; i++) {
            int playerIndex = customOrder[i];
            if (playerIndex < bm.players.Length) {
                GameObject icon = Instantiate(bm.players[playerIndex].GetComponent<Character>().healthIcon, healthBarPanel.transform);

                Slider slider = icon.transform.Find("Bar").GetComponent<Slider>();
                slider.interactable = false;
                playerHealthList.Add(icon);

                //Debug.Log($"Instantiating health bar for player {playerIndex} ({bm.players[playerIndex].GetComponent<Character>().characterName})");
            }
        }
    }

    public void StartEnemyHealth() {
        //Debug.Log("UIManager: Start Enemy Health");

        enemyHealthList.Clear();

        int[] customOrder = { 2, 0, 1, 3 };
        for (int i = 0; i < customOrder.Length; i++) {
            int enemyIndex = customOrder[i];
            if (enemyIndex < bm.enemies.Length) {
                GameObject icon = Instantiate(bm.enemies[enemyIndex].GetComponent<Character>().healthIcon, enemyHealthPanel.transform);

                Slider slider = icon.transform.Find("Bar").GetComponent<Slider>();
                slider.interactable = false;
                enemyHealthList.Add(icon);

                //Debug.Log($"Instantiating health bar for enemy {enemyIndex} ({bm.enemies[enemyIndex].GetComponent<Character>().characterName})");
            }
        }
    }

    public void UpdatePlayerHealth() {
        //Debug.Log("UIManager: Update Player Health");

        // Define the custom order
        int[] customOrder;
        if (bm.players.Length == 1) {
            customOrder = new int[1];
            customOrder[0] = 0;
        } else if (bm.players.Length == 2) {
            customOrder = new int[2];
            customOrder[0] = 0;
            customOrder[1] = 1;
        } else if (bm.players.Length == 3) {
            customOrder = new int[3];
            customOrder[0] = 2;
            customOrder[1] = 0;
            customOrder[2] = 1;
        } else {
            customOrder = new int[4];
            customOrder[0] = 2;
            customOrder[1] = 0;
            customOrder[2] = 1;
            customOrder[3] = 3;
        }

        for (int i = 0; i < customOrder.Length; i++) {
            int playerIndex = customOrder[i]; // Map to custom order
            if (playerIndex < bm.players.Length && i < playerHealthList.Count) {

                if (playerHealthList[i] == null) {
                    continue;
                }
                Character player = bm.players[playerIndex].GetComponent<Character>();
                Transform barTransform = playerHealthList[i].transform.Find("Bar");

                if (barTransform == null || barTransform.parent == null) {
                    continue; // Skip to the next iteration if the bar or its parent is missing
                }
                GameObject bar = barTransform.gameObject;
                GameObject text = bar.transform.Find("Health txt").gameObject;

                if (player != null) {
                    if (player.isDead) {
                        //GameObject parentObject = bar.transform.parent.gameObject;
                        //Destroy(parentObject);
                        bar.GetComponent<Slider>().value = 0;
                        text.GetComponent<TextMeshProUGUI>().text = $"HP 0/{player.characterData.maxHealth}";
                    } else {
                        int playerCurrentHealth = player.currentHealth;
                        int playerMaxHealth = player.characterData.maxHealth;
                        float value = (float)playerCurrentHealth / playerMaxHealth;

                        bar.GetComponent<Slider>().value = value;
                        text.GetComponent<TextMeshProUGUI>().text = $"HP {playerCurrentHealth}/{playerMaxHealth}";
                    }
                }
            }

        }
    }

    public void UpdateEnemyHealth() {
        //Debug.Log("UIManager: Update Enemy Health");

        // Define the custom order
        int[] customOrder;
        if (bm.enemies.Length == 1) {
            customOrder = new int[1];
            customOrder[0] = 0;
        } else if (bm.enemies.Length == 2) {
            customOrder = new int[2];
            customOrder[0] = 0;
            customOrder[1] = 1;
        } else if (bm.enemies.Length == 3) {
            customOrder = new int[3];
            customOrder[0] = 2;
            customOrder[1] = 0;
            customOrder[2] = 1;
        } else {
            customOrder = new int[4];
            customOrder[0] = 2;
            customOrder[1] = 0;
            customOrder[2] = 1;
            customOrder[3] = 3;
        }

        for (int i = 0; i < customOrder.Length; i++) {
            int enemyIndex = customOrder[i]; // Map to custom order
            if (enemyIndex < bm.enemies.Length && i < enemyHealthList.Count) {

                if (enemyHealthList[i] == null) {
                    continue;
                }
                Character enemy = bm.enemies[enemyIndex].GetComponent<Character>();
                Transform barTransform = enemyHealthList[i].transform.Find("Bar");

                if (barTransform == null || barTransform.parent == null) {
                    continue; // Skip to the next iteration if the bar or its parent is missing
                }

                GameObject bar = barTransform.gameObject;
                GameObject text = bar.transform.Find("Health txt").gameObject;
                GameObject name = bar.transform.Find("Name").gameObject;

                if (enemy != null) {
                    if (enemy.isDead) {
                        GameObject parentObject = bar.transform.parent.gameObject;
                        Destroy(parentObject);
                    } else {
                        int enemyCurrentHealth = enemy.currentHealth;
                        int enemyMaxHealth = enemy.characterData.maxHealth;
                        float value = (float)enemyCurrentHealth / enemyMaxHealth;

                        bar.GetComponent<Slider>().value = value;
                        text.GetComponent<TextMeshProUGUI>().text = $"HP {enemyCurrentHealth}/{enemyMaxHealth}";
                        name.GetComponent<TextMeshProUGUI>().text = bm.enemies[enemyIndex].GetComponent<Character>().characterName;
                    }
                }
            }
        }
    }

    public void setQuickTimePositionSlide(Vector3 position) {
        //Debug.Log("UIManager: set Quick Time Position");

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 uiLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.worldCamera,
            out uiLocalPoint
        );

        quickTimeEventBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(uiLocalPoint.x, uiLocalPoint.y + 180);
    }

    public void setQuickTimePositionNoSlide(Character character) {
        //Debug.Log("UIManager: Set Quick Time Position Above Character");

        Vector3 characterWorldPosition = character.transform.position;

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(characterWorldPosition);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 uiLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.worldCamera,
            out uiLocalPoint
        );
        quickTimeEventBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(uiLocalPoint.x, uiLocalPoint.y + 180);
    }


    public void ClearButtons() {
        //Debug.Log("UIManager: Clear Buttons");
        foreach(Transform child in attackButtonContainer) {
            Destroy(child.gameObject);
        }
    }

    public void ShowCancelButton(bool show) {
        //Debug.Log("UIManager: Show Cancel Button");
        cancelButton.SetActive(show);
    }

    public void ShowEnemySelection(bool show) {
        //Debug.Log("UIManager: Show Enemy Selection");
        CheckEnemySelection();
        enemySelectPanel.SetActive(show);
        ShowCancelButton(show);
        attackButtonContainer.gameObject.SetActive(!show);
        itemsPanel.SetActive(!show);
        attackButton.gameObject.SetActive(!show);
        itemButton.gameObject.SetActive(!show);
        backButton.SetActive(!show);
    }

    public void CheckEnemySelection() {
        enemySelectButtons[0].SetActive(false);
        enemySelectButtons[1].SetActive(false);
        enemySelectButtons[2].SetActive(false);
        enemySelectButtons[3].SetActive(false);

        for (int i = 0; i < bm.enemies.Length; i++) {
            if (!bm.enemies[i].GetComponent<Character>().isDead) {
                enemySelectButtons[i].SetActive(true);
            } else {
                enemySelectButtons[i].SetActive(false);
            }
        }
    }

    public void UpdateResultsScreen() {
        GameObject BG = resultsScreen.transform.Find("BG").gameObject;

        GameObject goldPanel = BG.transform.Find("GoldPanel").gameObject;
        GameObject EXPPanel = BG.transform.Find("EXPPanel").gameObject;

        GameObject goldText = goldPanel.transform.Find("GoldText").gameObject;
        GameObject EXPText = EXPPanel.transform.Find("EXPText").gameObject;

        goldText.GetComponent<TextMeshProUGUI>().text = "" + goldAmount;
        EXPText.GetComponent<TextMeshProUGUI>().text = "" + (int) (goldAmount / 5.5);


        // Items Section
        GameObject ItemsPanel = BG.transform.Find("ItemsPanel").gameObject;
        GameObject container = ItemsPanel.transform.Find("Container").gameObject;

        foreach(Transform child in container.transform) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < itemAwards.Count; i++) {
            GameObject item = Instantiate(itemAwardPrefab, container.transform);

            GameObject icon = item.transform.Find("Image").gameObject;
            GameObject number = item.transform.Find("Number").gameObject;

            icon.GetComponent<Image>().sprite = itemAwards[i].itemData.itemImage;
            number.GetComponent<TextMeshProUGUI>().text = "" + itemAwards[i].count;
        }

    }

    public void ResetUI() {
        //Debug.Log("UIManager: Resetting UI");

        // Clear health bars
        for (int i = 0; i < playerHealthList.Count; i++) {
            Destroy(playerHealthList[i]);
        }

        for (int i = 0; i < enemyHealthList.Count; i++) {
            Destroy(enemyHealthList[i]);
        }

        playerHealthList.Clear();
        enemyHealthList.Clear();

        // Clear timeline
        for (int i = 0; i < timelineList.Count; i++) {
            Destroy(timelineList[i]);
        }
        timelineList.Clear();

        // Reset other UI elements
        ClearButtons();
        ShowEnemySelection(false);
        ShowCancelButton(false);

        enemySelectTransforms.Clear();

        resultsScreen.SetActive(false);

        //Debug.Log("UIManager: UI reset complete.");
    }

    public void HideActionUI() {
        //Debug.Log("UIManager: Hide Action UI");
        ClearButtons();
        ShowCancelButton(false);
        ShowEnemySelection(false);
        backButton.SetActive(false);
    }

    public void OpenAttacks() {
        attackButton.SetActive(false);
        itemButton.SetActive(false);

        attacksPanel.SetActive(true);
        backButton.SetActive(true);
    }

    public void OpenItems() {
        attackButton.SetActive(false);
        itemButton.SetActive(false);

        itemsPanel.SetActive(true);
        backButton.SetActive(true);
    }

    public void GoBack() {
        attackButton.SetActive(true);
        itemButton.SetActive(true);

        itemsPanel.SetActive(false);
        attacksPanel.SetActive(false);
        backButton.SetActive(false);
    }
}
