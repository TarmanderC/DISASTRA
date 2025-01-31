using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory {
    
    [System.Serializable]
    public class Slot {
        public ItemData itemData;
        public int count;
        public int maxAllowed;

        public Slot() {
            itemData = null;
            count = 0;
            maxAllowed = 64;
        }

        public bool IsEmpty => itemData == null && count == 0;

        public bool CanAddItem(ItemData itemData, int quantity = 1) {
            return (this.itemData == itemData || IsEmpty) && count + quantity <= maxAllowed;
        }

        public void AddItem(ItemData itemData, int quantity = 1) {
            if (CanAddItem(itemData, quantity)) {
                this.itemData = itemData;
                count += quantity;
            }
        }

        public bool RemoveItem(int quantity = 1) {
            if (count >= quantity) {
                count -= quantity;
                if (count == 0) {
                    itemData = null;
                }
                return true;
            }

            return false;
        }
    }

    public List<Slot> slots = new List<Slot>();

    public Inventory(int numSlots) {
        for (int i = 0; i < numSlots; i++) {
            slots.Add(new Slot());
        }
    }

    public void AddList(List<Collectable> theList) {
        for (int i = 0; i < theList.Count; i++) {
            Add(theList[i].itemData, theList[i].count);
        }
    }

    public void Add(ItemData itemData, int quantity) {
        while (quantity > 0) {
            bool added = false;

            foreach (Slot slot in slots) {
                if (slot.CanAddItem(itemData, 1)) {
                    slot.AddItem(itemData, 1);
                    quantity--;
                    added = true;
                    break;
                }
            }

            if (!added) {
                foreach (Slot slot in slots) {
                    if (slot.IsEmpty) {
                        slot.AddItem(itemData, 1);
                        quantity--;
                        added = true;
                        break;
                    }
                }
            }

            // This is for when the inventory is full
            if (!added) {
                break;
            }
        }
    }

    public bool Remove(ItemData itemData, int quantity) {
        foreach (Slot slot in slots) {
            if (slot.itemData == itemData) {
                int toRemove = Mathf.Min(quantity, slot.count);
                slot.RemoveItem(toRemove);
                quantity -= toRemove;

                if (quantity == 0) {
                    return true;
                }
            }
        }

        return false;
    }

    public bool HasItem(ItemData itemData, int quantity = 1) {
        int totalCount = 0;

        foreach (Slot slot in slots) {
            if (slot.itemData == itemData) {
                totalCount += slot.count;
                if (totalCount >= quantity) {
                    return true;
                }
            }
        }

        return false;
    }
    
}
