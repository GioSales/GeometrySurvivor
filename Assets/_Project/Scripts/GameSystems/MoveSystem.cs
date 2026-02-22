using Entitas;
using UnityEngine;

namespace GameSystems
{
    public class MoveSystem : IExecuteSystem
    {
        readonly IGroup<GameEntity> _moves;
        const float _speed = 4f;

        public MoveSystem(Contexts contexts)
        {
            _moves = contexts.game.GetGroup(GameMatcher.Move);
        }

        public void Execute()
        {
            foreach (GameEntity e in _moves.GetEntities())
            {
                Vector2 dir = e.move.Target - e.position.Value;
                float dist = dir.magnitude;
                if (dist <= 0.5f)
                    continue;
                
                Vector2 newPosition = e.position.Value + dir.normalized * _speed * Time.deltaTime;
                e.ReplacePosition(newPosition);

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                e.ReplaceDirection(angle);
            }
        }
    }
}