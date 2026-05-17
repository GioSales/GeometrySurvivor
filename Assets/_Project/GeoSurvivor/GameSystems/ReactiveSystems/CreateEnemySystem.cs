using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameSystems
{
    public class CreateEnemySystem : ReactiveSystem<InputEntity>
    {
        readonly GameContext _gameContext;
        public CreateEnemySystem(Contexts contexts) : base(contexts.input)
        {
            _gameContext = contexts.game;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.AllOf(InputMatcher.RightMouse, InputMatcher.MouseDown));
        }

        protected override bool Filter(InputEntity entity)
        {
            return entity.hasMouseDown;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (InputEntity e in entities)
            {
                GameEntity enemy = _gameContext.CreateEntity();
                enemy.isEnemy = true;
                enemy.AddEnemyMoveTarget(newTarget: Vector2.zero, newMoveSpeed: 0.5f);
                enemy.AddPosition(e.mouseDown.position);
                enemy.AddDirection(Random.Range(0,360));
                enemy.AddSprite("Triangle");
                enemy.AddSpriteSize(new Vector3(0.15f, 0.15f, 1));
                enemy.AddCircleCollider(0.12f);
            }
        }
    }
}