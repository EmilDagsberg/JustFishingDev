[System.Serializable]
public class InventorySlot
{
    public FishData fish;
    public int amount;

    public bool IsEmpty()
    {
        return fish == null || amount <= 0;
    }

    public void Clear()
    {
        fish = null;
        amount = 0;
    }
}