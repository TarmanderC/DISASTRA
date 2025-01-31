using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BattleManager bm;
    public UIManager uiManager;
    public MenuController menuController;
    public TabController tabController;
    public CharacterTabController characterTabController;

    public KnightBattle knightBattle;
    public KnightMovement knightMovement;

    public List<Character> allPlayers;

    public Animator partyAnimator;

    void Awake() {
        knightBattle = FindFirstObjectByType<KnightBattle>();
        knightMovement = FindFirstObjectByType<KnightMovement>();
    }
    public void SetUpGM() {
        allPlayers = new List<Character>();
        UpdatePlayers();

        characterTabController.ActivateTab(0);
    }

    public void AddPlayer(GameObject character) {
        bm.playerBattle.currentParty.Add(character);
        
        partyAnimator.SetTrigger("NewParty");
    }

    public void UpdatePlayers() {
        allPlayers.Clear();
        for (int i = 0; i < bm.playerBattle.currentParty.Count; i++) {
            if (bm.playerBattle.currentParty[i] != null) {
                allPlayers.Add(bm.playerBattle.currentParty[i].GetComponent<Character>());
            }
        }
    }

    void Update() {
        UpdatePlayers();
    }
}
