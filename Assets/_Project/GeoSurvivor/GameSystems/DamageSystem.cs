using System.Collections.Generic;
using Entitas;
using GeoSurvivor.Extensions;

namespace GameSystems
{
    public class DamageSystem : ReactiveSystem<GameEntity>
    {
        readonly GameContext _context;

        public DamageSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.TakeDamage);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasHealth;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity e in entities)
            {
                e.health.Value -= e.takeDamage.Value;
                
                GameContextExtensions.CreateMessage(_context, e.takeDamage.Value + " Damage dealt to " + e.view.GameObject.name);
                
                if (e.health.Value <= 0)
                {
                    e.isToBeDestroyed = true;
                }
                    
            }
        }
    }
}