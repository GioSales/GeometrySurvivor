using Entitas;

namespace GameSystems
{
    //TODO: refactor to allow feeding in different targets for enemies instead of hardcoding to follow player?
    public class EnemyFollowPlayerSystem : IInitializeSystem, IExecuteSystem
    {
        readonly GameContext _context;
        readonly IGroup<GameEntity> _enemies;
        GameEntity _player;

        private const float _speed = 0.5f;

        public EnemyFollowPlayerSystem(Contexts contexts)
        {
            _context = contexts.game;
            _enemies = contexts.game.GetGroup(GameMatcher.Enemy);
        }
        
        public void Initialize()
        {
            _player = _context.playerEntity;
        }
        
        public void Execute()
        {
            foreach (GameEntity enemy in _enemies)
            {
                enemy.enemyMoveTarget.Target = _player.position.Value;
            }
        }
    }
}