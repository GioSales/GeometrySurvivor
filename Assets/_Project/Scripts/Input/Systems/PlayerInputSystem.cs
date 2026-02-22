using Entitas;
using GameSystems;

namespace Input.Systems
{
    public class PlayerInputSystem : IExecuteSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        
        public PlayerInputSystem(Contexts contexts)
        {
            _context = contexts.game;
        }
        
        public void Execute()
        {
            
        }
    }
}