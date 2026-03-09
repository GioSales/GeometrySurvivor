using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace GameSystems
{
    public class ProjectileDestroySystem : ReactiveSystem<GameEntity>
    {
        readonly GameContext _context;

        public ProjectileDestroySystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }
        
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.ToBeDestroyed, GameMatcher.Projectile));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isToBeDestroyed;
        }
        
        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity e in entities)
            {
                // TODO: trigger VFX and etc
            }
        }
    }
}