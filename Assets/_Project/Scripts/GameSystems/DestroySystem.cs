using Entitas;

namespace GameSystems
{
    public class DestroySystem : IExecuteSystem
    {
        readonly GameContext _context;
        
        public DestroySystem(Contexts contexts)
        {
            _context = contexts.game;
        }
        
        public void Execute()
        {
            GameEntity[] entitiesToDestroy = _context.GetGroup(GameMatcher.Destroyed).GetEntities();
            foreach (GameEntity e in entitiesToDestroy)
            {
                e.isDestroyed = false;
                e.Destroy();
            }
        }
    }
}