using Entitas;

namespace GamePlayer
{
    public class PlayerActionSystem : IExecuteSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        
        public PlayerActionSystem(Contexts contexts)
        {
            _context = contexts.game;
            _player = _context.playerEntity;
        }

        public void Execute()
        {
            
        }
    }
}