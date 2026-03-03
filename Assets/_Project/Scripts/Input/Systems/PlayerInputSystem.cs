using Entitas;
using GamePlayer;
using Rewired;
using UnityEngine;

namespace Input.Systems
{
    public class PlayerInputSystem : IExecuteSystem, IInitializeSystem
    {
        readonly GameContext _context;
        GameEntity _player;
        Camera _camera;
        
        private readonly Player _rewiredPlayer;
        
        public PlayerInputSystem(Contexts contexts)
        {
            _context = contexts.game;
            // TODO: stop using system player, use system player only for debugging actions?
            _rewiredPlayer = ReInput.players.GetSystemPlayer();
        }
        
        public void Initialize()
        {
            _camera = Camera.main;
        }
        
        public void Execute()
        {
            _player = _context.playerEntity;
            
            float horMovement = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveHorizontal);
            _player.playerMovement.HorizontalAxis = horMovement;
            
            float verMovement = _rewiredPlayer.GetAxis(RewiredConsts.Action.MoveVertical);
            _player.playerMovement.VerticalAxis = verMovement;

            PlayerActionComponent actionComponent = _player.playerAction;
            actionComponent.BasicAtkActive = _rewiredPlayer.GetButton(RewiredConsts.Action.BasicAttack);
            
            Mouse mouse = ReInput.controllers.Mouse;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mouse.screenPosition);
            // TODO: cooldown
            if (actionComponent.BasicAtkActive)
            {
                GameEntity projectile = _context.CreateEntity();
                projectile.AddProjectile(newSpeed: 0.5f);
                Vector2 direction = mousePosition - _player.position.Value;
                projectile.AddProjectileDirection(direction);
                projectile.AddPosition(_player.position.Value);
                projectile.AddDirection(0);
                projectile.AddSprite("Capsule");
                projectile.AddSpriteSize(new Vector3(0.04f, 0.08f, 1));
            }
        }
    }
}