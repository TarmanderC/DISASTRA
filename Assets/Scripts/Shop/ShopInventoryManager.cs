using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ShopInventoryManager : MonoBehaviour
{
    public ShopInventory shopInventory;

    public GameObject[] shopItemBoxes;

    public GameObject shopItemBoxPrefab;
    public Transform itemContainer;

    public PlayerInventory playerInventory;
    public KnightBattle knightBattle;

    void Awake() {
        shopItemBoxes = new GameObject[shopInventory.slots.Count];
        InitializeInventory();
    }

    public void InitializeInventory() {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < shopItemBoxes.Length; i++)
        {
            GameObject itemBox = Instantiate(shopItemBoxPrefab, itemContainer);

            Image itemIcon = itemBox.transform.Find("ItemIcon").GetComponent<Image>();
            TextMeshProUGUI itemText = itemBox.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemPrice = itemBox.transform.Find("Price").Find("CoinText").GetComponent<TextMeshProUGUI>();
            Button buyButton = itemBox.transform.Find("BuyButton").GetComponent<Button>();

            itemIcon.sprite = shopInventory.slots[i].itemData.itemImage;
            itemText.text = shopInventory.slots[i].itemData.itemName;
            itemPrice.text = shopInventory.slots[i].price.ToString();
            int index = i;
            buyButton.onClick.AddListener(() => {
                if (knightBattle.Gold >= shopInventory.slots[index].price) {
                    knightBattle.Gold -= shopInventory.slots[index].price;
                    playerInventory.SetQuantity(1);
                    playerInventory.AddItem(shopInventory.slots[index].itemData);
                }
            });

            shopItemBoxes[i] = itemBox;
        }
    }

    public void UpdateShopInventory() {
        for (int i = 0; i < shopItemBoxes.Length; i++)
        {
            Button buyButton = shopItemBoxes[i].transform.Find("BuyButton").GetComponent<Button>();

            if (shopInventory.slots[i].price > knightBattle.Gold) {
                buyButton.interactable = false;
            } else {
                buyButton.interactable = true;
            }
        }
    }

    void Update() {
        UpdateShopInventory();
    }
}
