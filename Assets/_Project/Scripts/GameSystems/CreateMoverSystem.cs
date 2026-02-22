using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameSystems
{
    public class CreateMoverSystem : ReactiveSystem<InputEntity>
    {
        readonly GameContext _gameContext;
        public CreateMoverSystem(Contexts contexts) : base(contexts.input)
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
                mover.isMover = true;
                mover.AddMove(Vector2.zero);
                mover.AddPosition(e.mouseDown.position);
                mover.AddDirection(Random.Range(0,360));
                mover.AddSprite("Triangle");
                mover.AddSpriteSize(new Vector3(0.15f, 0.15f, 1));
            }
        }
    }
}