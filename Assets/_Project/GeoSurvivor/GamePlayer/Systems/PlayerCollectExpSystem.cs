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

                
                if (_player.hasPlayerGainExp)
                {
                    // accumulate exp if gains more than 1 in same frame
                    int expToGain = _player.playerGainExp.ExpToGain;
                    _player.ReplacePlayerGainExp(expToGain + _player.playerGainExp.ExpToGain); 
                }
                else
                {
                    _player.AddPlayerGainExp(expEntity.exp.ExpValue);
                }
                expEntity.isToBeDestroyed = true;
            }
        }
    }
}