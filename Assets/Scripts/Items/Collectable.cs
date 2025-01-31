[System.Serializable]
public class Collectable {
    public ItemData itemData;
    public int count;

    public Collectable(ItemData itemData, int count) {
        this.itemData = itemData;
        this.count = count;
    }
}
