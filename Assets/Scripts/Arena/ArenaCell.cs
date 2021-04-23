using UnityEngine;
public class ArenaCell
{
    public WallTypes wallTypes;
    public bool isAccessible = false;
    public Vector2 position;
    public bool isThereFeature = false;
}
public enum WallTypes
{
    None,
    Horizontal,
    Vertical
}
