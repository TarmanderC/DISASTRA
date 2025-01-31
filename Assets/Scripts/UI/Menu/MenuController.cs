using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public TabController tabController;
    public CharacterTabController characterTabController;
    public QuestsTabController questsTabController;
    
    public PlayerInventory playerInventory;
    public KnightBattle knightBattle;

    void Start() {
        menuCanvas.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            UpdateTabs();

            //Debug.Log(CharacterData.currentExp);
        }
    }

    public void UpdateTabs() {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        tabController.ActivateTab(0);
        characterTabController.UpdateTabs();
        characterTabController.ActivateTab(0);
        playerInventory.UpdateInventory();
        questsTabController.UpdateTabs();
        questsTabController.ActivateTab(0);
    }
}
