using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GamePlayer
{
    public class PlayerActionSystem : ReactiveSystem<InputEntity>, IInitializeSystem
    {
        readonly GameContext _gameContext;
        private GameEntity _player;
        public PlayerActionSystem(Contexts contexts) : base(contexts.input)
        {
            _gameContext = contexts.game;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(InputMatcher.AllOf(InputMatcher.LeftMouse, InputMatcher.MouseDown));
        }

        protected override bool Filter(InputEntity entity)
        {
            return entity.hasMouseDown;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            foreach (InputEntity e in entities)
            {
                GameEntity projectile = _gameContext.CreateEntity();
                projectile.AddProjectile(newSpeed: 0.5f);
                Vector2 direction = e.mousePosition.position - _player.position.Value;
                projectile.AddProjectileDirection(direction);
                projectile.AddPosition(_player.position.Value);
                projectile.AddDirection(0);
                projectile.AddSprite("Capsule");
                projectile.AddSpriteSize(new Vector3(0.04f, 0.08f, 1));
            }
        }

        public void Initialize()
        {
            _player = _gameContext.playerEntity;
        }
    }
}