using Entitas;
using GameComponents;
using UnityEngine;

namespace GameSystems
{
    public class EnemyMoveSystem : IExecuteSystem
    {
        readonly IGroup<GameEntity> _enemies;

        public EnemyMoveSystem(Contexts contexts)
        {
            _enemies = contexts.game.GetGroup(GameMatcher.Enemy);
        }

        public void Execute()
        {
            foreach (GameEntity e in _enemies.GetEntities())
            {
                EnemyMoveTargetComponent enemyMoveTarget = e.enemyMoveTarget;
                Vector2 dir = enemyMoveTarget.Target - e.position.Value;
                float dist = dir.magnitude;
                if (dist <= 0.5f)
                    continue;
                
                Vector2 newPosition = e.position.Value + dir.normalized * enemyMoveTarget.MoveSpeed * Time.deltaTime;
                e.ReplacePosition(newPosition);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                e.ReplaceDirection(angle);
            }
        }
    }
}