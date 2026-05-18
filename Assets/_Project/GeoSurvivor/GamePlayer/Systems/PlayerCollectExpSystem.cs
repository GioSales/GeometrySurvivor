using Entitas;
using GameComponents;
using GeoSurvivor;
using GeoSurvivor.Extensions;

namespace GamePlayer
{
    public class PlayerCollectExpSystem : IExecuteSystem, IInitializeSystem
    {
        readonly GameContext _context;
        readonly IGroup<GameEntity> _expBlobs;
        GameEntity _player;

        public PlayerCollectExpSystem(Contexts contexts)
        {
            _context = contexts.game;
            _expBlobs = contexts.game.GetGroup(GameMatcher.Exp);
        }
        
        public void Initialize()
        {
            _player = _context.playerEntity;
        }
        
        public void Execute()
        {
            CircleColliderComponent playerColl = _player.circleCollider;
            foreach (GameEntity expEntity in _expBlobs)
            {
                CircleColliderComponent expColl = expEntity.circleCollider;
                bool colliding = CollisionCheckApi.IsColliding(expEntity.position, expColl, _player.position, playerColl);
                if (!colliding) 
                    continue;

                _player.playerExp.TotalExp += expEntity.exp.ExpValue;
                GameContextExtensions.CreateMessage(_context, "Player gained " + expEntity.exp.ExpValue + ", Total: " + _player.playerExp.TotalExp);
                expEntity.isToBeDestroyed = true;
            }
        }
    }
}