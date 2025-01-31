using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    public GameManager gm;
    private Camera cams;
    public GameObject[] players;
    public GameObject[] enemies;
    public bool isBattleActive = false;
    private Transform playerTransform;
    public Transform cameraTransform;

    private KnightMovement playerMovement;
    public KnightBattle playerBattle;


    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public Vector3[] playerPositions;
    public Vector3[] enemyPositions;



    public List<Character> queue = new List<Character>();
    public UIManager uiManager;



    public bool isEvent;



    private bool isSliding = false; // Flag to check if sliding is active
    private float slideSpeed = 5f; // Speed of the sliding motion

    private Vector3 slideTargetPosition;


    private enum SlideState { None, SlidingToEnemy, SlidingBack }
    private SlideState slideState = SlideState.None;





    public Character currentCharacter;
    public Vector3 currentPosition;

    public GameObject cancelButton;
    public GameObject enemySelectPanel;
    public GameObject actionPanel;

    public GameObject selectedEnemy;

    public CinemachineVirtualCamera cvc;
    public GameObject allCameras;

    public PlayerInventory playerInventory;


    private void Awake() {
        cams = cameraTransform.GetComponent<Camera>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        playerMovement = GameObject.Find("Player").GetComponent<KnightMovement>();
        cameraTransform.position = new Vector3(0,0,-10);
        selectedEnemy = null;

        isEvent = false;

        playerBattle = playerMovement.gameObject.GetComponent<KnightBattle>();
        for (int i = 0; i < playerBattle.currentParty.Count; i++) {
            playerBattle.currentParty[i].GetComponent<Character>().characterData.ResetHealth();
        }

        gm.SetUpGM();
        uiManager.resultsScreen.SetActive(false);

        //Debug.Log(playerBattle.Gold);
    }

    public void StartBattle(List<GameObject> thePlayers, GameObject[] theEnemies, int goldAmount, List<Collectable> itemAwards) {
        
        if (!isBattleActive) {
            //Debug.Log("BattleManager: Start Battle");

            allCameras.gameObject.SetActive(false);
            cvc.m_Lens.OrthographicSize = 5;
            cams.orthographicSize = 5;

            actionPanel.SetActive(true);
            playerMovement.canMove = false;

            isBattleActive = true;
            cameraTransform.position = new Vector3(-25,-26,-10);

            InitializeUnits(thePlayers, theEnemies);
            InitializeQueue();

            uiManager.updateTimeline();
            uiManager.StartPlayerHealth();
            uiManager.StartEnemyHealth();

            uiManager.UpdatePlayerHealth();
            uiManager.UpdateEnemyHealth();

            uiManager.goldAmount = goldAmount;
            uiManager.itemAwards = itemAwards;

            uiManager.enemySelectPanel.SetActive(false);
            
            StartNextTurn();
        }
    }

    public void updateUI(bool isOn) {
        //Debug.Log("BattleManager: updateUI");
        cancelButton.SetActive(isOn);
        enemySelectPanel.SetActive(isOn);
    }

    private void InitializeQueue()
    {
        //Debug.Log("BattleManager: Initialize Queue");
        queue = new List<Character>();
        for (int i = 0; i < players.Length; i++) {
            queue.Add(players[i].GetComponent<Character>());
        }
        for (int i = 0; i < enemies.Length; i++) {
            queue.Add(enemies[i].GetComponent<Character>());
        }
        queue.Sort((a, b) => a.TU.CompareTo(b.TU));
    }

    public void StartNextTurn()
    {
        //Debug.Log("BattleManager: Start Next Turn");

        if (!isBattleActive || queue.Count == 0) {
            //Debug.Log("Battle Over!");
            return;
        }

        currentCharacter = queue[0];
        queue.RemoveAt(0);

        if (currentCharacter.currentHealth <= 0) {
            StartNextTurn();
            return;
        }

        //Debug.Log($"It's {currentCharacter.characterName}'s turn.");

        currentCharacter.StartTurn(this);
    }

    public void StartEnemyTurn(Character enemy) {
        //Debug.Log($"BattleManager: Starting turn for {enemy.characterName}");

        EnemyAI enemyAI = GetComponent<EnemyAI>();

        Attack chosenAttack = enemyAI.DetermineAttack(enemy);
        Character target = enemyAI.DetermineTarget(players);

        enemy.currentAttack = chosenAttack;
        selectedEnemy = target.gameObject;

        currentPosition  = currentCharacter.gameObject.transform.position;

        if (chosenAttack.doesItSlide) {
            Slide(new Vector3(selectedEnemy.transform.position.x + 2f, selectedEnemy.transform.position.y), true);
        } else {
            currentCharacter.anim.SetBool("isFighting", true);
        }
    }

    public void SelectEnemy(int index) {
        //Debug.Log("BattleManager: Select Enemy");
        selectedEnemy = enemies[index];

        uiManager.ShowEnemySelection(false);
        uiManager.attackButtonContainer.gameObject.SetActive(false);
        uiManager.attackButton.gameObject.SetActive(false);
        uiManager.itemButton.gameObject.SetActive(false);
        uiManager.itemsPanel.SetActive(false);
        uiManager.backButton.SetActive(false);

        //Debug.Log($"{currentCharacter.characterName} attacks {selectedEnemy.GetComponent<Character>().characterName}!");


        currentPosition  = currentCharacter.gameObject.transform.position;
        
        // Check if the attack requires sliding
        if (currentCharacter.currentAttack.doesItSlide) {
            Slide(new Vector3(selectedEnemy.transform.position.x - 2f, selectedEnemy.transform.position.y), true);
        } else {
            DoQuickTimeEvent(); // Directly trigger quick time event without sliding
        }
    }

    private void InitializeUnits(List<GameObject> thePlayers, GameObject[] theEnemies) {
        //Debug.Log("BattleManager: Initialize Units");

        int playerIndex = 0;
        int enemyIndex = 0;

        players = new GameObject[thePlayers.Count];
        enemies = new GameObject[theEnemies.Length];

        for (int i = 0; i < thePlayers.Count; i++) {
            if (thePlayers[i] != null)
            {
                GameObject player = Instantiate(thePlayers[i], playerPositions[i], Quaternion.identity);
                player.transform.SetParent(playerSpawnPoint, false);

                players[playerIndex] = player;
                playerIndex++;
            }
        }

        for (int i = 0; i < theEnemies.Length; i++) {
            if (theEnemies[i] != null)
            {
                GameObject enemy = Instantiate(theEnemies[i], enemyPositions[i], Quaternion.identity);
                enemy.transform.SetParent(enemySpawnPoint, false);
                Character character = enemy.GetComponent<Character>();

                character.characterData.ResetHealth();
                character.currentHealth = character.characterData.currentHealth;

                enemies[enemyIndex] = enemy;
                enemyIndex++;
            }
        }
    }

    public void Slide(Vector3 slideTargetPosition, bool isSlidingToEnemy)
    {
        //Debug.Log("BattleManager: Slide");
        Vector3 inFront = new Vector3(slideTargetPosition.x, slideTargetPosition.y);

        this.slideTargetPosition = inFront;
        isSliding = true; // Enable sliding
        slideState = isSlidingToEnemy ? SlideState.SlidingToEnemy : SlideState.SlidingBack;
    }

    public void DoQuickTimeEvent() {
        //Debug.Log("BattleManager: Do Quick Time Event");

        if (currentCharacter.currentAttack.doesItSlide) {
            uiManager.setQuickTimePositionSlide(slideTargetPosition);
        } else {
            uiManager.setQuickTimePositionNoSlide(currentCharacter);
        }

        uiManager.quickTimeEventBar.SetActive(true);
        isEvent = true;
    }

    public void EndTurn(Character character) {
        //Debug.Log("BattleManager: End Turn");
        queue.Add(character);
        queue.Sort((a, b) => a.TU.CompareTo(b.TU));
        uiManager.updateTimeline();
        uiManager.UpdateEnemyHealth();
        uiManager.UpdatePlayerHealth();
        uiManager.ClearButtons();
        currentPosition = new Vector3(0,0,0);

        bool allEnemiesDefeated = enemies.All(enemy => enemy == null || enemy.GetComponent<Character>().currentHealth <= 0);

        bool allPlayersDefeated = players.All(player => player == null || player.GetComponent<Character>().currentHealth <= 0);

        foreach (GameObject player in players) {
            player.transform.position = player.GetComponent<Character>().originalPosition;
        }
        foreach (GameObject enemy in enemies) {
            enemy.transform.position = enemy.GetComponent<Character>().originalPosition;
        }

        if (CheckBattleOver()) {
            if (allEnemiesDefeated) {
                showResults(true);
            } else if (allPlayersDefeated) {
                EndBattle(false);
            }
        } else {
            StartNextTurn();
        }
    }

    public bool CheckBattleOver() {
        bool allEnemiesDefeated = enemies.All(enemy => enemy == null || enemy.GetComponent<Character>().currentHealth <= 0);

        bool allPlayersDefeated = players.All(player => player == null || player.GetComponent<Character>().currentHealth <= 0);

        if (allEnemiesDefeated || allPlayersDefeated) {
            //Debug.Log("Battle Over!");
            return true;
        }

        return false;
    }

    private void showResults(bool playerWon) {
        uiManager.UpdateResultsScreen();
        uiManager.resultsScreen.SetActive(true);

        playerBattle.Gold += uiManager.goldAmount;
        CharacterData.currentExp += (int) (uiManager.goldAmount / 5.5);

        UpdateEXP();
        
        uiManager.actionButtonPanel.SetActive(false);
    }

    private void UpdateEXP() {
        
        if (CharacterData.currentExp >= CharacterData.maxExp) {
            CharacterData.currentExp -= CharacterData.maxExp;
            CharacterData.level++;

            for (int i = 0; i < players.Length; i++) {
                players[i].GetComponent<Character>().characterData.baseAttack = (int) (players[i].GetComponent<Character>().characterData.baseAttack * 1.3);
                players[i].GetComponent<Character>().characterData.maxHealth = (int) (players[i].GetComponent<Character>().characterData.maxHealth * 1.1);
            }
        }
    }

    public void EndBattle(bool playerWon) {
        playerInventory.inventory.AddList(uiManager.itemAwards);

        isBattleActive = false;

        allCameras.SetActive(true);

        cvc.m_Lens.OrthographicSize = 6;
        cams.orthographicSize = 6;


        if (playerWon) {
            //Debug.Log("Victory! Going to victory screen...");
        } else {
            //Debug.Log("You suck at this nerd");
        }

        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        uiManager.HideActionUI();

        foreach (var player in players)
        {
            if (player != null)
            {
                Character character = player.GetComponent<Character>();
                character.characterData.UpdateHealth(character.currentHealth);
                character.characterData.UpdateMaxHealth(character.maxHealth);
            }
        }

        ResetBattle();

        playerMovement.canMove = true;
        isBattleActive = false;

        cameraTransform.position = new Vector3(0,0,-10);
    }

    public void ResetBattle() {
        //Debug.Log("BattleManager: Resetting Battle");

        // Reset players and enemies
        foreach (var player in players)
        {
            if (player != null) player.GetComponent<Character>().ResetCharacter();
        }
        foreach (var enemy in enemies)
        {
            if (enemy != null) enemy.GetComponent<Character>().ResetCharacter();
        }


        // Clear players and enemies
        foreach (var player in players) {
            if (player != null) Destroy(player);
        }

        foreach (var enemy in enemies) {
            if (enemy != null) Destroy(enemy);
        }

        currentCharacter = null;

        players = new GameObject[0];
        enemies = new GameObject[0];

        // Clear queue
        queue.Clear();

        // Reset UI
        uiManager.ResetUI();

        // Reset flags
        isBattleActive = false;
        selectedEnemy = null;

        //Debug.Log("BattleManager: Battle reset complete.");
    }

    
    void Update()
    {
        
        if (isSliding && currentCharacter != null && currentCharacter.gameObject != null)
        {
            currentCharacter.gameObject.transform.position += (slideTargetPosition - currentCharacter.gameObject.transform.position) * slideSpeed * Time.deltaTime;

            if (Vector3.Distance(currentCharacter.gameObject.transform.position, slideTargetPosition) < 0.05f)
            {
                currentCharacter.gameObject.transform.position = slideTargetPosition; // Snap to position
                isSliding = false; // Stop sliding

                if (slideState == SlideState.SlidingToEnemy && currentCharacter.isPlayer) {
                    DoQuickTimeEvent();
                } else if (slideState == SlideState.SlidingToEnemy && !currentCharacter.isPlayer) {
                    currentCharacter.anim.SetBool("isFighting", true);
                } else if (slideState == SlideState.SlidingBack) {
                    slideState = SlideState.None;
                    EndTurn(currentCharacter);
                }
            }
            
        }
        
    }

}
