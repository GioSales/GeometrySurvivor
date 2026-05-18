using Entitas;
using Rewired;
using UnityEngine;

namespace GamePlayer
{
    // TODO: Use ReplacePlayerBasicAtk() instead of direct field mutation for CdTimer,
    //       so reactive systems can detect changes (same issue in PlayerInputSystem for IsActive)
    // TODO: Extract hardcoded projectile values (speed, sprite, size, lifetime) into a config component
    public class PlayerActionSystem : IExecuteSystem, IInitializeSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        
        public PlayerActionSystem(Contexts contexts)
        {
            _context = contexts.game;
        }
        
        public void Initialize()
        {
            _player = _context.playerEntity;
        }
        
        public void Execute()
        {
            PlayerBasicAtkComponent basicAtk = _player.playerBasicAtk;

            if (basicAtk.IsActive && basicAtk.CdTimer <= 0)
            {
                // TODO: move projectile creation to a factory or somewhere, more scalable
                GameEntity projectile = _context.CreateEntity();
                projectile.AddProjectile(newSpeed: 3.5f);
                Vector2 direction = _player.playerAimDirection.Value;
                projectile.AddProjectileDirection(direction);
                projectile.AddPosition(_player.position.Value);
                projectile.AddDirection(0);
                projectile.AddSprite("Capsule");
                projectile.AddSpriteSize(new Vector3(0.04f, 0.08f, 1));
                projectile.AddLifeTime(2f);
                projectile.AddCircleCollider(0.1f);
                projectile.AddDealDamage(1);

                basicAtk.CdTimer = basicAtk.Cooldown;
            }

            if (basicAtk.CdTimer > 0)
            {
                basicAtk.CdTimer -= Time.deltaTime;
            }
        }
    }
}