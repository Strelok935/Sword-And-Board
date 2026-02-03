public static class PlayerInventoryMemory
{
    private static InventorySystem cachedInventory;

    public static InventorySystem GetOrStore(InventorySystem fallbackInventory)
    {
        if (cachedInventory == null)
        {
            cachedInventory = fallbackInventory;
        }

        return cachedInventory;
    }

    public static void Store(InventorySystem inventory)
    {
        if (inventory == null)
        {
            return;
        }

        cachedInventory = inventory;
    }
}
