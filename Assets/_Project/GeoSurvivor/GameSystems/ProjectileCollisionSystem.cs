using Entitas;
using GameComponents;
using GeoSurvivor;

namespace GameSystems
{
    public class ProjectileCollisionSystem : IExecuteSystem
    {
        readonly GameContext _context;
        readonly IGroup<GameEntity> _enemies;
        readonly IGroup<GameEntity> _projectiles;
        
        public ProjectileCollisionSystem(Contexts contexts)
        {
            _context = contexts.game;
            _enemies = contexts.game.GetGroup(GameMatcher.Enemy);
            _projectiles = contexts.game.GetGroup(GameMatcher.Projectile);
        }
        
        public void Execute()
        {
            // TODO: do spatial hashing instead of brute force O(N*M)

            foreach (GameEntity projEntity in _projectiles)
            {
                foreach (GameEntity enemyEntity in _enemies)
                {
                    CircleColliderComponent projColl = projEntity.circleCollider;
                    CircleColliderComponent enemyColl = enemyEntity.circleCollider;
                    bool colliding = CollisionCheckApi.IsColliding(projEntity.position, projColl, enemyEntity.position, enemyColl);
                    if (!colliding) 
                        continue;
                    
                    // prevents same projectile from hitting the same enemy multiple times
                    bool uniqueHit = projEntity.dealDamage.DamagedEntities.Add(enemyEntity);
                    if (!uniqueHit)
                        continue;
                    
                    enemyEntity.AddTakeDamage(projEntity.dealDamage.Value);
                    // TODO: remove this later when projectiles can pierce 
                    projEntity.isToBeDestroyed = true;
                }
            }
        }
    }
}