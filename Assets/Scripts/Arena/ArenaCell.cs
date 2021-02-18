namespace Assets.Scripts.Arena
{
    class ArenaCell
    {
        public WallTypes wallTypes;
        public bool isAccessible = false;
    }
    enum WallTypes
    {
        None,
        Horizontal,
        Vertical
    }
}