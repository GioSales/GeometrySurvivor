using Entitas;

namespace GameSystems
{
    public class EnemyFollowPlayerSystem : IInitializeSystem, IExecuteSystem
    {
        readonly GameContext _context;
        readonly IGroup<GameEntity> _enemies;
        GameEntity _player;

        private const float _speed = 0.5f;

        public EnemyFollowPlayerSystem(Contexts contexts)
        {
            _context = contexts.game;
            _enemies = contexts.game.GetGroup(GameMatcher.Mover);
        }
        
        public void Initialize()
        {
            _player = _context.playerEntity;
        }
        
        public void Execute()
        {
            foreach (var enemy in _enemies)
            {
                enemy.move.Target = _player.position.Value;
            }
        }
    }
}