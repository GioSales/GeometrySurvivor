using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameSystems
{
    public class ProjectileMovementSystem : IExecuteSystem
    {
        readonly GameContext _gameContext;
        
        public ProjectileMovementSystem(Contexts contexts)
        {
            _gameContext = contexts.game;
        }


        public void Execute()
        {
            IGroup<GameEntity> projectileEntities = _gameContext.GetGroup(GameMatcher.Projectile);
            foreach (GameEntity e in projectileEntities)
            {
                Vector2 dir = e.projectileDirection.Value;
                
                Vector2 newPosition = e.position.Value + (dir * e.projectile.Speed * Time.deltaTime);
                e.ReplacePosition(newPosition);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                e.ReplaceDirection(angle);
            }
        }
    }
}