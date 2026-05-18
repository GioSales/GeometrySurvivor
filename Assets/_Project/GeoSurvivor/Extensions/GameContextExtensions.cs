namespace GeoSurvivor.Extensions
{
    public static class GameContextExtensions
    {
        public static void CreateMessage(GameContext _context, string message)
        {
            GameEntity messageEntity = _context.CreateEntity();
            messageEntity.AddDebugMessage(message);
        }
    }
}