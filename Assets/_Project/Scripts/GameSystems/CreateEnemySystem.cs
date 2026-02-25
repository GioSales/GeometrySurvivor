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
                GameEntity mover = _gameContext.CreateEntity();
                mover.isEnemy = true;
                mover.AddEnemyMoveTarget(newTarget: Vector2.zero, newMoveSpeed: 0.5f);
                mover.AddPosition(e.mouseDown.position);
                mover.AddDirection(Random.Range(0,360));
                mover.AddSprite("Triangle");
                mover.AddSpriteSize(new Vector3(0.15f, 0.15f, 1));
            }
        }
    }
}