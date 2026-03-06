using Entitas;
using Rewired;
using UnityEngine;

namespace GamePlayer
{
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
                Mouse mouse = ReInput.controllers.Mouse;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouse.screenPosition);
                
                GameEntity projectile = _context.CreateEntity();
                projectile.AddProjectile(newSpeed: 3.5f);
                Vector2 direction = mousePosition - _player.position.Value;
                direction.Normalize();
                projectile.AddProjectileDirection(direction);
                projectile.AddPosition(_player.position.Value);
                projectile.AddDirection(0);
                projectile.AddSprite("Capsule");
                projectile.AddSpriteSize(new Vector3(0.04f, 0.08f, 1));
                projectile.AddLifeTime(2f);

                basicAtk.CdTimer = basicAtk.Cooldown;
            }

            if (basicAtk.CdTimer > 0)
            {
                basicAtk.CdTimer -= Time.deltaTime;
            }
        }
    }
}