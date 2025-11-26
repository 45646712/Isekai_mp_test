namespace Data;

public static class ItemConstants
{
    public enum ItemType
    {
        Crop,
        Fish,
        Building
    }

    public enum ItemCategory
    {
        Materials = 1,
        Resources,
        Valuables,
        Quest,
        Exotics,
        Others
    }

    public enum ResourceType
    {
        Exp,
        Item,
        Currency_Gold,
        Time,
    }
}