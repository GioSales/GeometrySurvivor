using Entitas;
using GameComponents;
using GeoSurvivor;

namespace GameSystems
{
    public class CollisionSystem : IExecuteSystem
    {
        readonly GameContext _context;
        readonly IGroup<GameEntity> _enemies;
        readonly IGroup<GameEntity> _projectiles;
        
        public CollisionSystem(Contexts contexts)
        {
            _context = contexts.game;
            _enemies = contexts.game.GetGroup(GameMatcher.Enemy);
            _projectiles = contexts.game.GetGroup(GameMatcher.Projectile);
        }
        
        public void Execute()
        {
            // loop projectiles
            // loop enemies
            // check each

            foreach (GameEntity projEntity in _projectiles)
            {
                foreach (GameEntity enemyEntity in _enemies)
                {
                    CircleColliderComponent projColl = projEntity.circleCollider;
                    CircleColliderComponent enemyColl = enemyEntity.circleCollider;
                    bool colliding = CollisionCheckApi.IsColliding(projEntity.position, projColl, enemyEntity.position, enemyColl);
                    if (colliding)
                    {
                        GameEntity messageEntity = _context.CreateEntity();
                        messageEntity.AddDebugMessage("Collision between: " + projEntity.view.GameObject.name + ", " + enemyEntity.view.GameObject.name);
                    }
                }
            }
        }
    }
}