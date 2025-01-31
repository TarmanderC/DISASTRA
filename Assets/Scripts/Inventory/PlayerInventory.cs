using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    
    public Inventory inventory;

    public List<GameObject> inventoryBoxes = new List<GameObject>();

    public GameManager gameManager;

    // This is only here because the stupid UnityEditor doesn't 
    // run methods with more than one parameter
    // bruh
    public int quantity;

    private void Awake() {
        inventory = new Inventory(24);
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    public void UpdateInventory() {
        for (int i = 0; i < inventoryBoxes.Count; i++) {
            GameObject itemBox = inventoryBoxes[i];
            Image itemIcon = itemBox.transform.Find("Image").GetComponent<Image>();
            TextMeshProUGUI itemText = itemBox.transform.Find("Number").GetComponent<TextMeshProUGUI>();
            Button itemButton = itemBox.GetComponent<Button>();

            if (inventory.slots[i].itemData != null) {
                itemIcon.gameObject.SetActive(true);
                itemIcon.sprite = inventory.slots[i].itemData.itemImage;
                itemText.text = "" + inventory.slots[i].count;

                itemButton.onClick.RemoveAllListeners();
                int index = i;
                AssignItemButton(index, itemButton);
            } else {
                itemIcon.gameObject.SetActive(false);
                itemIcon.sprite = null;
                itemText.text = "";
            }
        }
    }

    public void AssignItemButton(int slotIndex, Button itemButton)
    {
        itemButton.onClick.AddListener(() => UseItem(slotIndex));
    }

    public void SetQuantity(int count) {
        quantity = count;
    }
    public void AddItem(ItemData itemData) {
        inventory.Add(itemData, quantity);
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.slots.Count)
            return;

        Inventory.Slot slot = inventory.slots[slotIndex];

        if (slot.itemData != null && slot.count > 0)
        {
            if (slot.itemData.isConsumable)
            {
                // Apply the item effect
                ApplyItemEffect(slot.itemData);

                // Remove one item from the inventory
                inventory.Remove(slot.itemData, 1);
                UpdateInventory(); // Refresh UI
            }
        }
    }

    private void ApplyItemEffect(ItemData item)
    {
        if (item.isConsumable)
        {

            foreach (Character player in gameManager.allPlayers) {
                player.Heal(item.healAmount);
                Debug.Log(player.characterData.characterName + " healed " + item.healAmount + " HP");
            }
        }
    }
}
