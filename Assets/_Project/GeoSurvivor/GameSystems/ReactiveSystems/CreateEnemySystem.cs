using System.Collections.Generic;
using Entitas;
using GeoSurvivor.Extensions;
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
                GameContextExtensions.CreateEnemy(_gameContext, e.mouseDown.position);
            }
        }
    }
}