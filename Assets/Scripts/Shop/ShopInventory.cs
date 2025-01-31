using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopInventory {
    
    [System.Serializable]
    public class ItemSlot {
        public ItemData itemData;
        public int price;

        public ItemSlot() {
            itemData = null;
            price = 10;
        }
    }

    public List<ItemSlot> slots = new List<ItemSlot>();

    public ShopInventory(int numSlots) {
        for (int i = 0; i < numSlots; i++) {
            slots.Add(new ItemSlot());
        }
    }

    
}
